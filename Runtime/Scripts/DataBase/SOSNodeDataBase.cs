namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class represents our data for a node in a graph and the connection data to the other nodes
    /// Derived from Unity Scriptable Object
    /// </summary>
    /// <typeparam name="T">the transition data struct</typeparam>
    /// <typeparam name="R">the requirement type</typeparam>
    [Serializable]
    
    public abstract class SOSNodeDataBase<T,R> : ScriptableObject where T : struct where R : struct
    {
        public string NodeIDName;
        public List<T> StateTransitions;
        public List<SOSNodeDataBase<T,R>> Connections;
        public List<R> Requirements;
    }
}
