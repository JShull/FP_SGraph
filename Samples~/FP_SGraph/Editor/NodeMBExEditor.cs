namespace FuzzPhyte.SGraph.Editor
{
    using UnityEngine;
    using UnityEditor;
    using FuzzPhyte.SGraph;
    using FuzzPhyte.Utility.Editor;
    using FuzzPhyte.SGraph.Samples;
    using System.IO;
    using System;
    using UnityEngine.SceneManagement;
    using FuzzPhyte.Utility.Meta;
    using Unity.Serialization.Json;
    using System.Text;
    using SGUtility = FP_SGraphUtility<TransitionD, RequirementD>;
    [CustomEditor(typeof(NodeMBEx))]
    public class NodeMBExEditor : Editor
    {
        private bool showConnections = true;
        private bool showRequirements = true;
        
        private string ShowConnectionsKey = "SGraphGONodeEditor_ShowConnections";
        private string ShowRequirementsKey = "SGraphGONodeEditor_ShowRequirements";
        private void OnEnable()
        {
            SetKeys();
            showConnections = EditorPrefs.GetBool(ShowConnectionsKey);
            showRequirements = EditorPrefs.GetBool(ShowRequirementsKey);
        }
        private void SetKeys()
        {
            NodeMBEx nodeMonoScript = (NodeMBEx)target;
            ShowConnectionsKey = nodeMonoScript.GetInstanceID() + "_ShowConnections";
            ShowRequirementsKey = nodeMonoScript.GetInstanceID() + "_ShowRequirements";
        }
        private void OnDisable()
        {
            EditorPrefs.SetBool(ShowConnectionsKey, showConnections);
            EditorPrefs.SetBool(ShowRequirementsKey, showRequirements);
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NodeMBEx nodeMonoScript = (NodeMBEx)target;
            // Connection List
            /*
            showConnections = EditorGUILayout.Foldout(showConnections, "Connections");
            if (showConnections)
            {
                ShowConnectionsList(nodeMonoScript, SGUtility.requiredStyleTag);
            }
            EditorPrefs.SetBool(ShowConnectionsKey, showConnections);
            // Requirement List
            showRequirements = EditorGUILayout.Foldout(showRequirements, "Requirements");
            if (showRequirements)
            {
                ShowRequirementsList(nodeMonoScript, SGUtility.requiredStyleTag, SGUtility.requiredStyleName);
            }
            EditorPrefs.SetBool(ShowRequirementsKey, showRequirements);
            */
            GUIStyle copyScriptableObjectDataButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = SGUtility.DefaultEditorColor },
                hover = { textColor = SGUtility.DefaultEditorHoverColor }
            };
            
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box){
                margin = new RectOffset(15, 5, 5, 5),
                normal = {textColor = SGUtility.DefaultBoxColorText},
                hover = {textColor = SGUtility.DefaultBoxColorTextHover},
                border = new RectOffset(20, 7, 7, 7),
                focused = {textColor = SGUtility.DefaultBoxColorTextFocused},
            };
            
            EditorGUILayout.Space(25);
            FP_Utility_Editor.DrawUILine(SGUtility.LineBreakWarningColor, 5, 0);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(boxStyle);

            // Add a button to the inspector of NodeMBEx objects
            if (GUILayout.Button("Generate Events From Data",copyScriptableObjectDataButtonStyle))
            {
                GenerateEventsByType(nodeMonoScript);
            }
            EditorGUILayout.Space(20);
            /*
            if (GUILayout.Button("Build Copied Data", copyScriptableObjectDataButtonStyle))
            {
                var returnData = WriteRuntimeDataObjectWithUnityEvents(nodeMonoScript);
                if (returnData.Item1)
                {
                    WriteJSONFile(nodeMonoScript, returnData.Item2);
                }
                
            }
           
            EditorGUILayout.Space(10);
             */
            /*
            if(GUILayout.Button("Restore Data From JSON", copyScriptableObjectDataButtonStyle))
            {
                var dataInfo = ReadJSONFromLocalSelectedFileMenuUnity(nodeMonoScript.JSONData,nodeMonoScript);
                if (dataInfo.Item1)
                {
                    Debug.Log($"Try restoring the data now to our EventList");
                    nodeMonoScript.ReplaceEventsByTypeWithRuntimeData();
                }
            }
            */
            EditorGUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(nodeMonoScript);
            }
        }
        private void ShowConnectionsList(NodeMBEx nodeMonoScript,GUILayoutOption[] reqStyleTag)
        {
            /*
            EditorGUILayout.LabelField("Connections", EditorStyles.boldLabel);
            FP_Utility_Editor.DrawUILine(SGUtility.DefaultEditorColor, 5, 5);
            //try replacing with node sharp
            //nodeMonoScript.NodeSharp.Connections 
            if (nodeMonoScript.RuntimeNode != null && nodeMonoScript.RuntimeNode.Connections != null)
            {
                for (int i = 0; i < nodeMonoScript.RuntimeNode.Connections.Count; i++)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField($"Connection {i + 1}", reqStyleTag);
                    nodeMonoScript.RuntimeNode.Connections[i] = (NodeSOB<TransitionD, RequirementD>)EditorGUILayout.ObjectField(
                        nodeMonoScript.RuntimeNode.Connections[i],
                        typeof(NodeSOB<TransitionD, RequirementD>),
                        false,
                        reqStyleTag
                    );
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    if (GUILayout.Button("X",SGUtility.closeButtonStyle))
                    {
                        nodeMonoScript.RuntimeNode.Connections.RemoveAt(i);
                        i--; // Adjust index after removal
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                   
                }
            }
            FP_Utility_Editor.DrawUILine(SGUtility.DefaultEditorColor, 5, 5);
            if (GUILayout.Button("Add Connection"))
            {
                nodeMonoScript.RuntimeNode.Connections.Add(null);
            }
            */
        }
        
        private void ShowRequirementsList(NodeMBEx nodeMonoScript,GUILayoutOption[] reqStyleTag,GUILayoutOption[] reqStyleName)
        {
            /*
            EditorGUILayout.LabelField("Requirements", EditorStyles.boldLabel);
            FP_Utility_Editor.DrawUILine(SGUtility.DefaultEditorColor, 5, 5);
            if (nodeMonoScript.RuntimeNode != null && nodeMonoScript.RuntimeNode.Requirements != null)
            {
                for (int i = 0; i < nodeMonoScript.RuntimeNode.Requirements.Count; i++)
                {
                    var requirement = nodeMonoScript.RuntimeNode.Requirements[i];

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField($"Requirement {i + 1}", reqStyleTag);
                    requirement.RequirementTag = (FP_Tag)EditorGUILayout.ObjectField(
                        requirement.RequirementTag,
                        typeof(FP_Tag),
                        false,
                        reqStyleTag
                    );
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Name", reqStyleName);
                    requirement.RequirementName = EditorGUILayout.TextField(
                        requirement.RequirementName,
                        reqStyleName
                    );
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Met", GUILayout.Width(25));
                    requirement.RequirementMet = EditorGUILayout.Toggle(
                        requirement.RequirementMet,
                        GUILayout.Width(25)
                    );
                    EditorGUILayout.EndVertical();
                    nodeMonoScript.RuntimeNode.Requirements[i] = requirement;
                    if (GUILayout.Button("X", SGUtility.closeButtonStyle))
                    {
                        nodeMonoScript.RuntimeNode.Requirements.RemoveAt(i);
                        i--; // Adjust index after removal
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    
                }
                FP_Utility_Editor.DrawUILine(SGUtility.DefaultEditorColor, 5, 5);
            }
            if (GUILayout.Button("Add Requirement"))
            {
                nodeMonoScript.RuntimeNode.Requirements.Add(new RequirementD());
            }
            */
        } 
          
        private void GenerateEventsByType(NodeMBEx nodeMonoScript)
        {

            //nodeMonoScript.RuntimeNode = nodeMonoScript.BuildRuntimeNode();
            nodeMonoScript.UpdateUnityFromTemplateData();
            EditorUtility.SetDirty(nodeMonoScript);
            if (nodeMonoScript.NodeDataTemplate != null)
            {
                nodeMonoScript.EventsByType.Clear(); // Clear current list to repopulate
                // Assuming StateTransitions is a List<T> in your SOSNodeData<T, A>
                foreach (var transition in nodeMonoScript.NodeDataTemplate.StateTransitions)
                {
                    nodeMonoScript.EventsByType.Add(transition);
                }
                nodeMonoScript.ConnectedNodes.Clear();
                var foundObjects = GameObject.FindObjectsOfType<NodeMBEx>();
                for (int i=0;i<nodeMonoScript.NodeDataTemplate.Connections.Count; i++)
                {
                    var curDataObject = nodeMonoScript.NodeDataTemplate.Connections[i];
                    //look for gameobjects with NodeMBEx Components
                    for(int j=0;j<foundObjects.Length;j++)
                    {
                        var externalGO = foundObjects[j];
                        if(externalGO.NodeDataTemplate != null)
                        {
                            if(externalGO.NodeDataTemplate == curDataObject)
                            {
                                nodeMonoScript.ConnectedNodes.Add(externalGO);
                                break;
                            }
                        }
                    }

                    //find gameobjects in the scene that match the connections by ID
                }
                // Mark the object as having been modified, so Unity knows to save the changes
                EditorUtility.SetDirty(nodeMonoScript);
                // Optionally, you could also call nodeMonoScript.BuildRuntimeNode() here if you want
                // to automatically rebuild the runtime node whenever the list is generated.
            }
            
        }
        private (bool,string) WriteRuntimeDataObjectWithUnityEvents(NodeMBEx nodeMonoScript)
        {
            /*
            //generate sample data folder
            //FP_Utility_Editor.CreatePackageSampleFolder(FP_SGraphUtility<SGraphTransitionData, string>.PRODUCT_NAME_UNITY, FP_SGraphUtility<SGraphTransitionData, string>.BASEVERSION);
            if(nodeMonoScript.NodeDataTemplate != null)
            {
                nodeMonoScript.RuntimeNode = nodeMonoScript.BuildRuntimeNode();
                //save this scriptableobject to a file and update asset database
                EditorUtility.SetDirty(nodeMonoScript);
                var localSamplesFolder = SGUtility.ReturnInstallPath();

                (bool, string) dataPath = (false, "");
                try
                {
                    var potentialFolder = Path.Combine(Application.dataPath, localSamplesFolder);
                    var fileIO = File.Exists(potentialFolder);
                    Debug.Log($"Local Samples Folder: {potentialFolder}, exist? {fileIO}");
                    //check if a folder exists for the data
                    if (!fileIO)
                    {
                        Directory.CreateDirectory(potentialFolder);
                    }

                    if (File.Exists(Path.Combine(potentialFolder, SGUtility.SAMPLELOCALFOLDER)))
                    {
                        Debug.Log($"Local Samples Folder: {SGUtility.SAMPLELOCALFOLDER} exists!");
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.Combine(potentialFolder, SGUtility.SAMPLELOCALFOLDER));
                    }
                    Debug.Log($"Before Assets: {localSamplesFolder}");
                    localSamplesFolder = Path.Combine("Assets", localSamplesFolder);
                    Debug.Log($"After Assets: {localSamplesFolder}");
                    AssetDatabase.Refresh();
                    dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(localSamplesFolder, SGUtility.SAMPLELOCALFOLDER);
                    if (!dataPath.Item1)
                    {
                        Debug.LogError($"Error on Path Creation, check the logs! Path at: {dataPath.Item2}");
                        return dataPath;
                    }
                    var returnPath = SGUtility.WriteSOSNodeData(nodeMonoScript.RuntimeNode, dataPath.Item2, "SOSNodeData.asset");
                    Debug.Log($"Asset saved at {returnPath}");
                    AssetDatabase.SaveAssets();
                    //write JSON
                    return dataPath;
                }
                catch(Exception e)
                {
                    Debug.Log(e.ToString());
                    return (false, e.Message);
                }
            }
            */
            return (false, "NULL");
        }
        private void WriteJSONFile(NodeMBEx nodeMonoScript,string fullPath)
        {
            /*
            if (nodeMonoScript.NodeDataTemplate != null)
            {
                nodeMonoScript.RuntimeNode = nodeMonoScript.BuildRuntimeNode();
                //save this scriptableobject to a file and update asset database
                EditorUtility.SetDirty(nodeMonoScript);
            }
            
            //var TheRecorderSettingsJSON = JsonUtility.ToJson(nodeMonoScript.RuntimeNode);
            var TheRecorderSettingsJSON = JsonSerialization.ToJson(nodeMonoScript.RuntimeNode);
            var jsonAsset = TheRecorderSettingsJSON;
            //var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
            //string currentSceneName = SceneManager.GetActiveScene().name;
            string backUpRootFile = "/FPBackup_SGraph_" + System.DateTime.Now.ToString("MM_dd_yy_HHmm_s");

            string backupFile = backUpRootFile + ".json";

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(fullPath + backupFile);
            Debug.Log($"Asset Backup Path: {assetPath}");
            //FileStream file = File.Create(assetPath);
            //create the asset
            try
            {
                // Create a FileStream to write to the file, it will create the file if it does not exist, or overwrite it if it does.
                using (FileStream fileStream = new FileStream(assetPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    // Create a StreamWriter to write text to the file, using UTF8 encoding
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        // Write text to the file
                        writer.Write(jsonAsset);
                    } // StreamWriter is automatically flushed and closed here
                } // FileStream is automatically closed here

                Debug.Log($"Backup file generated at {assetPath} lets try generating and saving the file'");

                //
            }
            catch (Exception ex)
            {
                // If an error occurs, display the message
                Debug.LogError("An error occurred while writing the backup file: " + ex.Message);
            }
            */
        }
        private (bool, string) ReadJSONFromLocalSelectedFileMenuUnity(TextAsset data, NodeMBEx nodeMonoScript)
        {
            /*
            if (data != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(data.GetInstanceID());
                try
                {
                    string content;
                    using (var fileStream = new FileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                    //SGraphGONode<SGraphTransitionData, string>
                    var returnData = JsonSerialization.FromJson<NodeSOB<TransitionD,RequirementD>>(content);
                    nodeMonoScript.RuntimeNode = returnData;
                    return (true, assetPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"An error occurred while attempting to read the backup file at {assetPath}: {ex.Message}");
                    return (false, ex.Message);
                }
            }
            else
            {
                Debug.LogError("Make sure you have a TextAsset selected when loading from the backup!");
                return (false, "NULL");
            }
            */
            return (false, "NULL");
        }
        private (bool,string) ReadJSONFromLocalSelectedFileMenu(TextAsset data, NodeMBEx nodeMonoScript)
        {
            /*
            if (data != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(data.GetInstanceID());
                //find path of selected object
                try
                {
                    string content = string.Empty;
                    using (FileStream fileStream = new FileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            // Read the entire file content
                            content = reader.ReadToEnd();
                        }
                    }
                    Debug.Log($"Found the backup file and read it in from {assetPath}.");
                    //try now reading it back into Unity?
                    //TheRecorderSettingsJSON = content;
                    //var anyErrors = ReadEditorPrefsJSON();
                    //setup our tags to match if we need more
                    //SOSGraphNodeData
                    var returnData = JsonUtility.FromJson<NodeSOB<TransitionD,RequirementD>>(content);
                    //var returnData = JsonUtility.FromJson<SOSGraphNodeData>(content);
                    //we maybe got some data
                    nodeMonoScript.RuntimeNode = returnData;
                    return (true, assetPath);
                    //populate the events by type now from the return data
                }
                catch (Exception ex)
                {
                    // If an error occurs, display the message
                    Debug.LogError($"An error occurred while attempting to read the backup file at {assetPath}: " + ex.Message);
                    return (false,ex.Message);
                }

                //var dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(FP_RecorderUtility.SAMPLESPATH, FP_RecorderUtility.BACKUP);
                //var jsonAsset = TheRecorderSettingsJSON;
                //var asset = AnimationClipRecorder.CreateInstance(true, true, AnimationInputSettings.CurveSimplificationOptions.Lossless);
                //string backupFile = "/FPBackup_" + System.DateTime.Now.ToString("yyyyMMdd_hhmm") + ".json";
                //string assetPath = AssetDatabase.GenerateUniqueAssetPath(dataPath.Item2 + backupFile);
            }
            else
            {
                Debug.LogError($"Make sure you have a TextAsset selected-when loading from the backup!");
                return (false, "NULL");
            }
            */
            return (false, "NULL");
        }
    }
}
