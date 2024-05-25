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
    public abstract class SGraphGONode<T,A> : MonoBehaviour where T:struct
    {

        public SOSNodeData<T, A> NodeDataTemplate;
        public List<T> EventsByType;
        protected SOSNodeData<T, A> runtimeNode;
        public SOSNodeData<T, A> RuntimeNode
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
        public abstract SOSNodeData<T, A> BuildRuntimeNode();
    }
}
