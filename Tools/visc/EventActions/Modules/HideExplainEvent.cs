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
    /// Hides Tony after an explanation
    /// </summary>
	public class HideExplainEvent : EventAction
	{
        [SerializeField]
        private TutorialController _tutorial;

		protected override void OnStart(float startTime)
        {
            _tutorial.HideExplain();
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

            string s = "Hide Tony";
            
            GUI.Box(rect, s, GuiStyle);
		}

		private int _selectedParam;
		private string[] _parameterNames;

		public override void DrawEditorGui()
		{
            _tutorial = EditorGUILayout.ObjectField("Tutorial", _tutorial, typeof(TutorialController), true) as TutorialController;
        }
#endif
	}
}