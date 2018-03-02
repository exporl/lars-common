using System.Collections;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
#endif
using Lars;


namespace Visc
{
    /// <summary>
    /// Pauses the scenario it's in 
    /// </summary>
	public class CallbackEvent : EventAction
	{
        [SerializeField]
        private TutorialController _tutorial;
        [SerializeField]
        private string _callbackName = "";

        protected override void OnStart(float startTime)
        {
            _tutorial.doCallback(_callbackName);
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
            return "CCCCCC";
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
		{
            base.DrawTimelineGui(rect, selected);

            string s = _callbackName.Length > 0 ? ("Do "+_callbackName) : "Do Callback";

            GUI.Box(rect, s, GuiStyle);
		}

		private int _selectedParam;
		private string[] _parameterNames;

		public override void DrawEditorGui()
		{
            _tutorial = EditorGUILayout.ObjectField("Tutorial", _tutorial, typeof(TutorialController), true) as TutorialController;
            _callbackName = EditorGUILayout.TextField("Callback name", _callbackName);
        }
#endif
	}
}