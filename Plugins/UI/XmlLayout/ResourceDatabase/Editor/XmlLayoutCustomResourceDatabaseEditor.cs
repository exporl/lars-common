using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace UI.Xml
{
    [CustomEditor(typeof(XmlLayoutCustomResourceDatabase))]
    public class XmlLayoutCustomResourceDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            var instance = (XmlLayoutCustomResourceDatabase)target;

            if (GUILayout.Button("Clear Entries"))
            {
                instance.entries.Clear();
                instance.LoadFolders();
            }

            if (GUILayout.Button("Clear Folders"))
            {
                instance.folders.Clear();
            }
        }
    }
}
