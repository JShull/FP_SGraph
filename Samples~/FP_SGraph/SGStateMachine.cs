namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using System;
    using System.Collections.Generic;

    //Example of a state machine using RequirementD as the requirement type
    public class SGStateMachine : StateMachineSB<RequirementD>
    {
        #region Constructors
       
        public SGStateMachine(SequenceStatus startingState) : base(startingState)
        {
        }

        public SGStateMachine(SequenceStatus startingState, Dictionary<SequenceTransition, SequenceStatus> transitions) : base(startingState, transitions)
        {
        }
        public SGStateMachine(Dictionary<SequenceTransition, List<RequirementD>> requirements):base(requirements)
        { 
        }
        public SGStateMachine(Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<SequenceTransition, List<RequirementD>> requirements): base(transitions, requirements)
        {
        }
        public SGStateMachine(SequenceStatus startingState,Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<SequenceTransition, List<RequirementD>> requirements) : base(startingState,transitions, requirements)
        {
        }
        #endregion

        /// <summary>
        // check if we meet requirements by taking the passedRequirementList and removing them from the unlockRequirements 
        /// </summary>
        /// <param name="passedRequirements">ones to remove from unlockRequirements</param>
        /// <param name="useLockedState">are we using the state machine as a force/limit which if we pass true we want to make sure we aren't in a locked state</param>
        /// <returns></returns>
        public override bool UpdateTransitionCheckRequirementsList(SequenceTransition theTransition,List<RequirementD> passedRequirementList)
        {
            //comparative search to see if we have all of the requirements in the passedRequirementList list
            if(transitionRequirements.Count == 0)
            {
                return true;
            }
            
            //build out our list of requirements to remove
            List<RequirementD> indexToRemove = new List<RequirementD>();
            for (int i = 0; i < transitionRequirements[theTransition].Count; i++)
            {
                var currentUnlockReq = transitionRequirements[theTransition][i];
                for (int j = 0; j < passedRequirementList.Count; j++)
                {
                    var passedParameterRequest = passedRequirementList[j];
                    //string comparison
                    var nameMatch = string.Equals(passedParameterRequest.RequirementName, currentUnlockReq.RequirementName, StringComparison.OrdinalIgnoreCase);
                    if (passedParameterRequest.RequirementMet && nameMatch && (passedParameterRequest.RequirementTag == currentUnlockReq.RequirementTag))
                    {
                        indexToRemove.Add(currentUnlockReq);
                    }
                }
            }
            //remove transition requirements based on the index built list
            for (int a = 0; a < indexToRemove.Count; a++)
            {
                var index = indexToRemove[a];
                RemoveRequirementForTransition(theTransition, index);
            }
            if (transitionRequirements.ContainsKey(theTransition))
            {
                if (transitionRequirements[theTransition].Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            //now actually recheck if we have any requirements left
           
            
        }
       
    }
}
