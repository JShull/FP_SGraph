namespace FuzzPhyte.SGraph.Samples
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    /// <summary>
    /// A Sample Gameobject Node
    /// </summary>
    public class SGraphGONodeMono : SGraphGONode<SGraphTransitionData,string>
    {
        [Header("Backup Data")]
        public TextAsset JSONData;
        public override SOSNodeData<SGraphTransitionData, string> BuildRuntimeNode()
        {
            //return base.BuildRuntimeNode();
            //copy data from my NodeDataTemplate to this runtimeNode
            var tempData = Instantiate(NodeDataTemplate);

            //go through my NodeDataTemplate - find the StateTransitions and match them with my EventsByType
            //if they match, add the event to the NodeDataTemplate
            for (int i = 0; i < NodeDataTemplate.StateTransitions.Count; i++)
            {
                var curStateTransition = tempData.StateTransitions[i];
                for (int j = 0; j < EventsByType.Count; j++)
                {
                    List<UnityEvent> listByTransition = EventsByType[j].UnityActionEvents;
                    if (NodeDataTemplate.StateTransitions[i].Transition ==EventsByType[j].Transition)
                    {
                        //need to modify and replace the unity events from the EventsByType[j] with the ones in the NodeDataTemplate
                        
                        for(int k = 0; k < listByTransition.Count; k++)
                        {
                            //copy it manually
                            curStateTransition.UnityActionEvents.Add(listByTransition[k]);
                        }
                        Debug.Log($"Added Event List for transition [{curStateTransition.Transition}] that had {listByTransition.Count} events in the Unity events list");
                    }
                }
            }
            return tempData;
        }
        /// <summary>
        /// This method replaces the EventsByType list with the runtime data that we probably read in from JSON
        /// </summary>
        public void ReplaceEventsByTypeWithRuntimeData()
        {
            for (int i = 0; i < RuntimeNode.StateTransitions.Count; i++)
            {
                var curStateTransition = RuntimeNode.StateTransitions[i];
                EventsByType.Clear();
                // now copy each RunTimeNode StateTransition back into the EventsByType list
                SGraphTransitionData tempData = new SGraphTransitionData();
                tempData.Transition = curStateTransition.Transition;
                tempData.OutcomeStatus = curStateTransition.OutcomeStatus;
                for (int j = 0; j < curStateTransition.UnityActionEvents.Count; j++)
                {
                    tempData.UnityActionEvents.Add(curStateTransition.UnityActionEvents[j]);
                }
                EventsByType.Add(tempData);
            }


        }
    }
}

