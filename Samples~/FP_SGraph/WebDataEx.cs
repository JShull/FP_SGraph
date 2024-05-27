using System.Collections.Generic;

namespace FuzzPhyte.SGraph.Samples
{
    //Main Web Graph Data Class for the Sample
    public class WebDataEx : WebSB<TransitionD,RequirementD>
    {
        public new List<SGNode> allNodes;
        public new List<SGNode> nextNodes;

        public new SGNode currentNode;
        public WebDataEx(SGNode entryDataNode)
        {
            allNodes = new List<SGNode>();
            nextNodes = new List<SGNode>();
            currentNode = entryDataNode;
        }
        public void SetupWebNodeList(ref SGNode aNode)
        {
            if(allNodes==null)
            {
                allNodes = new List<SGNode>();
            }
            //add node to the Web data
            AddNode(aNode);
        }
        public void AddNode(SGNode aNode)
        {
            if (!allNodes.Contains(aNode))
            {
                allNodes.Add(aNode);
                aNode.SGStateMachine.OnFinish += OnNodeFinished;
            }
        }

        public (bool,string) CreateConnection(ref SGNode nodeA, ref SGNode nodeB)
        {
            //aNode is the head bNode is the tail
            ///if both of our nodes are in our current list
            if (allNodes.Contains(nodeA) && allNodes.Contains(nodeB))
            {
                //if we already have a connection we can't connect again
                if(nodeA.Connections.ContainsKey(nodeB.SGNodeID))
                {
                    return (false,"Already has a connection");
                }
                //add the connection
                nodeA.Connections.Add(nodeB.SGNodeID, nodeB);
                return (true,"Generated the connection!");
            }
            return (false,"One of the nodes is not in the list");
            //return ConnectNodes(ref aNode,ref bNode);  
        }

        public override void AddNode(NodeSB<TransitionD, RequirementD> node)
        {
            var aNode = node as SGNode;
            if (!allNodes.Contains(aNode))
            {
                allNodes.Add(aNode);
                aNode.SGStateMachine.OnFinish += OnNodeFinished;
            }
        }
        public override void RemoveNode(NodeSB<TransitionD, RequirementD> node)
        {
            var aNode = node as SGNode;
            if (allNodes.Contains(aNode))
            {
                allNodes.Remove(aNode);
                aNode.SGStateMachine.OnFinish -= OnNodeFinished;
            }
        }

        public override bool ConnectNodes(NodeSB<TransitionD, RequirementD> nodeA, NodeSB<TransitionD, RequirementD> nodeB)
        {
            var SGnodeA = nodeA as SGNode;
            var SGnodeB = nodeB as SGNode;
            ///if both of our nodes are in our current list
            if (allNodes.Contains(SGnodeA) && allNodes.Contains(SGnodeB))
            {
                //if we already have a connection we can't connect again
                if(SGnodeA.Connections.ContainsKey(SGnodeB.SGNodeID))
                {
                    return false;
                }
                //add the connection
                SGnodeA.Connections.Add(SGnodeB.SGNodeID, SGnodeB);
                return true;
            }
            return false;
        }

        public override bool ConnectNodes(string nodeA, string nodeB)
        {
            SGNode a = allNodes.Find(x => x.SGNodeID == nodeA);
            SGNode b = allNodes.Find(x => x.SGNodeID == nodeB);
            if (a != null && b != null)
            {
                //if we already have a connection we can't connect again
                if (a.Connections.ContainsKey(b.SGNodeID))
                {
                    return false;
                }
                //add the connection
                a.Connections.Add(b.SGNodeID, b);
                return true;
            }
            return false;
        }

        protected override void OnNodeFinished(StateMachineSB<RequirementD> nodeEvent)
        {
           //Get the node that finished
            SGNode finishedNode = GetNodeByEvent(nodeEvent) as SGNode;
            //get the connections
            Dictionary<string, SGNode> connections = finishedNode.Connections;
            //if we have connections we need to now add them to the next allNodes but also clean up those next allNodes
            if (connections.Count > 0)
            {
                nextNodes.Clear();
                foreach (KeyValuePair<string, SGNode> connection in connections)
                {
                    nextNodes.Add(connection.Value);
                }
            }
        }

        public override bool TryToGoToNode(string nodeID)
        {
            //if our nodeID is not in our nextNode list
            //we need to check if our current node has a connection to the nodeID
            //if it does we can go to it
            //if it doesn't we can't go to it
            if (nextNodes.Exists(x => x.SGNodeID == nodeID))
            {
                if (nextNodes.Find(x => x.SGNodeID == nodeID).RequirementsMet())
                {
                    return GoToNode(nodeID).Item1;
                }
            }
            
            return false;
        }

        protected override (bool, NodeSB<TransitionD, RequirementD>) GoToNode(string nodeID)
        {
            //is our node in the nextNodes list
            SGNode node = nextNodes.Find(x => x.SGNodeID == nodeID);
            if (node != null)
            {
                currentNode = node;
                return (true,currentNode);
            }
            return (false, currentNode);
        }

        protected override NodeSB<TransitionD, RequirementD> GetNodeByEvent(StateMachineSB<RequirementD> sEvent)
        {
            return allNodes.Find(x => x.SGStateMachine == sEvent);
        }
    }
}
