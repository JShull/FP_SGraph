using FuzzPhyte.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        /// <summary>
        // check if we meet requirements by taking the passedRequirements and removing them from the unlockRequirements 
        /// </summary>
        /// <param name="passedRequirements">ones to remove from unlockRequirements</param>
        /// <param name="useLockedState">are we using the state machine as a force/limit which if we pass true we want to make sure we aren't in a locked state</param>
        /// <returns></returns>
        public override bool MeetsRequirements(List<string> passedRequirements,bool useLockedState)
        {
            if(useLockedState&&CurrentState == SequenceStatus.Locked)
            {
                return false;
            }
            //comparative search to see if we have all of the requirements in the passedRequirements list
            if(unlockRequirements.Count == 0)
            {
                return true;
            }
            List<IRequirement<string>> requirementsToRemove = new List<IRequirement<string>>();
            for (int i = 0; i < unlockRequirements.Count; i++)
            {
                //current unlock requirement
                var curRequirement = unlockRequirements[i];
                for(int j=0; j < passedRequirements.Count; j++)
                {
                    var curPassedRequirement = passedRequirements[j];
                    if (curRequirement.IsSatisfied(curPassedRequirement))
                    {
                        requirementsToRemove.Add(curRequirement);
                        //RemoveUnlockRequirement(curRequirement);
                    }
                }
            }
            //remove them from the list
            for(int i = 0; i < requirementsToRemove.Count; i++)
            {
                RemoveUnlockRequirement(requirementsToRemove[i]);
            }
            if(unlockRequirements.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
