#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace Visc
{
	public class EventActionEditor : EditorWindow
	{	
		private EventAction _currentAction;

        /*
		public static void ShowWindow()
		{
			GetWindow(typeof(EventActionEditor));
		}
        */
        [MenuItem("Window/Scenario action editor %#L")]
        public static EventActionEditor ShowWindow()
        {
            var window = GetWindow(typeof(EventActionEditor), false, "Scenario Action Window") as EventActionEditor;
            //_myControlId = window.GetInstanceID();
            return window;
        }

        public void SetCurrentAction(EventAction action)
		{
            if (action == null) return;
			_currentAction = action;
		}

		private void OnGUI()
		{
			if (_currentAction != null)
			{
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
				{
					if(!Application.isPlaying)
						EditorSceneManager.MarkAllScenesDirty();
                    //Close();
                    Repaint();
				}
                
                _currentAction.DrawEditorGui();
                
                GUIStyle savBt = new GUIStyle(GUI.skin.button)
                {
                    //normal = { background = _currentAction._changed ? EventAction.MakeTexSq("ff0000") : EventAction.MakeTexSq("666666") },
                    name = "Save"
                };

                if (!Application.isPlaying && GUILayout.Button("Save", savBt))
                {
                    EditorSceneManager.MarkAllScenesDirty();
                    //_currentAction._changed = false;
                }
			}
			else
			{
				GUILayout.Label("Select action");
			}
		}
    }
}
#endif