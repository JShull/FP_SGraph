namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    /// <summary>
    /// This class is setup to manage a web of allNodes that can be traversed
    /// we are assuming that as we traverse the web we aren't going back
    /// we are also assuming that we are only going forward to the next set of possible 'next' allNodes
    /// Upon observing a node finishing we immediately update the next allNodes list in preparation
    /// </summary>
    // <typeparam name="T">struct that is the data for the transitions</typeparam>
    // <typeparam name="R">the type requirement</typeparam>
    public abstract class WebSB<T,R> where T : struct where R : struct
    {
        /// <summary>
        /// List of all allNodes
        /// </summary>
        protected List<NodeSB<T,R>> allNodes;
        /// <summary>
        /// initial root node for this graph
        /// </summary>
        protected NodeSB<T,R> headNode;
        public NodeSB<T,R> HeadNode
        {
            get { return headNode; }
        }
        /// <summary>
        /// list of allNodes that are next in line to be processed
        /// </summary>
        protected List<NodeSB<T,R>> nextNodes;
        /// <summary>
        /// current node we are actively on
        /// </summary>
        protected NodeSB<T,R> currentNode;
        public NodeSB<T,R> CurrentNode
        {
            get { return currentNode; }
        }
        
        public abstract void AddNode(NodeSB<T,R> node);

        public abstract void RemoveNode(NodeSB<T,R> node);
        
        /// <summary>
        /// Connect nodes together via their SGNodeID
        /// directional connection from nodeA-->nodeB
        /// </summary>
        /// <param name="nodeA">head node</param>
        /// <param name="nodeB">tail node</param>
        /// <returns></returns>
        public abstract bool ConnectNodes(NodeSB<T,R> nodeA,NodeSB<T,R> nodeB);
        /// <summary>
        /// Connect nodes together via their SGNodeID
        /// directional connection from nodeA-->nodeB
        /// </summary>
        /// <param name="nodeA">head node</param>
        /// <param name="nodeB">tail node</param>
        /// <returns></returns>
        public abstract bool ConnectNodes(string nodeA, string nodeB);
        /// <summary>
        /// We need to update the nextNodes list when a node finishes
        /// </summary>
        /// <param name="nodeEvent"></param>
        protected abstract void OnNodeFinished(StateMachineSB<R> nodeEvent);
        /// <summary>
        /// public accessor to try to go to a node
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public abstract bool TryToGoToNode(string nodeID);
        /// <summary>
        /// internal accessor to go to a node
        /// </summary>
        /// <param name="nodeID">nodeID as string</param>
        /// <returns></returns>
        protected abstract (bool,NodeSB<T,R>) GoToNode(string nodeID);
        
        /// <summary>
        /// Return a node by its event
        /// </summary>
        /// <param name="sEvent"></param>
        /// <returns></returns>
        protected abstract NodeSB<T,R> GetNodeByEvent(StateMachineSB<R> sEvent);
    }
}
