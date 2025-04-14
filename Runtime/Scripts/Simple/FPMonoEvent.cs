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
        [Tooltip("This will run the first event action tied to the starting state")]
        public bool ProcessEventActionsOnSetup;
        [Space]
        [Header("Transitions and Stuff")]
        public SequenceStatus StartingState = SequenceStatus.Locked;
        public List<FPTransitionMapper> TransitionBuilder = new List<FPTransitionMapper>();

        [Tooltip("If you wanted to listen in and subscribe to some events externally")]
        public delegate void FPMonoEventDelegate(FPMonoEvent theEvent);
        public event FPMonoEventDelegate OnFPMonoEventLocked;
        public event FPMonoEventDelegate OnFPMonoEventUnlocked;
        public event FPMonoEventDelegate OnFPMonoEventActivated;
        public event FPMonoEventDelegate OnFPMonoEventFinished;
        
        [Space]
        [Header("Event Actions")]
        public UnityEvent OnLockedEvent;
        public UnityEvent OnUnlockedEvent;
        public UnityEvent OnActiveEvent;
        public UnityEvent OnFinishEvent;
        protected Dictionary<SequenceTransition, SequenceStatus> transitions = new Dictionary<SequenceTransition, SequenceStatus>();
        protected Dictionary<SequenceStatus,FPHelperMapper> helpers = new Dictionary<SequenceStatus, FPHelperMapper>();
        [Tooltip("If we want to use the helpers we need a timer")]
        public FPHelperTimer HelperManager;
        [SerializeField]
        protected FPEventState eventState;
        
        public FPEventState EventState
        {
            get { return eventState; }
        }
        protected bool eventStateEst;
        protected virtual void Awake()
        {
        }
        protected virtual void Start()
        {
        }
        #region Event Setup Functions
        /// <summary>
        /// Process This First if we need to via external request
        /// If you pass null in the transitionMapper it won't use any data you pass it
        /// If you pass nothing to it, it will use the data on the Mono Behavior in the Editor to do it's thing and set itself up
        /// </summary>
        /// <param name="passedStartingState">The starting state</param>
        /// <param name="passedTransitionData"></param>
        public virtual void DataResolveAndActivate(SequenceStatus passedStartingState = SequenceStatus.Locked,List<FPTransitionMapper> passedTransitionData = null, List<RequirementD> passedRequirements = null)
        {
            if (passedTransitionData != null)
            {
                StartingState = passedStartingState;
                //use passed data instead of data maybe in the editor
                Debug.LogWarning($"Clearing FPMonoEvent Data and adding in passed data...");
                TransitionBuilder.Clear();
                //move passedTransitionData into TransitionBuilder as a new block of data not a reference
                TransitionBuilder.AddRange(passedTransitionData);
                //find all possible gameobject targets 
                var targets = Resources.FindObjectsOfTypeAll<FP_SelectionBase>();
                //var targets = GameObject.FindObjectsByType<FP_SelectionBase>(FindObjectsSortMode.None);
                Debug.LogWarning($"Found some possible Selection Targets: {targets.Length}");
                //build out UnityEvents now and inject them
                for (int i = 0; i < TransitionBuilder.Count; i++)
                {
                    var curTransitionHelper = TransitionBuilder[i].HelperLogic;
                    if (curTransitionHelper.UseHelper)
                    {
                        //find a match in my targets
                        GameObject matchedWorldItem = null;
                        for(int j = 0; j < targets.Length; j++)
                        {
                            var aPossibleMatch = targets[j].MainFPTag;
                           
                            if (aPossibleMatch == curTransitionHelper.TargetObjectData)
                            {
                                //MATCH
                                matchedWorldItem = targets[j].gameObject;
                                break;
                            }
                        }

                        if (matchedWorldItem != null)
                        {
                            //unity event stored in the data passed
                            //we have a match gameobject in the world and now need to build our event from this logic
                            switch (curTransitionHelper.ActionType)
                            {
                                case FPEventActionType.NA:
                                   
                                    break;
                                case FPEventActionType.SetActive:
                                    curTransitionHelper.TheHelperAction.AddListener(
                                        () => matchedWorldItem.SetActive(curTransitionHelper.BoolActionTypeState)
                                        );
                                    break;
                                case FPEventActionType.ComponentActive:
                                    //use custom string name for component look up
                                    
                                    var targetComponent = matchedWorldItem.GetComponent(curTransitionHelper.CustomString_NameAction) as Behaviour;
                                    //if we find the target component
                                    if (targetComponent)
                                    {
                                        curTransitionHelper.TheHelperAction.AddListener(
                                            ()=> targetComponent.enabled = curTransitionHelper.BoolActionTypeState
                                            );
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"Component '{curTransitionHelper.CustomString_NameAction}' not found on '{matchedWorldItem.name}'.");
                                    }
                                    break;
                                case FPEventActionType.PlayAnimationTrigger:
                                    //find the animator!
                                    Animator animator = matchedWorldItem.GetComponent<Animator>();
                                    //we found one?
                                    if (animator)
                                    {
                                        curTransitionHelper.TheHelperAction.AddListener(
                                            () => animator.SetTrigger(curTransitionHelper.CustomString_NameAction)
                                            );
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"GameObject, {matchedWorldItem.name}, didn't have an animator on it for the animation named {curTransitionHelper.CustomString_NameAction}");
                                    }
                                        break;
                                case FPEventActionType.CustomMethod:
                                    break;
                            }
                        }
                    }
                }
                
            }
            if (passedRequirements != null)
            {
                TheEventRequirements.Clear();
                TheEventRequirements.AddRange(passedRequirements);
            }
            //setup rest of conditions
            TransitionSetup();
            EventStatesSetup();
        }
        /// <summary>
        /// We have requirement updates we need to override
        /// </summary>
        /// <param name="passedStartingState"></param>
        /// <param name="passedRequirements"></param>
        public virtual void DataResolveAndActivate(SequenceStatus passedStartingState, List<RequirementD> passedRequirements)
        {
            if (passedRequirements != null)
            {
                TheEventRequirements.Clear();
                TheEventRequirements.AddRange(passedRequirements);
            }
            DataResolveAndActivate(passedStartingState);
        }
        /// <summary>
        /// Will setup the event with the monobehavior editor data and override the starting state you pass in
        /// </summary>
        /// <param name="passedStartingState"></param>
        public virtual void DataResolveAndActivate(SequenceStatus passedStartingState)
        {
            StartingState = passedStartingState;
            TransitionSetup();
            EventStatesSetup();
        }
        
        /// <summary>
        /// Uses the transitionBuilder to generate our transitions and sets the helpers up from the data
        /// </summary>
        protected void TransitionSetup()
        {
            for (int i = 0; i < TransitionBuilder.Count; i++)
            {
                if (transitions.ContainsKey(TransitionBuilder[i].TransitionKey))
                {
                    Debug.LogWarning("Transition already exists in the dictionary");
                    continue;
                }
                transitions.Add(TransitionBuilder[i].TransitionKey, TransitionBuilder[i].Outcome);
                if (TransitionBuilder[i].HelperLogic.UseHelper && HelperManager != null)
                {
                    helpers.Add(TransitionBuilder[i].Outcome, TransitionBuilder[i].HelperLogic);
                }
            }
        }
        /// <summary>
        /// This will add any additional Unity related references and/or events to the invoked actions
        /// Establishes and adds the state data to the Event Manager and Initializes the event
        /// </summary>
        protected void EventStatesSetup()
        {
            eventState = new FPEventState(StartingState, TheEventRequirements, transitions, this.gameObject);
            TheEventManager.AddFPEventStateData(this, eventState);
            eventState.Initialize();
            eventState.OnFinish += OnFinishMono;
            eventState.OnActive += OnActiveMono;
            eventState.OnLocked += OnLockedMono;
            eventState.OnUnlocked += OnUnlockedMono;
            eventStateEst = true;
            //process current state?
            if (ProcessEventActionsOnSetup)
            {
                TheEventManager.TriggerEventStateSetup(this);
            }
        }
        
        #endregion
        
        public virtual void PassBackFromManager(bool success, SequenceStatus theStatus)
        {
            Debug.LogWarning($"Manager called back: {success} and {theStatus}");
        }
        public virtual void OnActiveMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Active, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnFPMonoEventActivated?.Invoke(this);
            OnActiveEvent.Invoke();
        }
        public virtual void OnLockedMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Locked, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnFPMonoEventLocked?.Invoke(this);
            OnLockedEvent.Invoke();
        }
        public virtual void OnUnlockedMono(StateMachineSB<RequirementD> theEventData)
        {
            Debug.Log($"Event Unlocked, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnFPMonoEventUnlocked?.Invoke(this);
            OnUnlockedEvent.Invoke();
        }
        public virtual void OnFinishMono(StateMachineSB<RequirementD> theEventData)  
        {
            Debug.Log($"Event Finished, the current state? {theEventData.CurrentState}");
            CheckRunHelper(theEventData);
            OnFPMonoEventFinished?.Invoke(this);
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
                    HelperManager.StartTimer(helperData.TimeUntil, helperData.ActivateAction, helperData.HelperType, eventState,helperData.UniqueHelperName,helperData.HelperAction);
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

        protected virtual void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (TheEventManager==null)
            {
                return;
            }
            Vector3 centerP = transform.position;
            // List<Vector3> endPoints = new List<Vector3>();
            Color curColor = Color.black;
            if (eventStateEst)
            {
                curColor = FP_UtilityData.ReturnColorByStatus(eventState.CurrentState);
            }

            Vector3 nextS = TheEventManager.transform.position;
            Vector3 startTan = new Vector3(centerP.x, centerP.y + 1, centerP.z);

            Vector3 forwardV = (nextS - startTan).normalized;
            UnityEditor.Handles.DrawBezier(centerP, nextS - (forwardV * 0.25f), startTan, nextS, curColor, null, 2f);

            UnityEditor.Handles.color = curColor;
            UnityEditor.Handles.ConeHandleCap(0, nextS - (forwardV * 0.25f), Quaternion.LookRotation(forwardV), 0.25f, EventType.Repaint);
     

#endif
        }
        /// <summary>
        /// Help with debugging sequences
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (TheEventManager == null)
            {
                Gizmos.DrawIcon(transform.position, FP_UtilityData.ReturnIconAddressByStatus(SequenceStatus.NA), true);
                return;
            }
            if (!eventStateEst)
            {
                Gizmos.DrawIcon(transform.position, FP_UtilityData.ReturnIconAddressByStatus(SequenceStatus.None), true);
                return;
            }
            Gizmos.DrawIcon(transform.position, FP_UtilityData.ReturnIconAddressByStatus(eventState.CurrentState), true);
            
            //helper timer?
            
            //sequencestatus
            string outcome = eventState.CurrentState.ToString();
            if (helpers.ContainsKey(eventState.CurrentState))
            {
                var helperMap = helpers[eventState.CurrentState];
                if (helperMap.UseHelper)
                {
                    //figure out time left?
                    //HelperManager.StartTimer
                    outcome = eventState.CurrentState.ToString() + " " + helperMap.UniqueHelperName;
                    var dataReturn = HelperManager.ContainTimerByUniqueName(helperMap.UniqueHelperName);
                    Vector3 labelPos = this.transform.position + new Vector3(0, .2f, 0);
                    if (dataReturn.Item2 != null)
                    {
                        var helperRunning = HelperManager.TimerActiveByUniqueName(helperMap.UniqueHelperName);
                        if (helperRunning.Item1)
                        {
                            float runTimeLeft = helperRunning.Item2.ActivationTime - Time.time;
                            UnityEditor.Handles.Label(labelPos, outcome + " " + (runTimeLeft).ToString("0.00"));
                            if (runTimeLeft < 0)
                            {
                                UnityEditor.Handles.Label(labelPos, outcome);
                            }
                        }
                        else
                        { 
                            UnityEditor.Handles.Label(labelPos, outcome + " " + (dataReturn.Item2.ActivationTime).ToString("0.00"));
                        }
                        
                    }
                    else
                    {
                        UnityEditor.Handles.Label(labelPos, outcome);
                    }
                }
            }
            
#endif
        }
    }
}
