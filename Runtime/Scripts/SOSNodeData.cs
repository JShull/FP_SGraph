namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;

    /// <summary>
    /// This class represents our data for a node in a graph and the connection data to the other nodes
    /// Derived from Unity Scriptable Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    [Serializable]
    
    public abstract class SOSNodeData<T,A> : ScriptableObject where T : struct
    {
        public string NodeIDName;
        public List<T> StateTransitions;
        public List<SOSNodeData<T,A>> Connections;
        public List<A> Requirements;
    }
    [Serializable]
    public struct SGraphTransitionData 
    {
        public SequenceTransition Transition;
        public SequenceStatus OutcomeStatus;
        public List<UnityEvent> UnityActionEvents;
    }
}
