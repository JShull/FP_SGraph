namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using UnityEngine;
    public class FPEventState:StateMachineSB<RequirementD>
    {
        protected GameObject UnityRuntimeObject;
        public GameObject UnityObject { get { return UnityRuntimeObject; } }
        public FPEventState(SequenceStatus startingState,GameObject MonoOwner): base(startingState)
        {
            UnityRuntimeObject = MonoOwner;
        }
        public FPEventState(SequenceStatus startingState, Dictionary<SequenceTransition,SequenceStatus>transitions,Dictionary<SequenceTransition,List<RequirementD>> additionalR, GameObject MonoOwner):base(startingState, transitions, additionalR)
        {
            UnityRuntimeObject = MonoOwner;
        }
        /*
        public override bool UpdateUnlockCheckRequirementsList(List<RequirementD> passedParameters)
        {
            if(unlockRequirements.Count == 0)
            {
                return true;
            }
            //
            List<RequirementD> indexToRemove = new List<RequirementD>();
            for(int i = 0; i < unlockRequirements.Count; i++)
            {
                var currentUnlockReq = unlockRequirements[i];
                for(int j = 0; j < passedParameters.Count; j++)
                {
                    var passedParameterRequest = passedParameters[j];
                    //string comparison
                    var nameMatch = string.Equals(passedParameterRequest.RequirementName, currentUnlockReq.RequirementName, StringComparison.OrdinalIgnoreCase);
                    if (passedParameterRequest.RequirementMet && nameMatch && (passedParameterRequest.RequirementTag == currentUnlockReq.RequirementTag))
                    {
                        indexToRemove.Add(currentUnlockReq);
                    }
                }
            }
            for(int a = 0; a < indexToRemove.Count; a++)
            {
                var index = indexToRemove[a];
                unlockRequirements.Remove(index);
            }
            if (unlockRequirements.Count == 0)
            {
                unlockRequirements.Clear();
                return true;
            }
            return false;
        }
        
        public override bool UpdateUnlockCheckRequirement(RequirementD passedParameter)
        {
            if (unlockRequirements.Count == 0)
            {
                return true;
            }
            List<RequirementD> indexToRemove = new List<RequirementD>();
            for (int i = 0; i < unlockRequirements.Count; i++)
            {
                var currentUnlockReq = unlockRequirements[i];
                
                if (passedParameter.RequirementMet && passedParameter.RequirementName == currentUnlockReq.RequirementName)
                {
                    indexToRemove.Add(currentUnlockReq);
                }
            }
            for (int a = 0; a < indexToRemove.Count; a++)
            {
                var index = indexToRemove[a];
                unlockRequirements.Remove(index);
            }
            if (unlockRequirements.Count == 0)
            {
                unlockRequirements.Clear();
                return true;
            }
            return false;
        }
        */
        public override bool UpdateTransitionCheckRequirementsList(SequenceTransition transition, List<RequirementD> parameters)
        {
            
            if(transitionRequirements.Count== 0)
            {
                return true;
            }

            //build out our list of requirements to remove
            List<RequirementD> indexToRemove = new List<RequirementD>();
            for (int i = 0; i < transitionRequirements[transition].Count; i++)
            {
                var currentUnlockReq = transitionRequirements[transition][i];
                for(int j=0;j < parameters.Count; j++)
                {
                    var passedParameterRequest = parameters[j];
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
                RemoveRequirementForTransition(transition, index);
            }
            if (transitionRequirements.ContainsKey(transition))
            {
                if (transitionRequirements[transition].Count > 0)
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
        }
        public virtual void Initialize()
        {
            // Initialize event-specific logic if needed
        }
    }
    public class EventHelperData:HelperData
    {
        public FPEventState EventState { get; set; }
        protected SequenceStatus TheHelperState=SequenceStatus.NA;
        public bool FPEventStateHelperSetup = false;
        public float HelperSetupTime { get; set; } = 0f;
        public EventHelperData()
        {
            FPEventStateHelperSetup = false;
        }
        public EventHelperData(float activationTime, List<Action> activatedActions,List<HelperAction> helpActions = null, List<HelperCategory> helpCategories = null)
        {
            ActivationTime = activationTime;
            if (helpActions == null)
            {
                HelperAction = new List<HelperAction>();
            }
            else
            {
                HelperAction = helpActions;
            }
            if(helpCategories == null)
            {
                Category = new List<HelperCategory>();
            }
            else
            {
                Category = helpCategories;
            }
            onActivate = new List<Action>();
            onActivate.AddRange(activatedActions);
        }
        public EventHelperData(float activationTime, Action activate)
        {
            Category = new List<HelperCategory>();
            Category.Add(HelperCategory.SequenceEvent);
            HelperAction = new List<HelperAction>();
            HelperAction.Add(Utility.HelperAction.NA);
            ActivationTime = activationTime;
            onActivate = new List<Action>();
            onActivate.Add(activate);
        }
        public EventHelperData(FPEventState eventState, float activationTime, Action activate, HelperAction helpAction)
        {
            Category = new List<HelperCategory>();
            Category.Add(HelperCategory.SequenceEvent);
            HelperAction = new List<HelperAction>();
            HelperAction.Add(Utility.HelperAction.NA);
            ActivationTime = activationTime;
            onActivate = new List<Action>();
            onActivate.Add(activate);
            EventState = eventState;
            TheHelperState = eventState.CurrentState;
        }
        public void SetupEventState(FPEventState eventState)
        {
            EventState = eventState;
            TheHelperState = eventState.CurrentState;
            HelperSetupTime = Time.time;
            FPEventStateHelperSetup = true;
        }
        /// <summary>
        /// If we are in the same state as when we were instantiated
        /// </summary>
        /// <returns></returns>
        
        public bool IsHelperStateUnchanged() => TheHelperState == EventState.CurrentState;
    }
}
