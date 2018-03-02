#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Visc
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Scenario))]
	public class ScenarioEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open scenario editor"))
				ScenarioEditorWindow.ShowWindow().SetScenario(target as Scenario);
		}
	}
#endif
}
