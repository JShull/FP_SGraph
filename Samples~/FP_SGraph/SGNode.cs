namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using System.Linq;
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
        public override void SetupNode(string id, Dictionary<TransitionD,List<RequirementD>> requirements)
        {
            SGNodeID = id;
            SetupStateMachine(requirements);
        }
        public override void SetupNode(string id, Dictionary<TransitionD,List<RequirementD>> requirements, List<TransitionD> transitions)
        {
            SGNodeID = id;
            SetupStateMachine(requirements, transitions);
        }
        public override void SetupStateMachine(Dictionary<TransitionD,List<RequirementD>> Requirements)
        {
            Dictionary<SequenceTransition,List<RequirementD>> newStateMachineRequirements = new Dictionary<SequenceTransition, List<RequirementD>>();
            var requirementKeys = Requirements.Keys.ToList();
            for (int i = 0; i < requirementKeys.Count; i++)
            {
                var curRequirementKey = requirementKeys[i];
                var curRequirementList = Requirements[curRequirementKey];
                if (!newStateMachineRequirements.ContainsKey(curRequirementKey.Transition))
                {
                    newStateMachineRequirements.Add(curRequirementKey.Transition, new List<RequirementD>());
                }

                newStateMachineRequirements[curRequirementKey.Transition].AddRange(curRequirementList);
            }
           
            SGStateMachine = new SGStateMachine(newStateMachineRequirements);
        }
        public override void SetupStateMachine(Dictionary<TransitionD, List<RequirementD>> Requirements, List<TransitionD> Transitions)
        {
            Dictionary<SequenceTransition, SequenceStatus> newDictionary = new Dictionary<SequenceTransition, SequenceStatus>();
            for (int i = 0; i < Transitions.Count; i++)
            {
                var curTransition = Transitions[i];
                newDictionary.Add(curTransition.Transition, curTransition.OutcomeStatus);
            }
            Dictionary<SequenceTransition, List<RequirementD>> newStateMachineRequirements = new Dictionary<SequenceTransition, List<RequirementD>>();
            var requirementKeys = Requirements.Keys.ToList();
            for (int i = 0; i < requirementKeys.Count; i++)
            {
                var curRequirementKey = requirementKeys[i];
                var curRequirementList = Requirements[curRequirementKey];
                if (!newStateMachineRequirements.ContainsKey(curRequirementKey.Transition))
                {
                    newStateMachineRequirements.Add(curRequirementKey.Transition, new List<RequirementD>());
                }

                newStateMachineRequirements[curRequirementKey.Transition].AddRange(curRequirementList);
            }
            SGStateMachine = new SGStateMachine(startState, newDictionary,newStateMachineRequirements);
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
