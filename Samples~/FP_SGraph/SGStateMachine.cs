using FuzzPhyte.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FuzzPhyte.SGraph.Samples
{
    //Example of a state machine using RequirementD as the requirement type
    public class SGStateMachine : StateMachineSB<RequirementD>
    {
        #region Constructors
        public SGStateMachine(List<RequirementD> requirements) : base(requirements)
        {

        }
        public SGStateMachine(SequenceStatus startingState) : base(startingState)
        {
        }

        public SGStateMachine(SequenceStatus startingState, List<RequirementD> requirements) : base(startingState, requirements)
        {
        }

        public SGStateMachine(SequenceStatus startingState, List<RequirementD> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions) : base(startingState, requirements, transitions)
        {
        }

        public SGStateMachine(SequenceStatus startingState, List<RequirementD> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<int, List<Action>> actions) : base(startingState, requirements, transitions, actions)
        {
        }


        #endregion

        /// <summary>
        // check if we meet requirements by taking the passedRequirementList and removing them from the unlockRequirements 
        /// </summary>
        /// <param name="passedRequirements">ones to remove from unlockRequirements</param>
        /// <param name="useLockedState">are we using the state machine as a force/limit which if we pass true we want to make sure we aren't in a locked state</param>
        /// <returns></returns>
        public override bool UpdateUnlockCheckRequirementsList(List<RequirementD> passedRequirementList)
        {
            //comparative search to see if we have all of the requirements in the passedRequirementList list
            if(unlockRequirements.Count == 0)
            {
                return true;
            }
            List<RequirementD> requirementsToRemove = new List<RequirementD>();
            for (int i = 0; i < unlockRequirements.Count; i++)
            {
                //current unlock requirement
                var curRequirement = unlockRequirements[i];

                for(int j=0; j < passedRequirementList.Count; j++)
                {
                    var curPassedRequirement = passedRequirementList[j];
                    //do they match?
                    if(curRequirement.RequirementName == curPassedRequirement.RequirementName)
                    {
                        curRequirement.RequirementMet = true;
                        requirementsToRemove.Add(curRequirement);
                        break;
                    }
                }
            }
            //remove them from the list - thus updating our actual system
            for(int i = 0; i < requirementsToRemove.Count; i++)
            {
                RemoveUnlockRequirement(requirementsToRemove[i]);
            }
            //now actually recheck if we have any requirements left
            return MeetsRequirements();
            
        }
        public override bool UpdateUnlockCheckRequirement(RequirementD parameter)
        {
            throw new NotImplementedException();
        }
    }
}
