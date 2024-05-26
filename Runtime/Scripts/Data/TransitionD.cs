namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;
    using FuzzPhyte.Utility.Meta;
    
    [Serializable]
    public struct TransitionD 
    {
        public SequenceTransition Transition;
        public SequenceStatus OutcomeStatus;
        public List<UnityEvent> UnityActionEvents;
    }
}
