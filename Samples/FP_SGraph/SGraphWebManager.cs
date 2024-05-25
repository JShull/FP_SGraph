namespace FuzzPhyte.SGraph.Samples
{
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    public class SGraphWebManager : MonoBehaviour
    {
        public List<SOSGraphNodeData> DataNodes;

        protected Dictionary<SOSGraphNodeData, GameObject> unityNodes;

        private void Start()
        {
            //build out the data classes by the SO GraphNodeData
            unityNodes = new Dictionary<SOSGraphNodeData, GameObject>();
        }
        private void CreateEventData(SOSGraphNodeData data)
        {
            var currentState = data.StartingState;

            SGraphStateMachineData eventData = new SGraphStateMachineData(data.StartingState);
            if (data.Requirements != null)
            {
                //eventData.unlock
            }
            
        }
        public GameObject ReturnNodeByData(SOSGraphNodeData data, out bool foundMatch)
        {
            if (unityNodes.ContainsKey(data))
            {
                foundMatch = true;
                return unityNodes[data];
            }
            foundMatch = false;
            return null;
        }
    }
}
