namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;

    // main abstract class for the C# side of the nodes
    public abstract class NodeSB<T, R> where T : struct where R : struct
    {
        public abstract string SGNodeID { get; set; }
        public abstract bool IsHeadNode { get; set; }
        //public StateMachineSB<R> SGStateMachine { get; set; }
        /// <summary>
        /// Access if we have our Event Requirements met which usually means we can now run the event
        /// </summary>
        public abstract bool RequirementsMet();

        public abstract void SetupNode(string id, Dictionary<T,List<R>> requirements);
        public abstract void SetupNode(string id);
        public abstract void SetupNode(string id, Dictionary<T,List<R>>requirements, List<T> transitions);
        public abstract void SetupStateMachine(Dictionary<T,List<R>>Requirements);
        public abstract void SetupStateMachine(Dictionary<T,List<R>> Requirements, List<T> Transitions);
        public abstract StateMachineSB<R> ReturnStateMachine();
    }
}
