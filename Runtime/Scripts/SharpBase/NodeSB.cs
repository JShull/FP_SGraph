namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    // main abstract class for the C# side of the nodes 
    public abstract class NodeSB<T,R> where T : struct where R : struct
    {
        public string SGNodeID { get; protected set; }
        public bool IsHeadNode { get; protected set; }
        /// <summary>
        /// Access if we have our Event Requirements met which usually means we can now run the event
        /// </summary>
        public bool RequirementsMet
        {
            get { return SGStateMachine.MeetsRequirements(default(List<R>)); }
        }
        public StateMachineSB<R> SGStateMachine { get; protected set; }
        public Dictionary<string, NodeSB<T,R>> Connections { get; protected set; }
        public NodeSB(string id, StateMachineSB<R> stateMachine)
        {
            SGNodeID = id;
            SGStateMachine = stateMachine;
            Connections = new Dictionary<string, NodeSB<T,R>>();
        }
        public NodeSB(string id)
        {
            SGNodeID = id;
        }
    }
}
