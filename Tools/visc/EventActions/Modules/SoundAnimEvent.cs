using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;

namespace Visc
{
    public class SoundAnimEvent : EventAction
    {
        enum SoundType { Fade = 0, SetVol, PlayClip, PlayMusic };
        private string[] typeOptions = System.Enum.GetNames(typeof(SoundType));
        private SoundType _type;// = SoundType.Fade;

        [SerializeField]
        private AudioSource _player;

        private float _start;

        [SerializeField]
        private float _fadeFrom = -1;
        [SerializeField]
        private float _fadeTo;
        [SerializeField]
        private AudioClip _clip;
        [SerializeField]
        private float _vol = -1;

        private float _decibelValue;

        protected override void OnStart(float startTime)
        {
            _start = startTime;

            _type = ParseEnum<SoundType>(typeOptions[_typeSelect]);

            if (_player == null) return;
            
            // Do type-specific action
            switch (_type)
            {
                case SoundType.Fade:

                    if (_fadeFrom != -1)
                        _player.volume  = _fadeFrom; // DO DECIBEL CALC TODO
                    _player.DOFade(_fadeTo, _duration).SetEase(Ease.Linear);// DO DECIBEL CALC TODO

                    break;

                case SoundType.SetVol:

                    if (_vol != 0)
                        _player.volume = _vol; // DO DECIBEL CALC TODO

                    break;

                case SoundType.PlayClip:

                    if (_clip == null)
                        break;
                    if(_vol != 0)
                        _player.PlayOneShot(_clip,_vol);
                    else
                        _player.PlayOneShot(_clip);

                    break;

                case SoundType.PlayMusic:

                    if (_clip == null)
                        break;

                    _player.clip = _clip;
                    _player.Play();

                    if (_vol != -1)
                        _player.volume = _vol; //TODO decibel

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
            if (_player == null)
                return gray;
            return cList[_typeSelect];
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
        {
            base.DrawTimelineGui(rect, selected);

            GUI.Box(rect, "[" + (SoundType)_typeSelect + "]" + _description , GuiStyle);
        }

        public override void DrawEditorGui()
        {
#if UNITY_EDITOR
            _typeSelect = EditorGUILayout.Popup("Type", _typeSelect, typeOptions);

            _player = EditorGUILayout.ObjectField("Player", _player, typeof(AudioSource), true) as AudioSource;

            _startTime = EditorGUILayout.FloatField("Start time", _startTime);
            if((SoundType)_typeSelect == SoundType.Fade || (SoundType)_typeSelect == SoundType.PlayMusic)
                _duration = EditorGUILayout.FloatField("Duration", _duration);
            
            if ((SoundType)_typeSelect == SoundType.PlayMusic || (SoundType)_typeSelect == SoundType.PlayClip)
            {
                _clip = EditorGUILayout.ObjectField("Clip", _clip, typeof(AudioClip), true) as AudioClip;
                _vol = EditorGUILayout.FloatField("Volume", _vol);
            }

            if ((SoundType)_typeSelect == SoundType.Fade)
            {
                _fadeFrom = EditorGUILayout.FloatField("Fade from", _fadeFrom);
                _fadeTo = EditorGUILayout.FloatField("Fade to", _fadeTo);
            }

            if ((SoundType)_typeSelect == SoundType.SetVol)
            {
                _vol = EditorGUILayout.FloatField("Volume", _vol);
            }

            _description = EditorGUILayout.TextField("Description", _description);
#endif

        }
        
    }
}
