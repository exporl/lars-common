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
    /// Lets Tony explain something about the game to the player
    /// </summary>
	public class ExplainEvent : EventAction
	{
        [SerializeField]
        private TutorialController _tutorial;
        [SerializeField]
        private string _explanation = "";
        [SerializeField]
        private bool _playSound = true;
        [SerializeField]
        private bool _autoHide;

		protected override void OnStart(float startTime)
        { 
            string filtered = _explanation.Replace("%name%", Lars.UserProfileManager.instance.ActiveUser.name);
            _tutorial.Explain(filtered, _playSound, _autoHide);
        }

		protected override void OnStop() {  }


#if UNITY_EDITOR

        string[] cList = new string[]
                {
                    "00FFFF"
                };

        protected override string GetColor()
        {
            return "CCCCCC";
        }

        public override void DrawTimelineGui(Rect rect, bool selected)
		{
            base.DrawTimelineGui(rect, selected);

            string s = "Tony Explains";

            if (_explanation.Length > 0) s = "Tony: " + _explanation;

            GUI.Box(rect, s, GuiStyle);
		}

		private int _selectedParam;
		private string[] _parameterNames;

		public override void DrawEditorGui()
		{
            _tutorial = EditorGUILayout.ObjectField("Tutorial", _tutorial, typeof(TutorialController), true) as TutorialController;
            _playSound = EditorGUILayout.Toggle("Play sound", _playSound);
            _autoHide = EditorGUILayout.Toggle("Auto hide", _autoHide);
            _explanation = EditorGUILayout.TextField("Explanation", _explanation);
        }
#endif
	}
}