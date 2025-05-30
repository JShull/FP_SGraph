namespace FuzzPhyte.SGraph
{   
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    /// <summary>
    /// GameObject to build out an FPEventState
    /// </summary>
    public class FPMonoEvent : MonoBehaviour
    {
        public FPEVManager TheEventManager;
        [Tooltip("Can use this for matching/syncing with other components at a high level")]
        public FP_Data TheMainEventTag;
        //[Obsolete("Use other requirement struct")]
        //public List<RequirementD> TheEventRequirements = new List<RequirementD>();
        //public List<FPSequenceStatusRequirements> TheEventRequirementsData = new List<FPSequenceStatusRequirements>();
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
        protected Dictionary<SequenceStatus,List<FPHelperMapper>> helpers = new Dictionary<SequenceStatus, List<FPHelperMapper>>();
        protected Dictionary<SequenceStatus,string> uniqueNames = new Dictionary<SequenceStatus, string>();
        protected Dictionary<SequenceStatus,float> helpersTimer = new Dictionary<SequenceStatus, float>();
        protected Dictionary<SequenceStatus,bool> useHelpers = new Dictionary<SequenceStatus, bool>();
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
        #region General Setup
        /// <summary>
        /// Call this first if we are setting this up from nothing.
        /// </summary>
        /// <param name="helperMGR"></param>
        public virtual void SetupFromInstantiation(FPHelperTimer helperMGR,FPEVManager eventManager)
        {
            if (helperMGR == null) 
            {
                Debug.LogWarning($"No success on the helper you gave me, trying to see if one's in the scene...");
                HelperManager = FPHelperTimer.HelperTimer;
                if (HelperManager == null)
                {
                    Debug.LogWarning($"Wow really didn't find one, you should probably add one to the scene or something, but let me try one more thing...");
                    GameObject helperObj = new GameObject("FPHelperManager");
                    helperObj.transform.position = Vector3.zero;
                    helperObj.AddComponent<FPHelperTimer>();
                    helperObj.GetComponent<FPHelperTimer>().ResetSetupHelperTimer(null);
                }
            }
            else
            {
                HelperManager = helperMGR;
            }
            if (eventManager == null)
            {
                Debug.LogError($"We really need this to be given to us because there could be different ones in the scene managing different things.");
            }
            else
            {
                TheEventManager = eventManager;
            }
        }
        #endregion
        #region Event Setup Functions
        public virtual void DataResolveAndActivate(SequenceStatus passedStartingState = SequenceStatus.Locked,List<FPTransitionMapper> passedTransitionData = null)
        {
            if (HelperManager == null)
            {
                Debug.LogWarning($"Missing a helper manager, do you need one? If you do, maybe try calling SetupFromInstantiation first?");
            }
            if (passedTransitionData != null)
            {
                StartingState = passedStartingState;
                //use passed data instead of data maybe in the editor
                Debug.LogWarning($"Clearing FPMonoEvent Data and adding in passed data...");
                BuildTransitions(passedTransitionData);
            }
            //build out event requirement data from transition data
            //setup rest of conditions
            TransitionSetup();
            EventStatesSetup();
        }

        /// <summary>
        /// Process This First if we need to via external request
        /// If you pass null in the transitionMapper it won't use any data you pass it
        /// If you pass nothing to it, it will use the data on the Mono Behavior in the Editor to do it's thing and set itself up
        /// </summary>
        /// <param name="passedStartingState">The starting state</param>
        /// <param name="passedTransitionData"></param>
        [Obsolete]
        public virtual void DataResolveAndActivate(SequenceStatus passedStartingState = SequenceStatus.Locked,List<FPTransitionMapper> passedTransitionData = null, List<RequirementD> passedRequirements = null)
        {
            if(HelperManager == null)
            {
                Debug.LogWarning($"Missing a helper manager, do you need one? If you do, maybe try calling SetupFromInstantiation first?");
            }
            if (passedTransitionData != null)
            {
                StartingState = passedStartingState;
                //use passed data instead of data maybe in the editor
                Debug.LogWarning($"Clearing FPMonoEvent Data and adding in passed data...");
                BuildTransitions(passedTransitionData);
            }
            if (passedRequirements != null)
            {
                //TheEventRequirements.Clear();
                //TheEventRequirements.AddRange(passedRequirements);
            }
            //setup rest of conditions
            TransitionSetup();
            EventStatesSetup();
        }
        /// <summary>
        /// Build out the transitions and helpers from the passed data
        /// </summary>
        /// <param name="passedTransitionData"></param>
        protected virtual void BuildTransitions(List<FPTransitionMapper> passedTransitionData)
        {
            TransitionBuilder.Clear();
            TransitionBuilder.AddRange(passedTransitionData);
            for (int i = 0; i < TransitionBuilder.Count; i++)
            {
                if (TransitionBuilder[i].UseHelper)
                {
                    for (int j = 0; j < TransitionBuilder[i].HelperLogic.Count; j++)
                    {
                        var curTransitionHelper = TransitionBuilder[i].HelperLogic[j];

                        GameObject matchedWorldItem = TheEventManager.ReturnFPSelectionBaseItem(curTransitionHelper.TargetObjectData);
                        if (matchedWorldItem != null)
                        {
                            //unity event stored in the data passed
                            //we have a match gameobject in the world and now need to build our event from this logic
                            //we are adding onto a generic Unity Event that we have with nothing on it. We add various listeners to it as needed
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
                                            () => targetComponent.enabled = curTransitionHelper.BoolActionTypeState
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
                if(TransitionBuilder[i].UseHelper && HelperManager !=null)
                {
                    //if we have a helper and it's not already in the dictionary
                    if (!helpers.ContainsKey(TransitionBuilder[i].Outcome))
                    {
                        helpers.Add(TransitionBuilder[i].Outcome, TransitionBuilder[i].HelperLogic);
                        helpersTimer.Add(TransitionBuilder[i].Outcome, TransitionBuilder[i].TimeUntil);
                        uniqueNames.Add(TransitionBuilder[i].Outcome, TransitionBuilder[i].UniqueHelperName);
                        useHelpers.Add(TransitionBuilder[i].Outcome, TransitionBuilder[i].UseHelper);
                    }
                    else
                    {
                        Debug.LogWarning($"Helper for {TransitionBuilder[i].Outcome} already exists in the dictionary");
                    }
                }
            }
        }
        /// <summary>
        /// This will add any additional Unity related references and/or events to the invoked actions
        /// Establishes and adds the state data to the Event Manager and Initializes the event
        /// </summary>
        protected void EventStatesSetup()
        {
            //build out dictionary from TransitionMapper:TransitionBuilder
            Dictionary<SequenceTransition, List<RequirementD>> transitionRequirements = new Dictionary<SequenceTransition, List<RequirementD>>();
            //transition mapper TransitionBuilder
            for(int i = 0; i < TransitionBuilder.Count; i++)
            {
                var curTransitionMapper = TransitionBuilder[i];
                //add the requirements to the list
                if (!transitionRequirements.ContainsKey(curTransitionMapper.TransitionKey))
                {
                    transitionRequirements.Add(curTransitionMapper.TransitionKey, new List<RequirementD>());
                }
                for(int j = 0; j < curTransitionMapper.RequirementData.Count; j++)
                {
                    var curRequirement = curTransitionMapper.RequirementData[j];
                    transitionRequirements[curTransitionMapper.TransitionKey].Add(curRequirement);
                }
            }
            //eventState = new FPEventState(StartingState, TheEventRequirements, transitions, this.gameObject);
            eventState = new FPEventState(StartingState, transitions, transitionRequirements, this.gameObject);
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
            if (helpers.TryGetValue(theEventData.CurrentState, out List<FPHelperMapper> helperData) && HelperManager != null)
            {
                //build out a single time event with a list of actions
                //lets confirm we got a time
                if (!helpersTimer.TryGetValue(theEventData.CurrentState, out float timeUntil))
                {
                    Debug.LogWarning($"No timer found for {theEventData.CurrentState}");
                    return;
                }
                if(!uniqueNames.TryGetValue(theEventData.CurrentState, out string uniqueName))
                {
                    Debug.LogWarning($"No unique name found for {theEventData.CurrentState}");
                    return;
                }
                //build action list for one singular timer
                List<Action> newActions = new List<Action>();
                List<HelperCategory> newCategories = new List<HelperCategory>();
                List<HelperAction> newHelperActions = new List<HelperAction>();
                for (int i=0; i < helperData.Count; i++)
                {
                    var curHelper = helperData[i];
                    newActions.Add(curHelper.ActivateAction);
                    newCategories.Add(curHelper.HelperType);
                    newHelperActions.Add(curHelper.HelperAction);
                }
                var key = (helperData[0].HelperType, theEventData.CurrentState);
                if (!HelperManager.HasRecentlyTriggered(key, timeUntil))
                {
                    HelperManager.StartTimer(timeUntil, newActions, newCategories, eventState, newHelperActions,uniqueName);
                }
                //var key = (helperData.HelperType, theEventData.CurrentState);

                // Only queue if the helper's last trigger time has exceeded the threshold or if it has never been triggered

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
            if (eventState != null)
            {
                eventState.OnFinish -= OnFinishMono;
                eventState.OnActive -= OnActiveMono;
                eventState.OnLocked -= OnLockedMono;
                eventState.OnUnlocked -= OnUnlockedMono;
            }
           
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
            /*
            if (helpers.ContainsKey(eventState.CurrentState) && useHelpers.ContainsKey(eventState.CurrentState) && uniqueNames.ContainsKey(eventState.CurrentState))
            {
                var helperMap = helpers[eventState.CurrentState];
                var useHelper = useHelpers[eventState.CurrentState];
                var uniqueName = uniqueNames[eventState.CurrentState];
                if (useHelper)
                {
                    for(int i = 0; i < helperMap.Count; i++)
                    {
                        var helperMapCurrent = helperMap[i];
                        outcome = eventState.CurrentState.ToString() + " " + uniqueName + " Index " +i;
                        var dataReturn = HelperManager.ContainTimerByUniqueName(uniqueName);
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
                    //figure out time left?
                    //HelperManager.StartTimer
                   
                }
            }
            */
            
#endif
        }
    }
}
