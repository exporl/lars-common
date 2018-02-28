using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars.Sound
{
    /// <summary>
    /// Holds all references to locSound, sound fx & background music
    /// </summary>
    public class SoundLibrary : MonoBehaviour
    {
        /// <summary>
        /// pseudo dictionary for all localization stimuli
        /// </summary>
        public List<SoundClip> stimuli;

        /// <summary>
        /// pseudo dict for sound effects
        /// </summary>
        public List<SoundClip> soundFxList;

        public List<SoundClip> speechList;

        void Awake()
        {
            //  Load stimuli from Resources folder
            //  Get stim names from GameConfig

            //  Dictionary for fx
            
            
        }

        public AudioClip getFxClip(string clipName)
        {
            if (!soundFxList.Exists(x => x.name == clipName)) return null;
            return soundFxList.Find(x => x.name == clipName).clip;
        }

        public AudioClip getSpeechClip(string clipName)
        {
            if (!speechList.Exists(x => x.name == clipName)) return null;
            return speechList.Find(x => x.name == clipName).clip;
        }

        AudioClip loadAudioClip(string name)
        {
            return new AudioClip();
        }
    }

    
    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
        public float level; //  (optional) for setting specific sound effect's level
    }
}