using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
#endif


namespace Visc
{
	public class SetActiveAnimEvent : EventAction
	{
        [SerializeField]
        private bool _enabled = true;
        private bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                //if (value != _enabled) _changed = true;
                _enabled = value;
            }
        }

		protected override void OnStart(float startTime)
        {
            _actor.SetActive(_enabled);
        }
		protected override void OnStop() {  }



#if UNITY_EDITOR

        string[] cList = new string[]
                {
                    "CB4634",
                    "7FA28E"
                };

        protected override string GetColor()
        {
            if (_actor == null)
                return gray;
            return cList[Convert.ToInt32(_enabled)];
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
		{
            base.DrawTimelineGui(rect, selected);

            string s = "";
            if (_actor != null)
                s = _enabled ? "Enable " + _actor.name : "Disable " + _actor.name;
            else
                s = "Set on/off";

            GUI.Box(rect, s, GuiStyle);
		}

		private int _selectedParam;
		private string[] _parameterNames;

		protected override void OnEditorGui()
		{
            enabled = EditorGUILayout.Toggle("on/off",_enabled);
		}
#endif
	}
}