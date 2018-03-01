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
        /// pseudo dict for sound effects
        /// </summary>
        public List<SoundClip> soundFxList;

        /// <summary>
        /// pseudo dict for sound effects
        /// </summary>
        public List<MultiSoundClip> soundEffectList;

        /// <summary>
        /// pseudo dict for digits
        /// </summary>
        public List<SoundClip> speechList;

        /// <summary>
        /// Sets for character-specific voiceclips
        /// </summary>
        public List<SoundClipSet> voiceSets;

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

        List<ScaledSoundClip> scaledSpeechList = new List<ScaledSoundClip>();
        
        // single ear
        public AudioClip getScaledSpeechClip(string clipName, float dB, Channel chan)
        {
            if (!scaledSpeechList.Exists(x => x.name == clipName && x.dB == dB && x.chan == chan))
            {
                ScaledSoundClip sclip = new ScaledSoundClip();
                sclip.clip = getSpeechClip(clipName).CreateScaledClip(dB, chan);
                sclip.name = clipName;
                sclip.dB = dB;
                sclip.chan = chan;
                scaledSpeechList.Add(sclip);
                return sclip.clip;
            }
            else
            {
                return scaledSpeechList.Find(x => x.name == clipName && x.dB == dB && x.chan == chan).clip;
            }
        }

        // both ear
        public AudioClip getScaledSpeechClip(string clipName, float dBLeft, float dBRight)
        {
            if (!scaledSpeechList.Exists(x => x.name == clipName && x.dBLeft == dBLeft && x.dBRight == dBRight && x.chan == Channel.Both))
            {
                ScaledSoundClip sclip = new ScaledSoundClip();
                sclip.clip = getSpeechClip(clipName).CreateBilateralScaledClip(dBLeft, dBRight);
                sclip.name = clipName;
                sclip.dBLeft = dBLeft;
                sclip.dBRight = dBRight;
                sclip.chan = Channel.Both;
                scaledSpeechList.Add(sclip);
                return sclip.clip;
            }
            else
            {
                return scaledSpeechList.Find(x => x.name == clipName && x.dBLeft == dBLeft && x.dBRight == dBRight && x.chan == Channel.Both).clip;
            }
        }

        public AudioClip getVoiceClip(string currentChar, string clipName)
        {
            List<MultiSoundClip> voiceList = voiceSets.Find(x => x.name == currentChar).voiceList;

            if (!voiceList.Exists(x => x.name == clipName)) return null;
            return voiceList.Find(x => x.name == clipName).clip;
        }

        AudioClip loadAudioClip(string name)
        {
            return new AudioClip();
        }
    }

    public static class AudioClipExtensions
    {
        public static AudioClip CreateScaledClip(this AudioClip originalClip, float dB, Channel chan)//, int targetChannel)
        {
            if (originalClip.frequency != 44100)
            {
                Debug.LogError("CLIP FREQ IS NOT 44.1kHz !");
            }
            // Create a new clip with the target amount of channels.
            AudioClip clip = AudioClip.Create(originalClip.name + "_" + chan.ToString() + "_" + dB.ToString(), originalClip.samples, originalClip.channels, originalClip.frequency, false);

            // Init audio arrays.
            float[] audioData = new float[originalClip.samples * originalClip.channels];
            float[] originalAudioData = new float[originalClip.samples * originalClip.channels];

            if (!originalClip.GetData(originalAudioData, 0))
                return null;

            /*
            // Fill in the audio from the original clip into the target channel. Samples are interleaved by channel (L0, R0, L1, R1, etc).
            int originalClipIndex = 0;
            for (int i = targetChannel; i < audioData.Length; i += amountOfChannels)
            {
                audioData[i] = originalAudioData[originalClipIndex];
                originalClipIndex += originalClip.channels;
            }*/

            float linearVal = (float)Utils.DecibelToLinear(dB);

            /*
            //left
            if (chan == Channel.Left)
            {
                for (int i = 0; i < originalAudioData.Length; i+=2)
                {
                    audioData[i] = originalAudioData[i] * linearVal;
                    audioData[i + 1] = 0.0f;
                }
            }
            else if (chan == Channel.Right)
            {
                for (int i = 0; i < originalAudioData.Length; i+=2)
                {
                    audioData[i] = 0.0f;
                    audioData[i + 1] = originalAudioData[i] * linearVal;
                }
            }
            else
            {
                Debug.LogWarning("Incorrect channel !!");
            }
            */
            bool clipped = false;

            int originalClipIndex = 0;
            for (int i = (int)chan; i < audioData.Length; i += 2)
            {
                audioData[i] = originalAudioData[originalClipIndex] * linearVal;
                originalClipIndex += originalClip.channels;

                if (!clipped && (audioData[i] > 1 || audioData[i] < -1))
                {
                    clipped = true;
                }
            }

            if (clipped)
                Debug.LogWarning("Clipping occurred after scaling soundclip");

            if (!clip.SetData(audioData, 0))
                return null;

            return clip;
        }


        public static AudioClip CreateBilateralScaledClip(this AudioClip originalClip, float dBLeft, float dBRight)//, int targetChannel)
        {
            if (originalClip.frequency != 44100)
            {
                Debug.LogError("CLIP FREQ IS NOT 44.1kHz !");
            }
            // Create a new clip with the target amount of channels.
            AudioClip clip = AudioClip.Create(originalClip.name + "_bilateral_Left_" + dBLeft.ToString() + "_Right_" + dBRight.ToString(), originalClip.samples, originalClip.channels, originalClip.frequency, false);

            // Init audio arrays.
            float[] audioData = new float[originalClip.samples * originalClip.channels];
            float[] originalAudioData = new float[originalClip.samples * originalClip.channels];

            if (!originalClip.GetData(originalAudioData, 0))
                return null;

            float linearValLeft = (float)Utils.DecibelToLinear(dBLeft);
            float linearValRight = (float)Utils.DecibelToLinear(dBRight);

            bool clipped = false;

            for (int i = 0; i < audioData.Length; i+=2)
            {
                audioData[i] = originalAudioData[i] * linearValLeft;
                audioData[i+1] = originalAudioData[i+1] * linearValRight;

                if (!clipped && (audioData[i] > 1 || audioData[i] < -1))
                {
                    clipped = true;
                }
            }

            if (clipped)
                Debug.LogWarning("Clipping occurred after scaling soundclip");

            if (!clip.SetData(audioData, 0))
                return null;

            return clip;
        }
    }

    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
        public float level; //  (optional) for setting specific sound effect's level
    }

    [System.Serializable]
    public class ScaledSoundClip
    {
        public string name;
        public float dB, dBLeft, dBRight;
        public AudioClip clip;
        public Channel chan;
    }

    [System.Serializable]
    public class MultiSoundClip
    {
        public string name;
        public AudioClip[] clips; // Multiple variations of the same clip
        public float level; //  (optional) for setting specific sound effect's level

        public AudioClip clip
        {
            get
            {
                if(clips.Length > 0)
                {
                    return clips[Random.Range(0, clips.Length)];
                }
                return new AudioClip();
            }
        }
    }

    [System.Serializable]
    public class SoundClipSet
    {
        public string name; // must correspond with character name
        public List<MultiSoundClip> voiceList;
    }
}