namespace FuzzPhyte.SGraph
{   
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using UnityEngine;
    using UnityEngine.Events;

    
    /// <summary>
    /// GameObject to build out an FPEventState
    /// </summary>
    public class FPMonoEvent : MonoBehaviour
    {
        public FPEVManager TheEventManager;
        public List<RequirementD> TheEventRequirements = new List<RequirementD>();
        [Header("Testing Purposes")]
        public List<RequirementD> TestRequirements = new List<RequirementD>();
        public SequenceTransition TestTransition;
        public bool Testing=true;
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
        protected Dictionary<SequenceStatus,FPHelperMapper> helpers = new Dictionary<SequenceStatus, FPHelperMapper>();
        [Tooltip("If we want to use the helpers we need a timer")]
        public FPHelperTimer HelperManager;
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
                if(TransitionBuilder[i].HelperLogic.UseHelper && HelperManager != null)
                {
                    helpers.Add(TransitionBuilder[i].Outcome, TransitionBuilder[i].HelperLogic);
                }
            }
        }
        protected void Start()
        {
            eventState = new FPEventState(StartingState,TheEventRequirements,transitions,this.gameObject);
            TheEventManager.AddFPEventStateData(this,eventState);
            eventState.Initialize();
            eventState.OnFinish += OnFinishMono;
            eventState.OnActive += OnActiveMono;
            eventState.OnLocked += OnLockedMono;
            eventState.OnUnlocked += OnUnlockedMono;
        }
        protected void Update()
        {
            if(!Testing)return;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                TryForwardTransition();
            }
            if(Input.GetKeyDown(KeyCode.M))
            {
                TryTransition(TestTransition,TestRequirements);
            }
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
            CheckRunHelper(theEventData);
            OnActiveEvent.Invoke();
        }
        public void OnLockedMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Locked, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnLockedEvent.Invoke();
        }
        public void OnUnlockedMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Unlocked, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnUnlockedEvent.Invoke();
        }
        public void OnFinishMono(StateMachineSB<RequirementD> theEventData)  
        {
            Debug.Log($"Event Finished, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnFinishEvent.Invoke();
        }
        protected virtual void CheckRunHelper(StateMachineSB<RequirementD> theEventData)
        {
            //
            if (helpers.TryGetValue(theEventData.CurrentState, out FPHelperMapper helperData) && HelperManager != null)
            {
                var key = (helperData.HelperType, theEventData.CurrentState);

                // Only queue if the helper's last trigger time has exceeded the threshold or if it has never been triggered
                if (!HelperManager.HasRecentlyTriggered(key, helperData.TimeUntil))
                {
                    HelperManager.StartTimer(helperData.TimeUntil, helperData.ActivateAction, helperData.HelperType, eventState);
                }
            }
            /*
            if(helpers.ContainsKey(theEventData.CurrentState))
            {
                var helperData = helpers[theEventData.CurrentState];
                HelperManager.StartTimer(helperData.TimeUntil,helperData.ActivateAction,helperData.HelperType,eventState);
            }*/
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
