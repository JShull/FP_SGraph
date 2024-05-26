namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    // main abstract class for the C# side of the nodes 
    public abstract class SGraphNodeSharpBase<T,R> where T : struct where R : struct
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
        public SGraphStateMachineSharpBase<R> SGStateMachine { get; protected set; }
        public Dictionary<string, SGraphNodeSharpBase<T,R>> Connections { get; protected set; }
        public SGraphNodeSharpBase(string id, SGraphStateMachineSharpBase<R> stateMachine)
        {
            SGNodeID = id;
            SGStateMachine = stateMachine;
            Connections = new Dictionary<string, SGraphNodeSharpBase<T,R>>();
        }
        public SGraphNodeSharpBase(string id)
        {
            SGNodeID = id;
        }
    }
}
