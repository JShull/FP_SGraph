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
        public KeyCode TestForwardTransition = KeyCode.Space;
        public KeyCode TestRequirementsTransition = KeyCode.M;
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
        protected virtual void Awake()
        {
            //need to wait for the FPEVManager to initialize states so we shouldn't run anything here...
        }
        protected virtual void Start()
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

            if (Input.GetKeyDown(TestForwardTransition))
            {
                TryForwardTransition();
            }
            if (Input.GetKeyDown(TestRequirementsTransition))
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
