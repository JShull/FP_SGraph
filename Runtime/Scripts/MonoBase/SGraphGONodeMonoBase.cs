namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// A monobehaviour that represents the Unity Scene GameObject references needed to pair
    /// with our abstract SGraphNodeData class
    /// </summary>
    [Serializable]
    public abstract class SGraphGONodeMonoBase<T,R> : MonoBehaviour where T:struct where R:struct
    {
        public SGraphNodeSharpBase<T,R> NodeSharp { get; set; }
        public SOSNodeDataBase<T, R> NodeDataTemplate;
        public List<T> EventsByType;
        protected SOSNodeDataBase<T, R> runtimeNode;
        public SOSNodeDataBase<T, R> RuntimeNode
        {
            get
            {
                return runtimeNode;
            }
            set
            {
                runtimeNode = value;
            }
        }
        /// <summary>
        /// Uses the template data to build a runtime node with scene/gameobject references
        /// </summary>
        public abstract SOSNodeDataBase<T, R> BuildRuntimeNode();
    }
}
