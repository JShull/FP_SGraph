namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using PlasticPipe.PlasticProtocol.Messages;
    using System.Collections;
using System.Collections.Generic;

//main sample class
    public class SGNode : NodeSB<TransitionD, RequirementD>
    {
        public Dictionary<string, SGNode> Connections;
        public SGStateMachine SGStateMachine;
        private SequenceStatus startState;
        public SequenceStatus StartState { get { return startState; } set { startState = value; } }

        private string nodeID;
        public override string SGNodeID { get => nodeID; set => nodeID=value; }
        public override bool IsHeadNode { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        //public override StateMachineSB<RequirementD> SGStateMachine { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public SGNode(string id)
        {
            SetupNode(id);
        }
        
        public override StateMachineSB<RequirementD> ReturnStateMachine()
        {
            return SGStateMachine;
        }
        public override void SetupNode(string id)
        {
            SGNodeID = id;
        }
        public override void SetupNode(string id, List<RequirementD> requirements)
        {
            SGNodeID = id;
            SetupStateMachine(requirements);
        }
        public override void SetupNode(string id, List<RequirementD> requirements, List<TransitionD> transitions)
        {
            SGNodeID = id;
            SetupStateMachine(requirements, transitions);
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
        public void BuildConnectionDictionaryList()
        {
            Connections = new Dictionary<string, SGNode>();
        }

        public override bool RequirementsMet()
        {
            if (SGStateMachine == null)
            {
                //never had a statemachine setup so we have no requirements to meet... true/false?
                return true;
            }
            return SGStateMachine.MeetsRequirements(); 
            
        }
       
    }
}
