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
    using FuzzPhyte.Utility;
    using System.Text;

    [CustomEditor(typeof(SGraphGONodeMono))]
    public class SGraphGONodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //visually setup the editor script
            GUIStyle copyScriptableObjectDataButtonStyle = new GUIStyle(GUI.skin.button);
            copyScriptableObjectDataButtonStyle.normal.textColor = Color.red; // Text color for the normal state
            copyScriptableObjectDataButtonStyle.hover.textColor = Color.yellow; // Text color when hovered
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.margin = new RectOffset(15, 5, 5, 5);
            boxStyle.normal.textColor = Color.white;
            boxStyle.hover.textColor = Color.blue;
            boxStyle.border = new RectOffset(20, 7, 7, 7);
            boxStyle.focused.textColor = Color.blue;

            EditorGUILayout.Space(20);
            FP_Utility_Editor.DrawUILine(Color.red, 5, 5);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(boxStyle);


            //
            SGraphGONodeMono nodeMonoScript = (SGraphGONodeMono)target;

            // Add a button to the inspector of SGraphGONodeMono objects
            if (GUILayout.Button("Generate Events From Data",copyScriptableObjectDataButtonStyle))
            {
                GenerateEventsByType(nodeMonoScript);
            }
            EditorGUILayout.Space(10);
            FP_Utility_Editor.DrawUILine(Color.red, 5, 5);
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Build Copied Data"))
            {
                var returnData = WriteRuntimeDataObjectWithUnityEvents(nodeMonoScript);
                if (returnData.Item1)
                {
                    WriteJSONFile(nodeMonoScript, returnData.Item2);
                }
                
            }
            EditorGUILayout.Space(10);
            if(GUILayout.Button("Restore Data From JSON"))
            {
                var dataInfo = ReadJSONFromLocalSelectedFileMenu(nodeMonoScript.JSONData,nodeMonoScript);
                if (dataInfo.Item1)
                {
                    Debug.Log($"Try restoring the data now to our EventList");
                    nodeMonoScript.ReplaceEventsByTypeWithRuntimeData();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void GenerateEventsByType(SGraphGONodeMono nodeMonoScript)
        {
            if (nodeMonoScript.NodeDataTemplate != null)
            {
                nodeMonoScript.EventsByType.Clear(); // Clear current list to repopulate

                // Assuming StateTransitions is a List<T> in your SOSNodeData<T, A>
                foreach (var transition in nodeMonoScript.NodeDataTemplate.StateTransitions)
                {
                    nodeMonoScript.EventsByType.Add(transition);
                }

                // Mark the object as having been modified, so Unity knows to save the changes
                EditorUtility.SetDirty(nodeMonoScript);

                // Optionally, you could also call nodeMonoScript.BuildRuntimeNode() here if you want
                // to automatically rebuild the runtime node whenever the list is generated.
            }
        }
        
        private (bool,string) WriteRuntimeDataObjectWithUnityEvents(SGraphGONodeMono nodeMonoScript)
        {
            //generate sample data folder
            //FP_Utility_Editor.CreatePackageSampleFolder(FP_SGraphUtility<SGraphTransitionData, string>.PRODUCT_NAME_UNITY, FP_SGraphUtility<SGraphTransitionData, string>.BASEVERSION);
            if(nodeMonoScript.NodeDataTemplate != null)
            {
                nodeMonoScript.RuntimeNode = nodeMonoScript.BuildRuntimeNode();
                //save this scriptableobject to a file and update asset database
                EditorUtility.SetDirty(nodeMonoScript);
                var localSamplesFolder = FP_SGraphUtility<SGraphTransitionData, string>.ReturnInstallPath();

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

                    if (File.Exists(Path.Combine(potentialFolder, FP_SGraphUtility<SGraphTransitionData, string>.SAMPLELOCALFOLDER)))
                    {
                        Debug.Log($"Local Samples Folder: {FP_SGraphUtility<SGraphTransitionData, string>.SAMPLELOCALFOLDER} exists!");
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.Combine(potentialFolder, FP_SGraphUtility<SGraphTransitionData, string>.SAMPLELOCALFOLDER));
                    }
                    Debug.Log($"Before Assets: {localSamplesFolder}");
                    localSamplesFolder = Path.Combine("Assets", localSamplesFolder);
                    Debug.Log($"After Assets: {localSamplesFolder}");
                    AssetDatabase.Refresh();
                    dataPath = FP_Utility_Editor.CreateAssetDatabaseFolder(localSamplesFolder, FP_SGraphUtility<SGraphTransitionData, string>.SAMPLELOCALFOLDER);
                    if (!dataPath.Item1)
                    {
                        Debug.LogError($"Error on Path Creation, check the logs! Path at: {dataPath.Item2}");
                        return dataPath;
                    }
                    var returnPath = FP_SGraphUtility<SGraphTransitionData, string>.WriteSOSNodeData(nodeMonoScript.RuntimeNode, dataPath.Item2, "SOSNodeData.asset");
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
            return (false, "NULL");
        }
        private void WriteJSONFile(SGraphGONodeMono nodeMonoScript,string fullPath)
        {
            if (nodeMonoScript.NodeDataTemplate != null)
            {
                nodeMonoScript.RuntimeNode = nodeMonoScript.BuildRuntimeNode();
                //save this scriptableobject to a file and update asset database
                EditorUtility.SetDirty(nodeMonoScript);
            }
            var TheRecorderSettingsJSON = JsonUtility.ToJson(nodeMonoScript.RuntimeNode);
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

                Debug.Log($"Backup file generated at {assetPath} lets try generating and saving a 'Unity Recorder List'");

                //
            }
            catch (Exception ex)
            {
                // If an error occurs, display the message
                Debug.LogError("An error occurred while writing the backup file: " + ex.Message);
            }
        }

        private (bool,string) ReadJSONFromLocalSelectedFileMenu(TextAsset data, SGraphGONodeMono nodeMonoScript)
        {
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
                    var returnData = JsonUtility.FromJson<SOSNodeData<SGraphTransitionData,string>>(content);
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
        }
    }
}
