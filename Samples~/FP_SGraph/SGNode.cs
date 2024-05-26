namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using PlasticPipe.PlasticProtocol.Messages;
    using System.Collections;
using System.Collections.Generic;

//main sample class
    public class SGNode : NodeSB<TransitionD, RequirementD>
    {
        public new Dictionary<string, SGNode> Connections;
        private SequenceStatus startState;
        public SequenceStatus StartState { get { return startState; } set { startState = value; } }
        public SGNode(string id, SequenceStatus startState):base(id)
        {
            this.startState = startState;
        }
        public SGNode(string id) : base(id)
        {
        }
        public SGNode(string id, List<RequirementD> requirements) : base(id, requirements)
        {
            
        }
        public SGNode(string id, List<RequirementD> requirements, List<TransitionD> transitions):base(id,requirements,transitions)
        {

        }

        public override void SetupStateMachine(List<RequirementD> Requirements)
        {
            SGStateMachine = new SGStateMachine(Requirements);
        }
        public override void SetupStateMachine(List<RequirementD> Requirements, List<TransitionD> Transitions)
        {
            Dictionary<SequenceTransition, SequenceStatus> newDictionary = new Dictionary<SequenceTransition, SequenceStatus>();
            for (int i = 0; i < Transitions.Count; i++)
            {
                var curTransition = Transitions[i];
                newDictionary.Add(curTransition.Transition, curTransition.OutcomeStatus);
            }
            SGStateMachine = new SGStateMachine(startState, Requirements, newDictionary);
        }
        public void BuildConnectionDictionaryList(List<SGNode>connections)
        {
            Connections = new Dictionary<string, SGNode>();
            for(int i=0;i<connections.Count; i++)
            {
                var possibleConnection = connections[i];
                if(!Connections.ContainsKey(possibleConnection.SGNodeID))
                {
                    Connections.Add(possibleConnection.SGNodeID, possibleConnection);
                }
            }
        }
    }
}
