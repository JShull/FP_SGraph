namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;
    
    [Serializable]
    public struct TransitionD 
    {
        public SequenceTransition Transition;
        public SequenceStatus OutcomeStatus;
        public List<UnityEvent> UnityActionEvents;
    }
}
