using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Lars.Sound
{
    public enum Channel { Left, Right, Both };

    public class SoundManager : ManagerHelper
    {

        #region Fields, Properties and Inner Classes

        public static SoundManager instance = null;

        public AudioSource soundEffectPlayer;
        public AudioSource backgroundMusicPlayer;

        internal SoundLibrary soundLib;

        /// <summary>
        /// Target loudness in levelsettings (real world dB)
        /// </summary>
        private float targetVolume = 0;

        /// <summary>
        /// Target loudness property in linear scale
        /// </summary>
        protected double volumeLinear
        {
            get
            {
                return Utils.DecibelToLinear(targetVolume);
            }
        }

        /// <summary>
        /// Left Target calibration for neutral position in decibels (-144 to 0)
        /// </summary>
        private float targetCalib_L = -10;
        /// <summary>
        /// Right Target calibration for neutral position in decibels (-144 to 0)
        /// </summary>
        private float targetCalib_R = -10;

        #endregion

        
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

            //locPlayer = GetComponent<AudioSource>();

            soundLib = GetComponent<SoundLibrary>();
        }
        
        void LateUpdate()
        {
            
        }

        #endregion

        #region Calibration

        /// <summary>
        /// Called by calibrationManager when saving & applying
        /// </summary>
        /// <param name="db">loudness in dB</param>
        /// <param name="chan">channel</param>
        public virtual void SetCalibration(float db, Channel chan)
        {
            //if (db > 0) db = 0;
            if (chan == Channel.Left)
            {
                Debug.Log("Setting vol to: " + db);
                targetCalib_L = db;
            }
            else
            {
                Debug.Log("Setting vol to: " + db);
                targetCalib_R = db;
            }
        }

        /// <summary>
        /// Returns calibration value in decibel scale
        /// </summary>
        /// <param name="chan"></param>
        /// <returns></returns>
        protected float GetCalibDB(Channel chan)
        {
            if (chan == Channel.Left)
                return targetCalib_L;
            else
                return targetCalib_R;
        }

        /// <summary>
        /// Get calibration value in linear scale
        /// </summary>
        /// <param name="chan"></param>
        /// <returns></returns>
        protected double GetCalibLIN(Channel chan)
        {
            if (chan == Channel.Left)
                return Utils.DecibelToLinear(targetCalib_L);
            else
                return Utils.DecibelToLinear(targetCalib_R);
        }

        #endregion


        #region SoundEffects

        public void PlaySoundEffect(AudioClip clip, float vol = 1f)
        {
            soundEffectPlayer.PlayOneShot(clip, vol);
        }

        public void PlaySoundEffect(string clipName, float vol = 0)
        {
            AudioClip clip = soundLib.getFxClip(clipName);
            if(clip != null)
                soundEffectPlayer.PlayOneShot(clip);
        }

        #endregion
        
        public void PlaySpeech(string speechName, float vol = 0)
        {
            AudioClip clip = soundLib.getSpeechClip(speechName);
            if (clip != null)
                speechPlayer.PlayOneShot(clip);
        }
        
    }
}