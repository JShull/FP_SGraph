namespace FuzzPhyte.SGraph.Editor{

    using UnityEditor;
    using UnityEngine;
    using FuzzPhyte.SGraph.Samples;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FuzzPhyte.SGraph.Editor;
    using FuzzPhyte.Utility.Editor;
    using SGUtility = FP_SGraphUtility<TransitionD, RequirementD>;
    [CustomEditor(typeof(SGraphWebManager))]
    public class SGraphWebManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SGraphWebManager manager = (SGraphWebManager)target;

            EditorGUILayout.Space();
            var goNodes = FindObjectsOfType(typeof(NodeMBEx));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scene Node Count:", goNodes.Length.ToString());
            EditorGUILayout.LabelField("Unity Nodes Count:", manager.UnityNodes != null ? manager.UnityNodes.Count.ToString() : "0");
            EditorGUILayout.LabelField("Data Nodes Count:", manager.DataNodes != null ? manager.DataNodes.Count.ToString() : "0");
            EditorGUILayout.EndVertical();
            //check how many UnityNodes are in the dictionary, and then check how many items are in my DataNodes List, if there is a mixmatch change the color of the LabelField
            var buttonColor = SGUtility.DefaultBoxColorText;
            if(manager.UnityNodes != null && manager.UnityNodes.Count != manager.DataNodes.Count){
                buttonColor = Color.red;
            }
            GUI.color = buttonColor;
            if (GUILayout.Button("Update Nodes"))
            {
                UpdateNodes(manager);
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(manager);
            }
        }

        private void UpdateNodes(SGraphWebManager manager)
        {
            manager.UpdateAllNodes();
            
            // Ensure changes are saved
            EditorUtility.SetDirty(manager);
        }
        
    }
}