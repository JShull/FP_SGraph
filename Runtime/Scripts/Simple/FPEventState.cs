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
        public FPEventState(List<RequirementD> requirements) : base(requirements) {}
        public FPEventState(SequenceStatus startingState, List<RequirementD> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions, GameObject MonoOwner) : base(startingState,requirements, transitions) 
        {
            UnityRuntimeObject = MonoOwner;
        }
        public override bool UpdateUnlockCheckRequirementsList(List<RequirementD> parameters)
        {
            if(unlockRequirements.Count == 0)
            {
                return true;
            }
            foreach (var requirement in unlockRequirements)
            {
                if (!parameters.Contains(requirement))
                {
                    return false;
                }
            }
            unlockRequirements.Clear();
            return true;
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
        public EventHelperData(float activationTime, Action activate)
        {
            Category = HelperCategory.SequenceEvent;
            ActivationTime = activationTime;
            onActivate = activate;
        }
        public EventHelperData(FPEventState eventState, float activationTime, Action activate)
        {
            Category = HelperCategory.SequenceEvent;
            ActivationTime = activationTime;
            onActivate = activate;
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
