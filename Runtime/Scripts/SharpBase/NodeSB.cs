namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    // main abstract class for the C# side of the nodes
    public abstract class NodeSB<T, R> where T : struct where R : struct
    {
        public string SGNodeID { get; set; }
        public bool IsHeadNode { get; set; }
        /// <summary>
        /// Access if we have our Event Requirements met which usually means we can now run the event
        /// </summary>
        public bool RequirementsMet
        {
            
            get {
                if (SGStateMachine == null)
                {
                    //never had a statemachine setup so we have no requirements to meet... true/false?
                    return true;
                }
                return SGStateMachine.MeetsRequirements(); 
            }
        }
        public StateMachineSB<R> SGStateMachine { get; set; }
        public Dictionary<string, NodeSB<T, R>> Connections { get; set; }

        public NodeSB(string id, bool isHead)
        {
            SGNodeID = id;
            IsHeadNode = isHead;
        }
        public NodeSB(string id, StateMachineSB<R> stateMachine)
        {
            SGNodeID = id;
            SGStateMachine = stateMachine;
            Connections = new Dictionary<string, NodeSB<T,R>>();
        }
        public NodeSB(string id, List<R> requirements)
        {
            SGNodeID = id;
            SetupStateMachine(requirements);
        }
        public NodeSB(string id)
        {
            SGNodeID = id;
        }
        public NodeSB(string id, List<R>requirements, List<T> transitions)
        {
            SGNodeID = id;
            SetupStateMachine(requirements, transitions);
        }
        public abstract void SetupStateMachine(List<R>Requirements);
        public abstract void SetupStateMachine(List<R> Requirements, List<T> Transitions);
    }
}
