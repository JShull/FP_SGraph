namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    public abstract class SGraphNode<T> 
    {
        public string SGNodeID { get; protected set; }
        public bool IsHeadNode { get; protected set; }
        /// <summary>
        /// Access if we have our Event Requirements met which usually means we can now run the event
        /// </summary>
        public bool RequirementsMet
        {
            get { return SGStateMachine.MeetsRequirements(default(List<T>)); }
        }
        public SGraphStateMachine<T> SGStateMachine { get; protected set; }
        public Dictionary<string, SGraphNode<T>> Connections { get; protected set; }
        public SGraphNode(string id, SGraphStateMachine<T> statemachine)
        {
            SGNodeID = id;
            SGStateMachine = statemachine;
            Connections = new Dictionary<string, SGraphNode<T>>();
        }
    }
}
