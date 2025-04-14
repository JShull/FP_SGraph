namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System;
    using UnityEngine.Events;

    #region Data Items
    [Serializable]
    public struct FPTransitionMapper
    {
        public SequenceTransition TransitionKey;
        public SequenceStatus Outcome;
        public FPHelperMapper HelperLogic;
    }
    [Serializable]
    public struct FPHelperMapper
    {
        public bool UseHelper;
        public string UniqueHelperName;
        public HelperCategory HelperType;
        public HelperAction HelperAction;
        public float TimeUntil;
        //Unique Tag
        public FP_Data TargetObjectData;
        public FPEventActionType ActionType;
        [Tooltip("Used for state change on enabled/disabled items")]
        public bool BoolActionTypeState;
        [Tooltip("Used for method name and/or other lookup references for a component type")]
        public string CustomString_NameAction;
        public UnityEvent TheHelperAction;
        public void ActivateAction()
        {
            TheHelperAction.Invoke();
        }
    }
    [Serializable]
    public enum FPEventActionType
    {
        //Do Nothing
        NA=0,
        //SetActive(true)/SetActive(false)
        SetActive = 1,
        //enabled = true / enabled = false
        ComponentActive = 2,
        PlayAnimationTrigger = 8,
        CustomMethod = 9,
    }
    #endregion
    public class FPEVManager : MonoBehaviour
    {
        protected Dictionary<FPMonoEvent,FPEventState> eventStates = new Dictionary<FPMonoEvent, FPEventState>();
        public Dictionary<FPMonoEvent, FPEventState> EventStates { get { return eventStates; } }
        #region Standard Event Functions

        public virtual void AddFPEventStateData(FPMonoEvent theKey,FPEventState eventState)
        {
            if(eventStates.ContainsKey(theKey))
            {
                Debug.LogWarning("Key already exists in the dictionary");
                return;
            }
            eventStates.Add(theKey,eventState);
            
        }
        public virtual void DeleteFPEventStateData(FPMonoEvent theKey, FPEventState eventState)
        {
            if (eventStates.ContainsKey(theKey))
            {
                eventStates.Remove(theKey);
            }
            else
            {
                Debug.LogWarning($"Key,{theKey.name} doesn't exist in the dictionary");
            }
        }
        /// <summary>
        /// Method for us to invoke after we have setup our event and we need to fire off the immediate Unity Action associated with it
        /// </summary>
        /// <param name="theEventKey"></param>
        public virtual void TriggerEventStateSetup(FPMonoEvent theEventKey)
        {
            if (!eventStates.ContainsKey(theEventKey))
            {
                Debug.LogWarning("Key not found in the dictionary");
                return;
            }
            var initTrue = eventStates[theEventKey].TryInvokeEventInitialization();
            var curState = eventStates[theEventKey].CurrentState;
            theEventKey.PassBackFromManager(initTrue, curState);
        }
        public virtual void TriggerEventTransition(FPMonoEvent theEventKey, SequenceTransition transition, List<RequirementD> requirementValue)
        {
            if(!eventStates.ContainsKey(theEventKey))
            {
                Debug.LogWarning("Key not found in the dictionary");
                return;
            }
            var returnValues = eventStates[theEventKey].TryTransition(transition, requirementValue);
            theEventKey.PassBackFromManager(returnValues.Item1,returnValues.Item2);
        }
        public virtual void TriggerEventTransition(FPMonoEvent theEventKey, SequenceTransition transition)
        {
            if(!eventStates.ContainsKey(theEventKey))
            {
                Debug.LogWarning("Key not found in the dictionary");
                return;
            }
            var returnValues = eventStates[theEventKey].TryTransition(transition);
            theEventKey.PassBackFromManager(returnValues.Item1,returnValues.Item2);
        }    
        #endregion
    }
}
