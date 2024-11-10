namespace FuzzPhyte.SGraph
{   
    using System.Collections.Generic;
    using FuzzPhyte.Utility;  
    using UnityEngine;
    using UnityEngine.Events;

    
    /// <summary>
    /// GameObject to build my FPEventState
    /// </summary>
    public class FPMonoEvent : MonoBehaviour
    {
        public FPEVManager TheEventManager;
        public List<RequirementD> TheEventRequirements = new List<RequirementD>();
        [Space]
        [Header("Transitions and Stuff")]
        public SequenceStatus StartingState = SequenceStatus.Locked;
        public List<FPTransitionMapper> TransitionBuilder = new List<FPTransitionMapper>();
        //public List<HelperCategory>
        [Space]
        [Header("Event Actions")]
        public UnityEvent OnFinishEvent;
        public UnityEvent OnActiveEvent;
        public UnityEvent OnLockedEvent;
        public UnityEvent OnUnlockedEvent;
        protected Dictionary<SequenceTransition, SequenceStatus> transitions = new Dictionary<SequenceTransition, SequenceStatus>();
        protected FPEventState eventState;
        protected void Awake()
        {
            for(int i=0;i<TransitionBuilder.Count;i++)
            {
                if(transitions.ContainsKey(TransitionBuilder[i].TransitionKey))
                {
                    Debug.LogWarning("Transition already exists in the dictionary");
                    continue;
                }
                transitions.Add(TransitionBuilder[i].TransitionKey, TransitionBuilder[i].Outcome);
            }
        }
        protected void Start()
        {
            eventState = new FPEventState(StartingState,TheEventRequirements,transitions);
            TheEventManager.AddFPEventStateData(this,eventState);
            eventState.Initialize();
            eventState.OnFinish += OnFinishMono;
            eventState.OnActive += OnActiveMono;
            eventState.OnLocked += OnLockedMono;
            eventState.OnUnlocked += OnUnlockedMono;
        }
        #region Transition Method Calls
        public virtual void TryTransition(SequenceTransition transition, List<RequirementD> requirementValue)
        {
            TheEventManager.TriggerEventTransition(this,transition,requirementValue);
        }
        public virtual void TryForwardTransition()
        {
            var TheState = eventState.CurrentState;
            switch(TheState){
                case SequenceStatus.Locked:
                    //try to unlock
                    TheEventManager.TriggerEventTransition(this,SequenceTransition.LockToUnlock);
                    break;
                case SequenceStatus.Unlocked:
                    TheEventManager.TriggerEventTransition(this,SequenceTransition.UnlockToActive);
                    break;
                case SequenceStatus.Active:
                    TheEventManager.TriggerEventTransition(this,SequenceTransition.ActiveToFinished);
                    break;
            }
        }
        public virtual void TryTransition(SequenceTransition transition)
        {
            switch(eventState.CurrentState)
            {
                case SequenceStatus.Locked:
                    if(transition == SequenceTransition.LockToUnlock)
                    {
                        TheEventManager.TriggerEventTransition(this,transition);
                    }
                    break;
                case SequenceStatus.Active:
                    if(transition == SequenceTransition.ActiveToFinished)
                    {
                        TheEventManager.TriggerEventTransition(this,transition);
                    }
                    break;
                case SequenceStatus.Unlocked:
                    if(transition == SequenceTransition.UnlockToActive)
                    {
                         TheEventManager.TriggerEventTransition(this,transition);
                        break;
                    }
                    if(transition == SequenceTransition.UnlockToLock)
                    {
                         TheEventManager.TriggerEventTransition(this,transition);
                    }
                    break;
            }
        }
        #endregion
        public void PassBackFromManager(bool success, SequenceStatus theStatus)
        {
            Debug.LogWarning($"Manager called back: {success} and {theStatus}");
        }
        public void OnActiveMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Active, the current state? {theEventData.CurrentState}");
            OnActiveEvent.Invoke();

        }
        public void OnLockedMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Locked, the current state? {theEventData.CurrentState}");
            OnLockedEvent.Invoke();
        }
        public void OnUnlockedMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Unlocked, the current state? {theEventData.CurrentState}");
            OnUnlockedEvent.Invoke();
        }
        public void OnFinishMono(StateMachineSB<RequirementD> theEventData)  
        {
            Debug.Log($"Event Finished, the current state? {theEventData.CurrentState}");
            OnFinishEvent.Invoke();
        }
        protected void OnDestroy()
        {
            eventState.OnFinish -= OnFinishMono;
            eventState.OnActive -= OnActiveMono;
            eventState.OnLocked -= OnLockedMono;
            eventState.OnUnlocked -= OnUnlockedMono;
        }
    }
}
