namespace FuzzPhyte.SGraph
{
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// A monobehaviour that represents the Unity Scene GameObject references needed to pair
    /// with our abstract SGraphNodeData class
    /// </summary>
    public abstract class UNodeMB<T,R> : MonoBehaviour where T:struct where R:struct
    {
        public SequenceStatus StartState;
        public abstract NodeSB<T,R> NodeSharp { get; set; }
        public abstract NodeSOB<T, R> NodeDataTemplate { get; set; }
        public abstract List<T> EventsByType {get;set;}
        public abstract List<UNodeMB<T, R>> UnityOutConnections { get; set; }
        
        /// <summary>
        /// Build out the runtime node NodeSharp from the NodeDataTemplate
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="transitions"></param>
        //public abstract void BuildRuntimeNode(List<R>requirements, List<T>transitions);
    }
}
