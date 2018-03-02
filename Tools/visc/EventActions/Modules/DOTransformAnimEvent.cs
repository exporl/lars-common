using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
//using Lars;

namespace Visc
{
    public class DOTransformAnimEvent : EventAction
    {
        enum DOType { Move = 0, Rotate, Scale, Path };
        private string[] typeOptions = System.Enum.GetNames(typeof(DOType));
        private DOType _type = DOType.Move;

        [SerializeField]
        private Transform _transformFrom;
        [SerializeField]
        private Transform _transformTo;
        [SerializeField]
        private Transform[] _transformToPath; //DOPath

        private float _start;

        [SerializeField]
        private bool _blendable; //DOBlendable...
        
        [SerializeField]
        private int _loops = 0;
        [SerializeField]
        private int _loopType;
        [SerializeField]
        private bool _relative;
        [SerializeField]
        private int _rotaMode;
        
        string[] loopOptions = System.Enum.GetNames(typeof(LoopType));
        string[] rotaOptions = System.Enum.GetNames(typeof(RotateMode));

        private Tweener tween;
        private LoopType loopType;
        private RotateMode rotaMode;

        protected override void OnStart(float startTime)
        {
            _start = startTime;

            _type = ParseEnum<DOType>(typeOptions[_typeSelect]);

            if (_actor == null || _transformTo == null) return;
            //if (_transformFrom == null || _transformTo == null) return;

            if (_transformFrom == null)
                _transformFrom = _actor.transform;

            easer = ParseEnum<Ease>(easingOptions[_easingMode]);
            loopType = ParseEnum<LoopType>(loopOptions[_loopType]);

            // SET TWEEN
            switch (_type)
            {
                case DOType.Move:

                    _actor.transform.position = _transformFrom.position;
                    if(_blendable)
                        tween = _actor.transform.DOBlendableMoveBy(_transformTo.position, _duration);
                    else
                        tween = _actor.transform.DOMove(_transformTo.position, _duration);

                    break;

                case DOType.Rotate:

                    _actor.transform.rotation = _transformFrom.rotation;
                    if (_blendable)
                        tween = _actor.transform.DORotate(_transformTo.rotation.eulerAngles, _duration, rotaMode);
                    else
                        tween = _actor.transform.DORotate(_transformTo.rotation.eulerAngles, _duration, rotaMode);

                    break;

                case DOType.Scale:

                    _actor.transform.localScale = _transformFrom.localScale;
                    if (_blendable)
                        tween = _actor.transform.DOBlendableScaleBy(_transformTo.localScale, _duration);
                    else
                        tween = _actor.transform.DOScale(_transformTo.localScale, _duration);

                    break;

                case DOType.Path:

                    break;
                default:
                    Debug.Log("Incorrect DOType in DOTransformAnimEvent: " + _description);
                    break;
            }

            if (_easingMode > -1)
                tween.SetEase(easer);

            if (_loops != 1)
                tween.SetLoops(_loops, loopType); //maybe assign? 

            if (_relative)
                tween.SetRelative();
        }

        protected override void OnUpdate(ref float currentTime)
        {
            /*
            if (_actor == null) return;
            var coveredDistance = (currentTime - _start) * _speed;
            var journeyFraction = coveredDistance / _journeyLength;
            _actor.transform.position = Vector3.Lerp(_from, _to, journeyFraction);
            */
        }

        protected override void OnStop()
        {
            //if (_actor == null) return;
            //_actor.transform.position = _to;
        }

        string[] cList = new string[]
                {
                    "6BBED3",
                    "4A8494",
                    "37626D",
                };

        protected override string GetColor()
        {
            if (_actor == null)
                return gray;
            return cList[_typeSelect];
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
        {
            base.DrawTimelineGui(rect, selected);

            GUI.Box(rect, "[" + (DOType)_typeSelect + "]" + _description, GuiStyle);
        }

        public override void DrawEditorGui()
        {
#if UNITY_EDITOR
            typeSelect = EditorGUILayout.Popup("DO Type", typeSelect, typeOptions);

            _startTime = EditorGUILayout.FloatField("Start time", _startTime);
            _duration = EditorGUILayout.FloatField("Duration", _duration);
            Actor = EditorGUILayout.ObjectField("Actor", Actor, typeof(GameObject), true) as GameObject;

            _transformFrom = EditorGUILayout.ObjectField("From", _transformFrom, typeof(Transform), true) as Transform;

            if ((DOType)_typeSelect != DOType.Path)
            {
                _transformTo = EditorGUILayout.ObjectField("To", _transformTo, typeof(Transform), true) as Transform;
            }
            else
            {
                SerializedObject so = new SerializedObject(this);
                SerializedProperty pathList = so.FindProperty("_transformToPath");

                EditorGUILayout.PropertyField(pathList, true); // True means show children
                so.ApplyModifiedProperties();

            }

            _blendable = EditorGUILayout.Toggle("Blendable", _blendable);

            _easingMode = EditorGUILayout.Popup("Easingmode", _easingMode, easingOptions);
            _loops = EditorGUILayout.IntField("Loops", _loops);
            _loopType = EditorGUILayout.Popup("Loopmode", _loopType, loopOptions);

            if ((DOType)_typeSelect == DOType.Rotate)
            {
                _rotaMode = EditorGUILayout.Popup("Rotation mode", _rotaMode, rotaOptions);
            }

            _relative = EditorGUILayout.Toggle("Relative", _relative);

            _description = EditorGUILayout.TextField("Description", _description);
#endif

        }

        class PathPoints : MonoBehaviour
        {
            public Transform[] points;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(PathPoints))]
        public class PathEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                var controller = target as PathPoints;
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.fieldWidth = 0;
                SerializedProperty tps = serializedObject.FindProperty("points");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(tps, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.fieldWidth = 0;
                // ...
            }
        }
#endif

    }
}
