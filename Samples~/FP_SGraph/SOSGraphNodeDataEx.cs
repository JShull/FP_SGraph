namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using System;
    using UnityEngine;

    // main scriptable object 'data' class for the node(s) in the sample

    [Serializable]
    [CreateAssetMenu(fileName = "SGraphData", menuName = "FuzzPhyte/SGraph/SGraphData", order = 0)]
    public class SOSGraphNodeDataEx : NodeSOB<TransitionD,RequirementD>
    {
        public bool IsStartNode;
        public SequenceStatus StartingState;
    }
}
