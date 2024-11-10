namespace FuzzPhyte.SGraph.Samples
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Linq;
    using UnityEngine.Events;

    // Main Mono Class for the Sample 
    public class SGraphWebManager : WebManagerMB<TransitionD, RequirementD, NodeDataSOBEx>
    {
        public bool StartWebOnPlay = true;
        //private SGraphWebSharpEx graphWeb;
        public NodeMBEx EntryNode;
        public SGNode EntryDataNode;
        [SerializeField]
        private List<NodeMBEx> allNodesInScene = new List<NodeMBEx>();
        public WebDataEx TheWeb; 
        public Dictionary<NodeDataSOBEx,NodeMBEx> unityNodes;
        [SerializeField]
        private List<NodeDataSOBEx> dataNodes;
        public override List<NodeDataSOBEx> DataNodes { get => dataNodes; set {dataNodes=value;} }

        //public override List<NodeDataSOBEx> DataNodes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        // public override WebSB<TransitionD, RequirementD> TheWeb { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void Awake()
        {
            if(EntryNode != null)
            {
                //quick Setup so we can build the web data class out
                EntryDataNode = EntryNode.InitialSetup();
            }
            
        }
        private void OnEnable()
        {
            UpdateAllNodes();
        }
        private void Start()
        {
            //build out web data 
            if(EntryDataNode != null)
            {
                TheWeb = new WebDataEx(EntryDataNode);
            }else{
                Debug.LogWarning("No Entry Data Node was set");
                return;
            }
            //build each node in the graph over a loop

            for(int i=0;i<allNodesInScene.Count;i++)
            {
                //builds the node data
                var curNode = allNodesInScene[i];
                var theNodeData = curNode.SetupNodeData(allNodesInScene);
                TheWeb.SetupWebNodeList(ref theNodeData);
            }
            //nodes are all now connected on the runtime but not the data model under the TheWeb
            for(int i=0;i<allNodesInScene.Count;i++)
            {
                //foreach connection on the node we need it and the connection and that needs to be passed to the TheWeb
                //var aRunTime = allNodesInScene[i].SharpData;
                var aRuntime = allNodesInScene[i];
                //each connector node
                for(int j=0;j<aRuntime.ConnectedNodes.Count;j++)
                {
                    var bNode = aRuntime.ConnectedNodes[j].SharpData;
                    var aNode = aRuntime.SharpData;
                    var goodConnection = TheWeb.CreateConnection(ref aNode,ref bNode);
                    Debug.Log($"Connection {aNode} to {bNode} was connected?: {goodConnection.Item1}, reason? {goodConnection.Item2}");
                }
            }
            
        }
        //build all the nodes in the graph
        //connect all the nodes in the graph
        //set the starting node
        protected override void SetupEntryPoint(NodeDataSOBEx data)
        {
            
            var currentState = data.StartingState;
            SGStateMachine eventData = new SGStateMachine(currentState);
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
                unityNodes = new Dictionary<NodeDataSOBEx, NodeMBEx>();
            }
            // Clear the current nodes
            unityNodes.Clear();
            // data nodes
            if(dataNodes == null)
            {
                dataNodes = new List<NodeDataSOBEx>();
            }
            //clear data nodes?
            dataNodes.Clear();
            for(int i=0;i<allNodesInScene.Count;i++)
            {
                dataNodes.Add(allNodesInScene[i].NodeDataGen);
            }
            //get a list of gameObjects by the component type SGraphGoNode base
            var goNodes = FindObjectsOfType(typeof(NodeMBEx)).ToList();
            Debug.LogWarning($"All Nodes in Scene Size = {allNodesInScene.Count}");
            Debug.LogWarning($"GoNodes Size = {goNodes.Count}");
            Debug.LogWarning($"Unity Node Size = {unityNodes.Count}");
            Debug.LogWarning($"DataNodes Size = {dataNodes.Count}");
            allNodesInScene = goNodes.Cast<NodeMBEx>().ToList();
            // Update with new nodes
            for(int i=0;i<DataNodes.Count;i++)
            {
                var aDataNode = DataNodes[i];
                for(int j=0;j<goNodes.Count;j++){
                    var goNode = goNodes[j] as NodeMBEx;
                    if(goNode.NodeDataTemplate == aDataNode){
                        unityNodes.Add(aDataNode,goNode);
                        break;
                    }
                }
            }
        }

        public override GameObject ReturnNodeByData(NodeDataSOBEx data, out bool foundMatch)
        {
            if (unityNodes.ContainsKey(data))
            {
                foundMatch = true;
                return unityNodes[data].gameObject;
            }
            foundMatch = false;
            return null;
        }

        public override UnityEvent ReturnUnityEventFromDataAction(Action action)
        {
            UnityEvent unityEvent = new UnityEvent();
            unityEvent.AddListener(() => { action(); });
            return unityEvent;
        }
    }
}
