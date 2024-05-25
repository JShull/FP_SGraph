namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    /// <summary>
    /// This class is setup to manage a web of allNodes that can be traversed
    /// we are assuming that as we traverse the web we aren't going back
    /// we are also assuming that we are only going forward to the next set of possible 'next' allNodes
    /// Upon observing a node finishing we immediately update the next allNodes list in preparation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SGraphWeb<T>
    {
        /// <summary>
        /// List of all allNodes
        /// </summary>
        protected List<SGraphNode<T>> allNodes;
        /// <summary>
        /// initial root node for this graph
        /// </summary>
        protected SGraphNode<T> headNode;
        /// <summary>
        /// list of allNodes that are next in line to be processed
        /// </summary>
        protected List<SGraphNode<T>> nextNodes;
        /// <summary>
        /// current node we are actively on
        /// </summary>
        protected SGraphNode<T> currentNode;
        public SGraphNode<T> CurrentNode
        {
            get { return currentNode; }
        }
        public SGraphStateMachine<T> CurrentEvent
        {
            get { return currentNode.SGStateMachine;}
        }

        #region Constructors
        protected SGraphWeb()
        {
            allNodes = new List<SGraphNode<T>>();
            nextNodes = new List<SGraphNode<T>>();
            currentNode = null;
        }
        protected SGraphWeb(SGraphNode<T> head)
        {
            allNodes = new List<SGraphNode<T>>();
            nextNodes = new List<SGraphNode<T>>();
            AddNode(head);
            headNode = head;
            currentNode = head;
        }
        #endregion
        public void AddNode(SGraphNode<T> node)
        {
            if (!allNodes.Contains(node))
            {
                allNodes.Add(node);
                node.SGStateMachine.OnFinish += OnNodeFinished;
            }
        }
        protected void RemoveNode(SGraphNode<T> node)
        {
            if (allNodes.Contains(node))
            {
                allNodes.Remove(node);
                node.SGStateMachine.OnFinish -= OnNodeFinished;
            }
        }
        /// <summary>
        /// Connect nodes together via their SGNodeID
        /// directional connection from nodeA-->nodeB
        /// </summary>
        /// <param name="nodeA">head node</param>
        /// <param name="nodeB">tail node</param>
        /// <returns></returns>
        public virtual bool ConnectNodes(SGraphNode<T> nodeA, SGraphNode<T> nodeB)
        {
            ///if both of our nodes are in our current list
            if (allNodes.Contains(nodeA) && allNodes.Contains(nodeB))
            {
                //if we already have a connection we can't connect again
                if(nodeA.Connections.ContainsKey(nodeB.SGNodeID))
                {
                    return false;
                }
                //add the connection
                nodeA.Connections.Add(nodeB.SGNodeID, nodeB);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Connect nodes together via their SGNodeID
        /// directional connection from nodeA-->nodeB
        /// </summary>
        /// <param name="nodeA">head node</param>
        /// <param name="nodeB">tail node</param>
        /// <returns></returns>
        public virtual bool ConnectNodes(string nodeA, string nodeB)
        {
            SGraphNode<T> a = allNodes.Find(x => x.SGNodeID == nodeA);
            SGraphNode<T> b = allNodes.Find(x => x.SGNodeID == nodeB);
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
        /// <summary>
        /// We need to update the nextNodes list when a node finishes
        /// </summary>
        /// <param name="nodeEvent"></param>
        protected virtual void OnNodeFinished(SGraphStateMachine<T> nodeEvent)
        {
            //Get the node that finished
            SGraphNode<T> finishedNode = GetNodeByEvent(nodeEvent);
            //get the connections
            Dictionary<string, SGraphNode<T>> connections = finishedNode.Connections;
            //if we have connections we need to now add them to the next allNodes but also clean up those next allNodes
            if (connections.Count > 0)
            {
                nextNodes.Clear();
                foreach (KeyValuePair<string, SGraphNode<T>> connection in connections)
                {
                    nextNodes.Add(connection.Value);
                }
            }
        }

        /// <summary>
        /// public accessor to try to go to a node
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public virtual bool TryToGoToNode(string nodeID)
        { 
            //if our nodeID is not in our nextNode list
            //we need to check if our current node has a connection to the nodeID
            //if it does we can go to it
            //if it doesn't we can't go to it
            if (nextNodes.Exists(x => x.SGNodeID == nodeID))
            {
                if (nextNodes.Find(x => x.SGNodeID == nodeID).RequirementsMet)
                {
                    return GoToNode(nodeID).Item1;
                }
            }
            
            return false;

        }
        /// <summary>
        /// internal accessor to go to a node
        /// </summary>
        /// <param name="nodeID">nodeID as string</param>
        /// <returns></returns>
        protected (bool,SGraphNode<T>) GoToNode(string nodeID)
        {
            //is our node in the nextNodes list
            SGraphNode<T> node = nextNodes.Find(x => x.SGNodeID == nodeID);
            if (node != null)
            {
                currentNode = node;
                return (true,currentNode);
            }
            return (false, currentNode);
        }
        /// <summary>
        /// internal accessor to go to a node
        /// </summary>
        /// <param name="node">nodeID as SGraphNode</param>
        /// <returns></returns>
        protected (bool, SGraphNode<T>) GoToNode(SGraphNode<T> node)
        {
            //is our node in the nextNodes list
            return GoToNode(node.SGNodeID);
        }
        /// <summary>
        /// Return a node by its event
        /// </summary>
        /// <param name="sEvent"></param>
        /// <returns></returns>
        protected SGraphNode<T> GetNodeByEvent(SGraphStateMachine<T> sEvent)
        {
            return allNodes.Find(x => x.SGStateMachine == sEvent);
        }
    }
}
