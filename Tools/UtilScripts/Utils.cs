using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Xml.Serialization;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif


namespace Lars
{
    /// <summary>
    /// A utility static clss
    /// </summary>
    public static class Utils
    {
        public static string version = "alpha"; // TODO make automatic link with commit hash

        private static System.Random Random = new System.Random();

        public static T[][] CopyArrayLinq<T>(T[][] source)
        {
            return source.Select(s => s.ToArray()).ToArray();
        }

        public static void Shuffle<T>(T[] array)
        {
            System.Random rng = new System.Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float RandomRange(float min, float max)
        {
            return min + ((float)Random.NextDouble() * (max - min));
        }

        public static float RandomValue()
        {
            return (float)Random.NextDouble();
        }

        public static T ParseEnum<T>(string val)
        {
            return (T)Enum.Parse(typeof(T), val, true);
        }

        private static float GetRGB(float v1, float v2, float h)
        {
            if (h < 0) h += 1;
            if (h > 1f) h -= 1;
            if (h * 6f < 1f) return v1 + (v2 - v1) * h * 6f;
            if (h * 2f < 1f) return v2;
            if (h * 3f < 2f) return v1 + (v2 - v1) * (2f / 3f - h) * 6f;
            return v1;
        }

        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static bool IsEqual(this Color c, Color d)
        {
            if (c.r == d.r && c.g == d.g && c.b == d.b && c.a == d.a)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clean the memory and reload the scene
        /// </summary>
        public static void ReloadLevel()
        {
            CleanMemory();

#if UNITY_5_3_OR_NEWER
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
#else
            Application.LoadLevel(Application.loadedLevel);
#endif

            CleanMemory();
        }
        /// <summary>
        /// Clean the memory
        /// </summary>
        public static void CleanMemory()
        {
            DOTween.KillAll();
            GC.Collect();
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1f / 60f;
            Time.maximumDeltaTime = 3 * Time.deltaTime;
        }

        /// <summary>
        /// Sanitizes string to generate valid filename
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        /// <summary>
        /// Destroys all child objects of a gameobject.transform
        /// </summary>
        /// <param name="root"></param>
        public static void DestroyChildren(this Transform root)
        {
            int childCount = root.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(root.GetChild(i).gameObject);
            }
        }

        #region File I/O

        /// <summary>
        /// Generic XML saver
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSave"></param>
        /// <param name="filePath"></param>
        public static void saveToXml<T>(T objectToSave, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            //var fname = Path.Combine(Application.persistentDataPath, filePath);

            string fname = Application.persistentDataPath + filePath;

            //  Ensure directory existence
            Directory.CreateDirectory(Path.GetDirectoryName(fname));

            //Debug.Log("SAVED TO: " + fname);
            var encoding = Encoding.GetEncoding("UTF-8");

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (StreamWriter sw = new StreamWriter(fname, false, encoding))
            {
                serializer.Serialize(sw, objectToSave, ns);
            }
        }
        
        public static T loadFromXml<T>(string filePath, Action callback) where T : class, new()
        {
            //var fname = Path.Combine(Application.persistentDataPath, filePath);
            string fname = Application.persistentDataPath + filePath;

            if (!File.Exists(fname))
                return null;

            object result;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                //string fname = Path.Combine(Application.persistentDataPath, "/settings/profiles.xml");
                FileStream stream = new FileStream(fname, FileMode.Open);
                result = serializer.Deserialize(stream) as T;
                stream.Close();
            }
            catch (System.Exception e)
            {
                Debug.Log("could not load xml profiles file: " + e.ToString());
                return null;
            }

            if (callback != null)
                callback();

            return (T)result;
        }

        #endregion
    }
}


namespace Lars.Sound
{
    public static class Utils
    {
        /// <summary>
        /// Converts linear factor (0 to 1) to dB value (-144 to 0)
        /// </summary>
        /// <param name="linear"></param>
        /// <returns>decibel value [-144 - 0]</returns>
        public static float LinearToDecibel(float linear)
        {
            linear = Mathf.Abs(linear);
            if(linear < Mathf.Pow(10f, -144f / 20f))
                return -144f;
            else
                return 20f * Mathf.Log10(linear);
        }

        /// <summary>
        /// Converts dB value to corresponding linear multiplication factor
        /// </summary>
        /// <param name="dB"></param>
        /// <returns>linear value [ 0 - 1 ]</returns>
        public static float DecibelToLinear(float dB)
        {
            return Mathf.Pow(10f, dB / 20f);
        }
    }
}