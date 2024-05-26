namespace FuzzPhyte.SGraph.Samples
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    /// <summary>
    /// A Sample Gameobject Node
    /// </summary>
    public class NodeMBEx : UNodeMB<TransitionD,RequirementD>
    {
        public SGNode SharpData;
        public SOSGraphNodeDataEx NodeDataGen;
        public List<NodeMBEx> ConnectedNodes = new List<NodeMBEx>();

        [Header("Backup Data")]
        public TextAsset JSONData;

        public override NodeSB<TransitionD, RequirementD> NodeSharp { get { return SharpData; } set { SharpData = (SGNode)value; } }
        public override NodeSOB<TransitionD, RequirementD> NodeDataTemplate { get => NodeDataGen; set { NodeDataGen = (SOSGraphNodeDataEx)value; } }
        public override List<UNodeMB<TransitionD,RequirementD>> UnityOutConnections { get=>ConnectedNodes.Cast<UNodeMB<TransitionD,RequirementD>>().ToList(); set { ConnectedNodes = value.Cast<NodeMBEx>().ToList(); } }

        /// <summary>
        /// Humble Setup
        /// Build out our internal C# runtime data
        /// Mix of Unity and C# data
        /// </summary>
        /// <param name="shellRequirements"></param>
        /// <returns></returns>
        public override void BuildRuntimeNode(List<RequirementD>shellRequirements, List<TransitionD>interiorTransitions)
        {
            //new runtime data node
            SharpData = new SGNode(this.gameObject.GetInstanceID().ToString());
            //starting state from TemplateData
            //var curData = NodeDataTemplate as SOSGraphNodeDataEx;
            //var curNodeSharp = NodeSharp as SGNode;
            SharpData.StartState = NodeDataGen.StartingState;
            SharpData.SetupStateMachine(shellRequirements, interiorTransitions);
            //cast list as SGNode
            //NodeSharp.BuildConnectionDictionaryList(externalConnections.OfType<SGNode>().ToList());
            //go through my NodeDataTemplate - find the StateTransitions and match them with my EventsByType
            //if they match, add the event to the NodeDataTemplate
            /*
            for (int i = 0; i < NodeDataTemplate.StateTransitions.Count; i++)
            {
                var curStateTransition = NodeDataTemplate.StateTransitions[i];
                for (int j = 0; j < EventsByType.Count; j++)
                {
                    List<UnityEvent> listByTransition = EventsByType[j].UnityActionEvents;
                    if (NodeDataTemplate.StateTransitions[i].Transition == EventsByType[j].Transition)
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
            */
            //NodeSharp.Connections
            //
            
            //
            //return NodeDataTemplate;
        }
        /// <summary>
        /// Called from the Editor Script
        /// </summary>
        public void UpdateUnityFromTemplateData()
        {
            this.StartState = NodeDataGen.StartingState;
            //this.ConnectedNodes.Clear();
            //this.ConnectedNodes = editorConnectionsPassed;
        }
        /// <summary>
        /// This method replaces the EventsByType list with the runtime data that we probably read in from JSON
        /// </summary>
        public void ReplaceEventsByTypeWithRuntimeData()
        {
            /*
            for (int i = 0; i < RuntimeNode.StateTransitions.Count; i++)
            {
                var curStateTransition = RuntimeNode.StateTransitions[i];
                EventsByType.Clear();
                // now copy each RunTimeNode StateTransition back into the EventsByType list
                TransitionD tempData = new TransitionD();
                tempData.Transition = curStateTransition.Transition;
                tempData.OutcomeStatus = curStateTransition.OutcomeStatus;
                for (int j = 0; j < curStateTransition.UnityActionEvents.Count; j++)
                {
                    tempData.UnityActionEvents.Add(curStateTransition.UnityActionEvents[j]);
                }
                EventsByType.Add(tempData);
            }
            */
        }
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (NodeDataTemplate != null)
            {
                // Trigger editor update when NodeDataTemplate changes
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        #endif
    }
}

