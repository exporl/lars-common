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
	public class WaitForTapEvent : EventAction
	{
        [SerializeField]
        private Scenario _scenario;
        [SerializeField]
        private TutorialController _tutorial;
        [SerializeField]
        private bool _showIcon;
        [SerializeField]
        private bool _tapAnywhere;
        [SerializeField]
        private bool _pauseGame;

        protected override void OnStart(float startTime)
        {
            _scenario.Pause();
            if (_pauseGame)
                _tutorial.gameManager.PauseGame(true);

            _tutorial.StartWait(_showIcon, _tapAnywhere, 
            () => 
            {
                _scenario.Resume();
                if (_pauseGame)
                    _tutorial.gameManager.ResumeGame(true);
            });
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

            string s = "Wait for Tap";

            GUI.Box(rect, s, GuiStyle);
		}

		private int _selectedParam;
		private string[] _parameterNames;

		public override void DrawEditorGui()
		{
            //enabled = EditorGUILayout.Toggle("on/off",_enabled);
            _scenario = EditorGUILayout.ObjectField("Scenario", _scenario, typeof(Scenario), true) as Scenario;
            _tutorial = EditorGUILayout.ObjectField("Tutorial", _tutorial, typeof(TutorialController), true) as TutorialController;
            _showIcon = EditorGUILayout.Toggle("Show icon", _showIcon);
            _tapAnywhere = EditorGUILayout.Toggle("Tap anywhere", _tapAnywhere);
            _pauseGame = EditorGUILayout.Toggle("Pause game", _pauseGame);
        }
#endif
	}
}