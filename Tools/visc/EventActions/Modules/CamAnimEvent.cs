using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
using Lars;

namespace Visc
{
    public class CamAnimEvent : EventAction
    {
        enum CamType { SetActive = 0, LookAt, SetTransformNow, TransitionTransform, TransitionCam, Shake, OrthoSize, Zoom, SetProjection };
        private string[] typeOptions = System.Enum.GetNames(typeof(CamType));
        private CamType _type = CamType.SetActive;

        [SerializeField]
        private Camera _cam;
        [SerializeField]
        private Camera _camTo;
        [SerializeField]
        private Transform _transformFrom;
        [SerializeField]
        private Transform _transformTo;

        //lookat
        [SerializeField]
        private bool _transitionLookAt;

        //shake
        [SerializeField]
        private bool _doShakeRot;
        [SerializeField]
        private bool _doShakePos;
        [SerializeField]
        private Vector3 _strength;
        [SerializeField]
        private int _vibrato;
        [SerializeField, Range(0, 150)]
        private float _rndm;

        //orthosize
        [SerializeField, Range(.05f, 20)]
        private float _orthoVal;

        //zoom
        [SerializeField]
        private float _zoomVal;
        [SerializeField]
        private int _zoomDirSelect;
        private ZoomDirection _zoomDir;
        private string[] zoomDirOptions = new string[] { "Out", "In" };
        enum ZoomDirection { Out = -1, In = 1 };

        //ortho
        [SerializeField]
        private bool _projectionType; //true = ortho, false = perspective

        private float _start;
        
        GameManager _gameManager;
        GameManager gameManager
        {
            get
            {
                if (_gameManager == null)
                    _gameManager = FindObjectOfType<GameManager>();
                return _gameManager;
            }
        }

        protected override void OnStart(float startTime)
        {
            _start = startTime;

            _type = ParseEnum<CamType>(typeOptions[_typeSelect]);

            if (_cam == null) return;

            // Do type-specific action
            switch (_type)
            {
                case CamType.SetActive:

                    if (_gameManager == null)
                    {
                        Camera current = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                        current.tag = "InactiveCamera";
                        current.enabled = false;
                        //current.gameObject.SetActive(false);

                        _cam.tag = "MainCamera";
                        _cam.enabled = true;
                        //_cam.gameObject.SetActive(true);
                    }
                    else
                    {
                        _gameManager.SetCam(_cam);
                    }

                    break;

                case CamType.LookAt:

                    if (_cam == null || _transformTo == null) return;

                    if(_transitionLookAt)
                    {
                        _cam.transform.DOLookAt(_transformTo.position, _duration).SetEase(easer);
                    }
                    else
                    {
                        _cam.transform.LookAt(_transformTo);
                    }

                    break;

                case CamType.SetTransformNow:

                    _cam.transform.position = _transformTo.position;
                    _cam.transform.rotation = _transformTo.rotation;

                    break;

                case CamType.TransitionTransform:

                    if (_cam != null && _transformTo != null)
                        TransitTransform(_transformTo, _transformFrom, easer);

                    break;

                case CamType.TransitionCam:

                    if (_cam == null || _camTo == null)
                        return;

                    if (_cam.orthographic != _camTo.orthographic)
                        Debug.Log("WARNING: TransitionCam has an orthographic & perspective cam as targets!");

                    TransitCam(_cam, _camTo, _duration, null, easer);

                    break;

                case CamType.Shake:

                    if (_doShakePos)
                        _cam.DOShakePosition(_duration, _strength, _vibrato, _rndm).SetEase(easer);
                    if (_doShakeRot)
                        _cam.DOShakeRotation(_duration, _strength, _vibrato, _rndm).SetEase(easer);

                    break;

                case CamType.OrthoSize:

                    _cam.DOOrthoSize(_orthoVal, _duration).SetEase(easer);

                    break;

                    /*
                case CamType.Zoom:

                    if (_cam == null || _transformTo == null) return;

                    //TODO calculations need work
                    
                    _zoomDir = ParseEnum<ZoomDirection>(zoomDirOptions[_zoomDirSelect]);
                    Vector3 camPos = _cam.transform.position;
                    Vector3 toPos = _transformTo.position;
                    float dist = Vector3.Distance(camPos,toPos);
                    Vector3 rotationDir = new Vector3(Mathf.Abs(camPos.x - toPos.x) / dist, Mathf.Abs(camPos.y - toPos.y) / dist, Mathf.Abs(camPos.z - toPos.z) / dist);
                    rotationDir = rotationDir * _zoomVal * (float)_zoomDir;
                    _cam.transform.LookAt(toPos);
                    _cam.transform.DOMove(rotationDir, _duration).SetEase(easer).SetRelative();

                    break;
                    */
                case CamType.SetProjection:

                    _cam.orthographic = _projectionType;

                    break;

                default:

                    Debug.Log("Invalid CamAnimEvent type");

                    break;
            }
        }

        protected override void OnUpdate(ref float currentTime)
        {

        }

        protected override void OnStop()
        {

        }

        /// <summary>
        /// Transits the active camera (_cam)
        /// </summary>
        /// <param name="to"></param>
        ///  <param name="from"> optional</param>
        /// <param name="easing"></param>
        private void TransitTransform(Transform to, Transform from = null, Ease easing = Ease.OutSine)
        {
            if (to == from || to == null) return;

            if (from != null)
            {
                _cam.transform.position = from.position;
                _cam.transform.rotation = from.rotation;
            }

            Transit(_cam.gameObject, to, _duration, easing, null);

        }

        private void TransitCam(Camera fromCam, Camera toCam, float duration, Action callback = null, Ease easing = Ease.OutSine)
        {
            if (fromCam == toCam) return;

            var origPos = fromCam.transform.position;
            var origRot = fromCam.transform.rotation;

            fromCam.enabled = true;
            //fromCam.gameObject.SetActive(true);

            Transit(fromCam.gameObject, toCam.transform, _duration, easing, () =>
            {
                toCam.enabled = true;
                toCam.tag = "MainCamera";
                //toCam.gameObject.SetActive(true);

                fromCam.enabled = false;
                fromCam.tag = "InactiveCamera";
                //fromCam.gameObject.SetActive(false);

                fromCam.transform.position = origPos;
                fromCam.transform.rotation = origRot;
            });

            //fromCam.transform.DOMove(toCam.transform.position, duration).SetEase(easing);
            //fromCam.transform.DORotate(toCam.transform.rotation.eulerAngles, duration)
            //                 .SetEase(easing)
            //                 .OnComplete(() =>
            //                 {
            //                     toCam.enabled = true;
            //                     toCam.gameObject.SetActive(true);

            //                     fromCam.enabled = false;
            //                     fromCam.gameObject.SetActive(false);

            //                     fromCam.transform.position = origPos;
            //                     fromCam.transform.rotation = origRot;
            //                 });
            /*
            () =>
            {
                toCam.enabled = true;
                toCam.gameObject.SetActive(true);

                fromCam.enabled = false;
                fromCam.gameObject.SetActive(false);

                fromCam.transform.position = origPos;
                fromCam.transform.rotation = origRot;
            }
            */
        }

        private void Transit(GameObject obj, Transform to, float duration, Ease easing = Ease.OutSine, Action callback = null)
        {
            obj.transform.DOMove(to.position, duration)
                             .SetEase(easing);
            obj.transform.DORotate(to.rotation.eulerAngles, duration)
                             .SetEase(easing)
                             .OnComplete(() => { if (callback != null) callback(); });
        }

        // GUI

        string[] cList = new string[]
            {
                "ABA8D8",
                "6F6C8B",
                "797698",
                "5A5972",
                "6F67D8"
            };


        protected override string GetColor()
        {
            if (_cam == null)
                return "CCCCCC";

            return cList[_typeSelect % cList.Length];
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
        {
            base.DrawTimelineGui(rect, selected);

            if (_cam == null)
                GuiStyle.normal.background = GrayTex;

            GUI.Box(rect, "[ Cam | " + (CamType)_typeSelect + "]" + _description, GuiStyle);
        }

        public override void DrawEditorGui()
        {
#if UNITY_EDITOR
            typeSelect = EditorGUILayout.Popup("Type", typeSelect, typeOptions);

            _cam = EditorGUILayout.ObjectField("Camera from", _cam, typeof(Camera), true) as Camera;
            if ((CamType)_typeSelect == CamType.TransitionCam)
                _camTo = EditorGUILayout.ObjectField("Camera to", _camTo, typeof(Camera), true) as Camera;

            _startTime = EditorGUILayout.FloatField("Start time", _startTime);

            if ((CamType)_typeSelect == CamType.TransitionCam || (CamType)_typeSelect == CamType.TransitionTransform || (CamType)_typeSelect == CamType.Shake
                || (CamType)_typeSelect == CamType.OrthoSize || (CamType)_typeSelect == CamType.Zoom)
            {
                _duration = EditorGUILayout.FloatField("Duration", _duration);
            }

            if ((CamType)_typeSelect == CamType.TransitionTransform)
                _transformFrom = EditorGUILayout.ObjectField("Transform from (optional)", _transformFrom, typeof(Transform), true) as Transform;

            if ((CamType)_typeSelect == CamType.SetTransformNow || (CamType)_typeSelect == CamType.TransitionTransform
                || (CamType)_typeSelect == CamType.LookAt)
            {
                _transformTo = EditorGUILayout.ObjectField("Transform to", _transformTo, typeof(Transform), true) as Transform;
            }

            if((CamType)_typeSelect == CamType.LookAt)
            {
                _transitionLookAt = EditorGUILayout.Toggle("Transition look at?", _transitionLookAt);
            }

            if ((CamType)_typeSelect == CamType.OrthoSize || (CamType)_typeSelect == CamType.Zoom)
            {
                _zoomDirSelect = EditorGUILayout.Popup("Zoom direction", _zoomDirSelect, zoomDirOptions);
            }

            if ((CamType)_typeSelect == CamType.OrthoSize)
                _orthoVal = EditorGUILayout.FloatField("Ortho zoom value", _orthoVal);
            if ((CamType)_typeSelect == CamType.Zoom)
                _zoomVal = EditorGUILayout.FloatField("3D Zoom value", _zoomVal);

            if ((CamType)_typeSelect == CamType.Shake)
            {
                _doShakePos = EditorGUILayout.Toggle("Shake position", _doShakePos);
                _doShakeRot = EditorGUILayout.Toggle("Shake rotation", _doShakeRot);

                _strength = EditorGUILayout.Vector3Field("Shake strength", _strength);
                _vibrato = EditorGUILayout.IntField("Vibrato", _vibrato);
                _rndm = EditorGUILayout.FloatField("Randomness", _rndm);
            }

            if ((CamType)_typeSelect != CamType.SetActive && (CamType)_typeSelect != CamType.SetTransformNow 
                && (CamType)_typeSelect != CamType.SetProjection && _transitionLookAt)
                _easingMode = EditorGUILayout.Popup("Easingmode", _easingMode, easingOptions);

            if ((CamType)_typeSelect == CamType.SetProjection)
                _projectionType = EditorGUILayout.Toggle("Orthographic?", _projectionType);

            _description = EditorGUILayout.TextField("Description", _description);
#endif

        }

    }
}
