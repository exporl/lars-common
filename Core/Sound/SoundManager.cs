using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Lars.Sound
{
    public enum Channel { Left, Right };

    public class SoundManager : ManagerHelper
    {

        #region Fields, Properties and Inner Classes

        public static SoundManager instance = null;

        public AudioSource soundEffectPlayer;
        public AudioSource backgroundMusicPlayer;

        public AudioSource speechPlayer;
        
        private SoundLibrary soundLib;

        /// <summary>
        /// Target loudness in levelsettings (real world dB)
        /// </summary>
        private float targetVolume = 0;

        /// <summary>
        /// Target loudness property in linear scale
        /// </summary>
        private float volumeLinear
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

            locPlayer = GetComponent<AudioSource>();

            soundLib = GetComponent<SoundLibrary>(); 
        }

        #endregion


        #region Localization

        private AudioSource locPlayer;

        /// <summary>
        /// Global calibration & stimulus level
        /// See Unity documentation on OnAudioFilterRead for more info
        /// </summary>
        /// <param name="data">One block of samples (both channels, interleaved)</param>
        void OnAudioFilterRead(float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i = i + channels)
            {
                data[i + (int)Channel.Left] = data[i + (int)Channel.Left] * GetCalibLIN(Channel.Left) * volumeLinear;
                data[i + (int)Channel.Right] = data[i + (int)Channel.Right] * GetCalibLIN(Channel.Right) * volumeLinear;
            }
        }

        /// <summary>
        /// Calls play on & fades in locPlayer sound
        /// </summary>
        public void StartLocSound(float duration = 3)
        {
            if (locPlayer.isPlaying) return;

            locPlayer.volume = 0;
            locPlayer.Play();
            locPlayer.DOFade(1, duration);
        }

        /// <summary>
        /// Fades out and stops locPlayer
        /// </summary>
        /// <param name="duration">(Optional) Duration of fadeout, default is 3, 0 is insta</param>
        public void StopLocSound(float duration = 3)
        {
            if (duration == 0)
            {
                locPlayer.volume = 0;
                locPlayer.Stop();
            }
            else
            {
                locPlayer.DOFade(0, duration).OnComplete(() => { locPlayer.Stop(); });
            }
        }

        /// <summary>
        /// Instantly stops locPlayer
        /// </summary>
        public void StopLocSoundInsta()
        {
            locPlayer.volume = 0;
            locPlayer.Stop();
        }

        /// <summary>
        /// Fade in sound ( no .play() )
        /// </summary>
        public void FadeInLocSound()
        {
            //TODO create decibel fader
            locPlayer.DOFade(1, 2.0f);
        }

        /// <summary>
        /// Fade out sound ( no .stop() )
        /// </summary>
        public void FadeOutLocSound()
        {
            //todo seem
            locPlayer.DOFade(0.3f, 1.0f);
        }

        /// <summary>
        /// Set LOC stimulus (by clip)
        /// </summary>
        /// <param name="clip">UnityEngine audioclip</param>
        public void SetLocStimulus(AudioClip clip)
        {
            locPlayer.clip = clip;
            locPlayer.Play();
        }

        /// <summary>
        /// Set loudness of stimulus in 'real world dB'
        /// (Not the same as calibration level)
        /// </summary>
        /// <param name="db"></param>
        public void SetLocLevel(float db)
        {
            float calculation = 0;//db - CalibrationManager.instance.calibData.targetLevel;//todo
            targetVolume = calculation;
        }

        public void ResumeLoc()
        {
            locPlayer.UnPause();
        }

        public void PauseLoc()
        {
            locPlayer.Pause();
        }

        #endregion


        #region Calibration

        /// <summary>
        /// Called by calibrationManager when saving & applying
        /// </summary>
        /// <param name="db">loudness in dB</param>
        /// <param name="chan">channel</param>
        public void SetCalibration(float db, Channel chan)
        {
            if (db > 0) db = 0;
            if (chan == Channel.Left)
            {
                targetCalib_L = db;
            }
            else
            {
                targetCalib_R = db;
            }
        }

        /// <summary>
        /// Returns calibration value in decibel scale
        /// </summary>
        /// <param name="chan"></param>
        /// <returns></returns>
        private float GetCalibDB(Channel chan)
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
        private float GetCalibLIN(Channel chan)
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