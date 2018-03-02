using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
using Lars.Sound;

namespace Visc
{
    public class SoundClipEvent : EventAction
    {
        enum SoundType { Effect = 0, Speech, Digit };
        private string[] typeOptions = System.Enum.GetNames(typeof(SoundType));
        private SoundType _type;// = SoundType.Fade;

        private float _start;

        [SerializeField]
        private string _clipName;

        [SerializeField]
        private float _vol = 1;

        SoundManager sound;

        protected override void OnStart(float startTime)
        {
            _start = startTime;
            _type = ParseEnum<SoundType>(typeOptions[_typeSelect]);

            sound = SoundManager.instance as SoundManager;

            if (string.IsNullOrEmpty(_clipName) || !sound) return;
           
            // Do type-specific action
            switch (_type)
            {
                case SoundType.Effect:

                    sound.PlaySoundEffect(_clipName);

                    break;

                case SoundType.Speech:

                    sound.speech("story_"+_clipName);

                    break;

                case SoundType.Digit:
                    // for saying single digits
                    sound.say(_clipName);

                    break;
                    
                default:
                    Debug.Log("Incorrect SoundType in SoundAnimEvent: " + _description);
                    break;
            }
        }

        protected override void OnUpdate(ref float currentTime)
        {
            
        }

        protected override void OnStop()
        {
         //todo stop music   
        }

        string[] cList = new string[]
                {
                    "D6CF9A",
                    "A39E75",
                    "DDCE5D",
                    "AA9E47"
                };

        protected override string GetColor()
        {
            if (_clipName == null)
                return gray;
            return cList[_typeSelect];
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
        {
            base.DrawTimelineGui(rect, selected);

            GUI.Box(rect, "[" + (SoundType)_typeSelect + "] - " + _clipName , GuiStyle);
        }

        public override void DrawEditorGui()
        {
#if UNITY_EDITOR
            _typeSelect = EditorGUILayout.Popup("Type", _typeSelect, typeOptions);

            //_player = EditorGUILayout.ObjectField("Player", _player, typeof(AudioSource), true) as AudioSource;
            _clipName = EditorGUILayout.TextField("Clip name", _clipName);

            _startTime = EditorGUILayout.FloatField("Start time", _startTime);
            
#endif

        }
        
    }
}
