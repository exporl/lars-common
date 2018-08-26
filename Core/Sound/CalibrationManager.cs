using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using Lars.UI;
//using UI.Xml;

namespace Lars.Sound
{

    public abstract class CalibrationManager : ManagerHelper
    {
        public static CalibrationManager instance = null;

        protected AudioSource calibPlayer;

        public CalibrationData calibData = new CalibrationData();
        bool dataLoaded;


        #region Methods

        void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }

            else if (instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            calibPlayer = GetComponent<AudioSource>();

            panelController = calibPanel.GetComponent<CalibrationPanelController>();
        }

        void Start()
        {
            //  Calibration profile exists? Apply it
            if (!LoadData())
            {
                uiController.ShowWarning("No calibration profile found");
            }
            else
            {
                ApplyCalibration();
            }
        }

        public CalibrationData GetData()
        {
            if (!dataLoaded) LoadData();
            return calibData;
        }

        #endregion


        #region GUI

        public GameObject calibPanel;
        CalibrationPanelController panelController;

        /// <summary>
        /// Show calibrationpanel
        /// </summary>
        /// <param name="s"></param>
        public void ShowPanel(bool s)
        {
            calibPanel.SetActive(s);

            if (s)
            {
                //soundManager.StopAllSounds();
            }
            else
            {
                StopCalibration();
                ApplyCalibration();
            }
        }

        protected Button GetButton(bool max, bool left)
        {
            string t = max ? "play" : "test";
            string s = left ? "left" : "right";
            return panelController.xmlLayout.GetElementById<Button>(t + s);
        }

        #endregion


        #region Calibration

        protected bool calibrating, calibrateLeft;

        public abstract void StartCalibration(bool max, Channel chan);

        /// <summary>
        /// Plays calib player at max output
        /// </summary>
        /// <param name="chan"></param>
        public void PlayMaxCalibration(Channel chan)
        {
            StartCalibration(true, chan);
        }

        /// <summary>
        /// Plays calib at level that was inputted into UI
        /// </summary>
        /// <param name="chan"></param>
        public void PlayTestCalibration(Channel chan)
        {
            StartCalibration(false, chan);
        }

        /// <summary>
        /// Stop calib player, checks if values are correct
        /// </summary>
        public void StopCalibration()
        {
            calibPlayer.Stop();
            dataLoaded = true;
        }

        /// <summary>
        /// Set calibplayer level with buttons
        /// </summary>
        /// <param name="dB"></param>
        public abstract void SetLevel(int dB);

        /// <summary>
        /// Pass values to SoundManager
        /// </summary>
        public virtual void ApplyCalibration()
        {
            if (!dataLoaded) LoadData();

            if (calibData.measuredAtMax_L < calibData.targetLevel || calibData.measuredAtMax_R < calibData.targetLevel)
            {
                uiController.ShowWarning("Target value should not be greater than measured value");
                return;
            }

            Debug.Log("LEFT difference is: " + GetCalibrationDiff(Channel.Left) + "which is: " + Utils.DecibelToLinear(GetCalibrationDiff(Channel.Left)));
            Debug.Log("RIGHT difference is: " + GetCalibrationDiff(Channel.Right) + "which is: " + Utils.DecibelToLinear(GetCalibrationDiff(Channel.Right)));

            soundManager.SetCalibration(GetCalibrationDiff(Channel.Left), Channel.Left);
            soundManager.SetCalibration(GetCalibrationDiff(Channel.Right), Channel.Right);
        }

        /// <summary>
        /// Returns difference between recorded max (PlayMaxCalibration) & the desired target dB
        /// </summary>
        /// <param name="chan"></param>
        /// <returns></returns>
        public float GetCalibrationDiff(Channel chan)
        {
            if (!dataLoaded) LoadData();

            float diff;

            if (chan == Channel.Left)
            {
                diff = calibData.targetLevel - calibData.measuredAtMax_L;
                if (diff > 0f) diff = 0;
            }
            else
            {
                diff = calibData.targetLevel - calibData.measuredAtMax_R;
                if (diff > 0f) diff = 0;
            }

            return diff;
        }

        #endregion


        #region IO

        public void SaveData()
        {
            ApplyCalibration();

            XmlSerializer serializer = new XmlSerializer(typeof(CalibrationData));

            var fname = Path.Combine(Application.persistentDataPath, "calibration_profile.xml");
            Debug.Log("SAVED TO: " + fname);
            var encoding = Encoding.GetEncoding("UTF-8");

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (StreamWriter sw = new StreamWriter(fname, false, encoding))
            {
                serializer.Serialize(sw, calibData, ns);
            }
        }

        public bool LoadData()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CalibrationData));
                string fname = Path.Combine(Application.persistentDataPath, "calibration_profile.xml");
                FileStream stream = new FileStream(fname, FileMode.Open);
                calibData = serializer.Deserialize(stream) as CalibrationData;
                stream.Close();
                dataLoaded = true;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log("Could not Load XML due to error: " + e.ToString());
                return false;
            }
        }

        #endregion

    }

    [System.Serializable]
    public class CalibrationData
    {
        [XmlElement("measured_at_max_left")]
        public float measuredAtMax_L = 80;
        [XmlElement("measured_at_max_right")]
        public float measuredAtMax_R = 80;
        [XmlElement("target_level")]
        public float targetLevel = 70;

        public CalibrationData()
        { }
    }

}