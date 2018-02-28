using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars.Sound
{
    /// <summary>
    /// Baseclass for OnAudioFilterRead-based audiofilters
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class FilterBase : MonoBehaviour {
        #region Fields, Properties and Inner classes

        protected const int FLOAT_SIZE = 4;
        protected const int BLOCK_SIZE = 1024; //per channel
        protected const int CHANNELCOUNT = 2;

        protected static int SAMPLERATE;
        protected static double SECONDS_PER_SAMPLE, SECONDS_PER_BLOCK;

        private void Awake() {
            SAMPLERATE = AudioSettings.outputSampleRate;
            SECONDS_PER_SAMPLE = 1d / SAMPLERATE;
            SECONDS_PER_BLOCK = SECONDS_PER_SAMPLE * BLOCK_SIZE;
        }

        #endregion



    }

}