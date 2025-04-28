namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System;
    using UnityEngine.Events;
    using System.Linq;
    using FuzzPhyte.Utility.Attributes;

    #region Data Items
    [Serializable]
    public struct FPSingleEventData
    {
        [Tooltip("Something that can be used to identify the event")]
        [SerializeField, FPNest] string eventName;
        public SequenceStatus StartingEventState;
        public List<FPTransitionMapper> TransitionMapperData;
    }
    [Serializable]
    public struct FPTransitionMapper
    {
        [SerializeField, FPNest] string transitionName;
        [FPNest] public SequenceTransition TransitionKey;
        public SequenceStatus Outcome;
        public List<RequirementD> RequirementData;
        public bool UseHelper;
        public string UniqueHelperName;
        public float TimeUntil;
        public List<FPHelperMapper> HelperLogic;
    }
    [Serializable]
    public struct FPSequenceStatusRequirements
    {
        public SequenceTransition Transition;
        public List<RequirementD> RequirementData;
    }
    
    [Serializable]
    public struct FPHelperMapper
    {
        public string HelperName;
        [FPNest]public HelperCategory HelperType;
        public HelperAction HelperAction;
        //Unique Tag
        public FPHelperFindTag TargetObjectData;
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
        public bool SetupResourcesOnAwake = false;
        protected Dictionary<FPMonoEvent,FPEventState> eventStates = new Dictionary<FPMonoEvent, FPEventState>();
        public Dictionary<FPMonoEvent, FPEventState> EventStates { get { return eventStates; } }
        protected FP_SelectionBase[] sceneSelectionResources;
        protected Dictionary<GameObject,FP_SelectionBase> sceneSelectionResourcesDict = new Dictionary<GameObject, FP_SelectionBase>();
        public Dictionary<GameObject,FP_SelectionBase> SceneSelectionResources { get { return sceneSelectionResourcesDict; } }
        #region Standard Event Functions
        public virtual void Awake()
        {
            if (SetupResourcesOnAwake)
            {
                BuildSceneResources();
            }
        }
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
            var returnValues = eventStates[theEventKey].TryAnyTransition(transition, requirementValue);
            theEventKey.PassBackFromManager(returnValues.Item1,returnValues.Item2);
        }
        public virtual void TriggerEventTransition(FPMonoEvent theEventKey,SequenceTransition transition,RequirementD requirementValue)
        {
            if (!eventStates.ContainsKey(theEventKey))
            {
                Debug.LogWarning("Key not found in the dictionary");
                return;
            }
            var returnValues = eventStates[theEventKey].TryAnyTransition(transition, requirementValue);
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

        /// <summary>
        /// This will look through the scene and find a various set of items by the FPSelectionBase component
        /// Should call this at the beginning of an experience as well as anytime something is added to the scene that contains FP_SelectionBase
        /// </summary>
        public virtual void BuildSceneResources()
        {
            sceneSelectionResources = Resources.FindObjectsOfTypeAll<FP_SelectionBase>();
            if(sceneSelectionResources.Length == 0)
            {
                Debug.LogWarning("No FP_SelectionBase objects found in the scene");
                return;
            }
            foreach (var item in sceneSelectionResources)
            {
                if (!sceneSelectionResourcesDict.ContainsKey(item.gameObject))
                {
                    sceneSelectionResourcesDict.Add(item.gameObject, item);
                }
                else
                {
                    Debug.LogWarning($"Duplicate FP_SelectionBase object found in the scene: {item.name}, you should only have one FP_SelectionBase per gameobject");
                }
            }
        }
        /// <summary>
        /// This will look through the scene and find a various set of items by the FPSelectionBase component
        /// </summary>
        /// <param name="tagToCheck"></param>
        /// <returns></returns>
        public virtual GameObject ReturnFPSelectionBaseItem(FP_Data tagToCheck)
        {
            var foundItem = sceneSelectionResourcesDict.FirstOrDefault(x => x.Value.HelperTag == tagToCheck);
            if (foundItem.Key != null)
            {
                return foundItem.Key;
            }
            else
            {
                Debug.LogWarning($"No FP_SelectionBase object found with the tag: {tagToCheck}");
                return null;
            }
        }
        #endregion
    }
}
