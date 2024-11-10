namespace FuzzPhyte.SGraph
{
    using System;
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    public class FPEventState:StateMachineSB<RequirementD>
    {
        public FPEventState(List<RequirementD> requirements) : base(requirements) {}
        public FPEventState(SequenceStatus startingState, List<RequirementD> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions) : base(startingState,requirements, transitions) {}

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
        public EventHelperData()
        {

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
        }
    }
}
