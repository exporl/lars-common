using UnityEngine;
using System.Collections;

namespace Lars.Race
{
    /*
    public enum enColorchannels
    {
        all = 0,
        red = 1,
        blue = 2,
        green = 3
    }
    public enum enWaveFunctions
    {
        sinus = 0,
        triangle = 1,
        square = 2,
        sawtooth = 3,
        inverted_saw = 4,
        custom = 5
    }
    */
    public class MatFlicker : MonoBehaviour
    {

        public enColorchannels colorChannel = enColorchannels.all;
        public enWaveFunctions waveFunction = enWaveFunctions.sinus;
        public float offset = 0.0f; // constant offset
        public float amplitude = 1.0f; // amplitude of the wave
        public float phase = 0.0f; // start point inside on wave cycle
        public float frequency = 0.5f; // cycle frequency per second
        public bool inverted = false;

        // Keep a copy of the original values
        private Color originalColor;
        private float originalIntensity;


        // Use this for initialization
        void Start()
        {
            originalColor = GetComponent<Renderer>().material.color;
            //originalIntensity = GetComponent<Material>().
        }

        void test()
        {
            GetComponent<Renderer>().material.SetColor("_Color",Color.black);
        }

        // Update is called once per frame
        void Update()
        {
            Color o = originalColor;
            Color c = GetComponent<Renderer>().material.GetColor("_Color");

            if (colorChannel == enColorchannels.all)
                c = originalColor * EvalWave();
            else if (colorChannel == enColorchannels.red)
                c = new Color(o.r * EvalWave(), c.g, c.b, c.a);
            else if (colorChannel == enColorchannels.green)
                c = new Color(c.r, o.g * EvalWave(), c.b, c.a);
            else // blue       
                c = new Color(c.r, c.g, o.b * EvalWave(), c.a);

            GetComponent<Renderer>().material.SetColor("_Color",c);

        }

        private float EvalWave()
        {
            float x = (Time.time + phase) * frequency;
            float y;
            x = x - Mathf.Floor(x); // normalized value (0..1)
            if (waveFunction == enWaveFunctions.sinus)
            {
                y = Mathf.Sin(x * 2f * Mathf.PI);
            }
            else if (waveFunction == enWaveFunctions.triangle)
            {
                if (x < 0.5f)
                    y = 4.0f * x - 1.0f;
                else
                    y = -4.0f * x + 3.0f;
            }
            else if (waveFunction == enWaveFunctions.square)
            {
                if (x < 0.5f)
                    y = 1.0f;
                else
                    y = -1.0f;
            }
            else if (waveFunction == enWaveFunctions.sawtooth)
            {
                y = x;
            }
            else if (waveFunction == enWaveFunctions.inverted_saw)
            {
                y = 1.0f - x;
            }
            else if (waveFunction == enWaveFunctions.custom)
            {
                float y1 = Mathf.Sin(x * 2f * Mathf.PI);
                float y2 = 1f - (Random.value * 2f);
                y = (y1 * 2 + y2) / 3;
            }
            else
            {
                y = 1.0f;

            }

            if (inverted)
                y = 1f - y;

            return (y * amplitude) + offset;

        }
    }
}