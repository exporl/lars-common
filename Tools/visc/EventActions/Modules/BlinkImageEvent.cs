using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
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
	public class BlinkImageEvent : EventAction
	{
        [SerializeField]
        private TutorialController _tutorial;
        [SerializeField]
        private Image _img;

		protected override void OnStart(float startTime)
        {
            _tutorial.BlinkImage(_img);
        }

		protected override void OnStop()
        {
            _tutorial.StopBlinkImage(_img);
        }


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

            string s = "Blink Arrow";

            GUI.Box(rect, s, GuiStyle);
		}

		private int _selectedParam;
		private string[] _parameterNames;

		public override void DrawEditorGui()
		{
            _tutorial = EditorGUILayout.ObjectField("Tutorial", _tutorial, typeof(TutorialController), true) as TutorialController;
            _img = EditorGUILayout.ObjectField("Image", _img, typeof(Image), true) as Image;
        }
#endif
	}
}