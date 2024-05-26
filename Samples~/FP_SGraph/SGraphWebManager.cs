namespace FuzzPhyte.SGraph.Samples
{
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System;
    // Main Mono Class for the Sample 
    public class SGraphWebManager : WebManagerMB<TransitionD, RequirementD, SOSGraphNodeDataEx>
    {
        //private SGraphWebSharpEx graphWeb;
        public SGraphGONodeMono MonoNode;
        public SGNode EntryDataNode;
        
        private void Awake()
        {
            if(MonoNode != null)
            {
                MonoNode.BuildRuntimeNode();
                if(EntryDataNode == null)
                {
                    EntryDataNode = (SGNode)MonoNode.NodeSharp;
                }
            }
            if(EntryDataNode != null)
            {
                TheWeb = new SGraphWebSharpEx(EntryDataNode);
            }
        }
        private void OnEnable()
        {
            if(unityNodes == null)
            {
                UpdateAllNodes();
            }
        }
        private void Start()
        {
            
            
        }
        //build all the nodes in the graph
        //connect all the nodes in the graph
        //set the starting node
        protected override void SetupEntryPoint(SOSGraphNodeDataEx data)
        {
            
            var currentState = data.StartingState;
            SGraphStateMachineData eventData = new SGraphStateMachineData(currentState);
            //Dictionary<SequenceTransition, SequenceStatus> stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            //Dictionary<int, List<Action>> stateActions = new Dictionary<int, List<Action>>();
            //Dictionary<SequenceStatus, List<Action>> stateActions = new Dictionary<SequenceStatus, List<Action>>();
            for(int i=0;i<data.StateTransitions.Count;i++)
            {
                var transition = data.StateTransitions[i];
                
                eventData.AddStateTransition(transition.Transition, transition.OutcomeStatus);
                for(int j=0;j<transition.UnityActionEvents.Count;j++)
                {
                    System.Action action = () => { transition.UnityActionEvents[j].Invoke(); };
                    eventData.AddStateAction(transition.OutcomeStatus, transition.Transition, action);
                } 
            }
        }
        public void UpdateAllNodes()
        {
            // Build Out Lookup Dictionary by the NodeData
            if (unityNodes == null)
            {
                unityNodes = new Dictionary<SOSGraphNodeDataEx, GameObject>();
            }
            // Clear the current nodes
            unityNodes.Clear();

            //get a list of gameObjects by the component type SGraphGoNode base
            var goNodes = FindObjectsOfType(typeof(SGraphGONodeMono));
            // Update with new nodes
            for(int i=0;i<DataNodes.Count;i++)
            {
                var aDataNode = DataNodes[i];
                for(int j=0;j<goNodes.Length;j++){
                    var goNode = goNodes[j] as SGraphGONodeMono;
                    if(goNode.NodeDataTemplate == aDataNode){
                        unityNodes.Add(aDataNode,goNode.gameObject);
                    }
                }
            }
        }
    }
}
