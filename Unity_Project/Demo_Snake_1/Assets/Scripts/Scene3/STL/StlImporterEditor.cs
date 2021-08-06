using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;

namespace Parabox.Stl.Editor
{
    [CustomEditor(typeof(StlImporter))]
    class StlImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var m_CoordinateSpace = serializedObject.FindProperty("m_CoordinateSpace");
            // todo
            // var m_UpAxis = serializedObject.FindProperty("m_UpAxis");
            var m_Smooth = serializedObject.FindProperty("m_Smooth");

            // PropertyField isn't displaying as a enum here, not sure why.
            EditorGUILayout.PropertyField(m_CoordinateSpace);
            EditorGUILayout.PropertyField(m_Smooth);

            base.ApplyRevertGUI();
        }
    }
}
