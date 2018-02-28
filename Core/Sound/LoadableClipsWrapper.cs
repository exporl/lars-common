using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars.Sound
{
    /// <summary>
    /// Monobehaviour wrapper for loadable audioclips
    /// Will be saved as a prefab and then loaded as a Resource at runtime
    /// </summary>
    public class LoadableClipsWrapper : MonoBehaviour
    {
        [SerializeField]
        List<SpeechClip> speechClips;

        public SpeechClip getSpeechClip(string clipName)
        {
            if (!speechClips.Exists(x => x.name == clipName)) return null;
            return speechClips.Find(x => x.name == clipName);
        }
    }

    /// <summary>
    /// Extends soundclip and adds a transcript
    /// Used for tutorial speech etc
    /// </summary>
    [System.Serializable]
    public class SpeechClip : SoundClip
    {
        public string explanation;
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class LoadableSpeechClips
    {
        
    }
}