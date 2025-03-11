using FuzzPhyte.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.SGraph
{
    [RequireComponent(typeof(FPMonoEvent))]
    public class FPMonoEventTester : MonoBehaviour
    {
        public bool KeyBoardActiveTesting;
        public FPEVManager TheEventManager;
        protected FPMonoEvent eventRef;
        [Header("Testing Purposes")]
        public List<RequirementD> TestRequirements = new List<RequirementD>();
        public SequenceTransition TestTransition;

        [Space]
        [Header("Build From Data Test")]
        public bool TestFromData = false;
        public SequenceStatus StartingEventState = SequenceStatus.Locked;
        public List<FPTransitionMapper> fakeTestData = new List<FPTransitionMapper>();
        public List<RequirementD> fakeRequirementData = new List<RequirementD>();
        public string CurrentStateName;
        private void Awake()
        {
            //need to wait for the FPEVManager to initialize states so we shouldn't run anything here...
        }
        private void Start()
        {
            eventRef = this.GetComponent<FPMonoEvent>();
            if (TestFromData)
            {
                eventRef.DataResolveAndActivate(StartingEventState, fakeTestData, fakeRequirementData);
            }
        }
        protected virtual void Update()
        {
            if (!KeyBoardActiveTesting) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryForwardTransition();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                TryTransition(TestTransition, TestRequirements);
            }
            CurrentStateName = eventRef.EventState.CurrentState.ToString();
        }
        public virtual void TryTransition(SequenceTransition transition, List<RequirementD> requirementValue)
        {
            TheEventManager.TriggerEventTransition(eventRef, transition, requirementValue);
        }
        public virtual void TryForwardTransition()
        {
            var TheState = eventRef.EventState.CurrentState;
            switch (TheState)
            {
                case SequenceStatus.Locked:
                    //try to unlock
                    TheEventManager.TriggerEventTransition(eventRef, SequenceTransition.LockToUnlock);
                    break;
                case SequenceStatus.Unlocked:
                    TheEventManager.TriggerEventTransition(eventRef, SequenceTransition.UnlockToActive);
                    break;
                case SequenceStatus.Active:
                    TheEventManager.TriggerEventTransition(eventRef, SequenceTransition.ActiveToFinished);
                    break;
            }
        }
    }
}
