using FuzzPhyte.Utility;
using System;
using System.Collections.Generic;

namespace FuzzPhyte.SGraph.Samples
{
    public class SGraphStateMachineData : SGraphStateMachine<string>
    {
        #region Constructors
        public SGraphStateMachineData(SequenceStatus startingState) : base(startingState)
        {
        }

        public SGraphStateMachineData(SequenceStatus startingState, List<IRequirement<string>> requirements) : base(startingState, requirements)
        {
        }

        public SGraphStateMachineData(SequenceStatus startingState, List<IRequirement<string>> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions) : base(startingState, requirements, transitions)
        {
        }

        public SGraphStateMachineData(SequenceStatus startingState, List<IRequirement<string>> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<SequenceStatus, List<Action>> actions) : base(startingState, requirements, transitions, actions)
        {
        }
        #endregion
        public override bool MeetsRequirements()
        {
            if(CurrentState == SequenceStatus.Locked)
            {
                return false;
            }
            return true;
            
        }
    }
}
