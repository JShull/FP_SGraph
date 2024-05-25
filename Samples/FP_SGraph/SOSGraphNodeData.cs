namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using System;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "SGraphData", menuName = "FuzzPhyte/SGraph/SGraphData", order = 0)]
    public class SOSGraphNodeData : SOSNodeData<SGraphTransitionData,string>
    {
        public bool IsStartNode;
        public SequenceStatus StartingState;
    }
}
