using System;
using UnityEngine;
using FuzzPhyte.Utility;
using UnityEditor;
using System.Collections;
using FuzzPhyte.Utility.Editor;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using System.Linq;
using System.Collections.Generic;

namespace FuzzPhyte.SGraph.Editor
{
    [Serializable]
    public static class FP_SGraphUtility<T,R> where T: struct where R: struct
    {
        public const string PRODUCT_NAME = "FP_SGraph";
        public const string PRODUCT_NAME_UNITY = "FP SGraph";
        public const string CAT0 = "NodeData";
        public const string CAT1 = "GraphType";
        public const string SAMPLESPATH = "Assets/" + PRODUCT_NAME + "/Samples/FP_SGraph";
        public const string SAMPLELOCALFOLDER = PRODUCT_NAME_UNITY + " Samples";
        public const string INSTALLSAMPLEPATH = "Samples/" + PRODUCT_NAME_UNITY + "/";
        public const string BASEVERSION = "0.1.0";
        public static Color DefaultEditorColor = new Color(1,0.5f,0,1);
        public static Color LineBreakWarningColor = new Color(1,0,0,1);
        public static Color DefaultEditorHoverColor = Color.yellow;
        public static Color DefaultBoxColorText = Color.white;
        public static Color DefaultBoxColorTextHover = Color.blue;
        public static Color DefaultBoxColorTextFocused = Color.cyan;
        public static Color DefaultButtonColor = Color.white;
        
        public static GUILayoutOption[] requiredStyleTag = new GUILayoutOption[]
        {
            GUILayout.Width(125),
            GUILayout.ExpandWidth(true),
            GUILayout.MaxWidth(200),
            GUILayout.MinWidth(50)
        };
        public static GUILayoutOption[] requiredStyleName = new GUILayoutOption[]
        {
            GUILayout.Width(75),
            GUILayout.ExpandWidth(true),
            GUILayout.MaxWidth(200),
            GUILayout.MinWidth(25)
        };
        public static GUILayoutOption[] closeButtonStyle = new GUILayoutOption[]
        {
            GUILayout.Width(25),
            GUILayout.ExpandWidth(true),
            GUILayout.MaxWidth(200),
            GUILayout.MinWidth(25)
        };

        public static GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
        {
            margin = new RectOffset(15, 5, 5, 5),
            normal = {textColor = DefaultBoxColorText},
            hover = {textColor = DefaultBoxColorTextHover},
            border = new RectOffset(20, 7, 7, 7),
            focused = {textColor = DefaultBoxColorTextFocused},
        };

        public static string WriteSOSNodeData(SOSNodeDataBase<T, R> passedData,string fullLocalPath,string assetName)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(fullLocalPath + "/" + assetName);
            return FP_Utility_Editor.CreateAssetAt(passedData, assetPath);
        }
        public static string ReturnInstallPath()
        {
            //var results = FP_Utility_Editor.CreatePackageSampleFolder(PRODUCT_NAME, "\\"+BASEVERSION);
            //return results.Item1;
            return INSTALLSAMPLEPATH + BASEVERSION;
        }
    }
}
