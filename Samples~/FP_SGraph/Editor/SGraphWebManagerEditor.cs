namespace FuzzPhyte.SGraph.Editor{

    using UnityEditor;
    using UnityEngine;
    using FuzzPhyte.SGraph.Samples;
   
    using SGUtility = FP_SGraphUtility<TransitionD, RequirementD>;
    [CustomEditor(typeof(SGraphWebManager))]
    public class SGraphWebManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SGraphWebManager manager = (SGraphWebManager)target;

            EditorGUILayout.Space();
            var goNodes = FindObjectsByType<NodeMBEx>(FindObjectsSortMode.InstanceID);
            //var goNodes = FindObjectsOfType(typeof(NodeMBEx));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scene Node Count:", goNodes.Length.ToString());
            EditorGUILayout.LabelField("Unity Nodes Count:", manager.unityNodes != null ? manager.unityNodes.Count.ToString() : "0");
            EditorGUILayout.LabelField("Data Nodes Count:", manager.DataNodes != null ? manager.DataNodes.Count.ToString() : "0");
            EditorGUILayout.EndVertical();
            //check how many UnityNodes are in the dictionary, and then check how many items are in my DataNodes List, if there is a mixmatch change the color of the LabelField
            var buttonColor = SGUtility.DefaultBoxColorText;
            if(manager.unityNodes != null && manager.unityNodes.Count != manager.DataNodes.Count){
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