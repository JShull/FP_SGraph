using System;
using UnityEngine;
using FuzzPhyte.Utility;
using UnityEditor;
using System.Collections;
using FuzzPhyte.Utility.Editor;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using System.Linq;

namespace FuzzPhyte.SGraph.Editor
{
    [Serializable]
    public static class FP_SGraphUtility<T,A> where T: struct 
    {
        public const string PRODUCT_NAME = "FP_Graph";
        public const string PRODUCT_NAME_UNITY = "FP Graph";
        public const string CAT0 = "NodeData";
        public const string CAT1 = "GraphType";
        public const string SAMPLESPATH = "Assets/" + PRODUCT_NAME + "/Samples/URPSamples";
        public const string SAMPLELOCALFOLDER = PRODUCT_NAME_UNITY + " Samples";
        public const string INSTALLSAMPLEPATH = "Samples\\" + PRODUCT_NAME_UNITY + "\\";
        public const string BASEVERSION = "0.1.0";

        public static string WriteSOSNodeData(SOSNodeData<T, A> passedData,string fullLocalPath,string assetName)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(fullLocalPath + "/" + assetName);
            return FP_Utility_Editor.CreateAssetAt(passedData, assetPath);
        }
        public static string ReturnInstallPath()
        {
            //var result = await Rereturn INSTALLSAMPLEPATH + BASEVERSION;turnPackageVersion();
            return INSTALLSAMPLEPATH + BASEVERSION;
        }
    }
}
