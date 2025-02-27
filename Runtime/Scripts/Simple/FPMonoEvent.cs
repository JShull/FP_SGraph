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
        [SerializeField]
        protected FPEventState eventState;
        [SerializeField]
        protected string curStateName;
        protected bool eventStateEst;
        protected virtual void Awake()
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
        protected virtual void Start()
        {
            eventState = new FPEventState(StartingState,TheEventRequirements,transitions,this.gameObject);
            TheEventManager.AddFPEventStateData(this,eventState);
            eventState.Initialize();
            eventState.OnFinish += OnFinishMono;
            eventState.OnActive += OnActiveMono;
            eventState.OnLocked += OnLockedMono;
            eventState.OnUnlocked += OnUnlockedMono;
            eventStateEst = true;
        }
        protected virtual void Update()
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
            curStateName = eventState.CurrentState.ToString();
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
                    HelperManager.StartTimer(helperData.TimeUntil, helperData.ActivateAction, helperData.HelperType, eventState,helperData.UniqueHelperName);
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

        private void OnDrawGizmosSelected()
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
            
            /*
            for (int j = 0; j < TheEventRequirements.Count; j++)
            {

                if (TheEventRequirements[j].RequirementTag != null)
                {
                    if (ConnectedNodes[j] != null)
                    {
                        Vector3 nextS = ConnectedNodes[j].transform.position;
                        //endPoints.Add(nextS);
                        Vector3 startTan = new Vector3(centerP.x, centerP.y + 1 + (j * 2f), centerP.z);

                        Vector3 forwardV = (nextS - startTan).normalized;
                        Color fromColor = FP_UtilityData.ReturnColorByStatus(SharpData.StartState);
                        UnityEditor.Handles.DrawBezier(centerP, nextS - (forwardV * 0.25f), startTan, nextS, fromColor, null, 2f);

                        UnityEditor.Handles.color = fromColor;
                        UnityEditor.Handles.DrawSolidDisc(nextS - (forwardV * 0.25f), forwardV, 0.25f);
                    }

                }
            }
            */

#endif
        }
        /// <summary>
        /// Help with debugging sequences
        /// </summary>
        private void OnDrawGizmos()
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
