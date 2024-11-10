
namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System;
    [Serializable]
    public struct FPTransitionMapper
    {
        public SequenceTransition TransitionKey;
        public SequenceStatus Outcome;
    }
    public class FPEVManager : MonoBehaviour
    {
        public Dictionary<FPMonoEvent,FPEventState> eventStates;

        private void Awake()
        {
            eventStates = new Dictionary<FPMonoEvent, FPEventState>();
        }
        public void AddFPEventStateData(FPMonoEvent theKey,FPEventState eventState)
        {
            if(eventStates.ContainsKey(theKey))
            {
                Debug.LogWarning("Key already exists in the dictionary");
                return;
            }
            eventStates.Add(theKey,eventState);
            
        }

        private void Update()
        {
            /*
            foreach (var eventState in eventStates)
            {
                eventState.UpdateState();
            }
            */
        }
        public void TriggerEventTransition(FPMonoEvent theEventKey, SequenceTransition transition, List<RequirementD> requirementValue)
        {
            if(!eventStates.ContainsKey(theEventKey))
            {
                Debug.LogWarning("Key not found in the dictionary");
                return;
            }
            var returnValues = eventStates[theEventKey].TryTransition(transition, requirementValue);
            theEventKey.PassBackFromManager(returnValues.Item1,returnValues.Item2);
        }
        public void TriggerEventTransition(FPMonoEvent theEventKey, SequenceTransition transition)
        {
            if(!eventStates.ContainsKey(theEventKey))
            {
                Debug.LogWarning("Key not found in the dictionary");
                return;
            }
            var returnValues = eventStates[theEventKey].TryTransition(transition);
            theEventKey.PassBackFromManager(returnValues.Item1,returnValues.Item2);

        }
    }
}
