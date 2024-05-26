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
    public abstract class UNodeMB<T,R> : MonoBehaviour where T:struct where R:struct
    {
        public NodeSB<T,R> NodeSharp { get; set; }
        public NodeSOB<T, R> NodeDataTemplate;
        public List<T> EventsByType;
        protected NodeSOB<T, R> runtimeNode;
        public NodeSOB<T, R> RuntimeNode
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
        public abstract NodeSOB<T, R> BuildRuntimeNode();
    }
}
