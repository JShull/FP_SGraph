using FuzzPhyte.Utility;
using System.Collections.Generic;
using UnityEngine;
using FuzzPhyte.Utility.Attributes;
namespace FuzzPhyte.SGraph
{
    [RequireComponent(typeof(FPMonoEvent))]
    public class FPMonoEventTester : MonoBehaviour
    {
        public bool KeyBoardActiveTesting;
        public FPEVManager TheEventManager;
        protected FPMonoEvent eventRef;
        

        public KeyCode TestForwardTransition = KeyCode.Space;
        [Space]
        [Header("Testing Purposes")]
        public KeyCode TestRequirementsUnlockTransition = KeyCode.U;
        public SequenceTransition TransitionToUnlockRequirements = SequenceTransition.LockToUnlock;
        public List<RequirementD> TestUnlockRequirements = new List<RequirementD>();
        [Space]
        public KeyCode TestRequirementsFinishedTransition = KeyCode.F;
        public SequenceTransition TransitionToFinishRequirements = SequenceTransition.ActiveToFinished;
        public List<RequirementD> FinishRequirements = new List<RequirementD>();

        [Space]
        [Header("Build From Data Test")]
        public bool TestFromData = false;
        //public SequenceStatus StartingEventState = SequenceStatus.Locked;
        //public List<FPTransitionMapper> fakeTestData = new List<FPTransitionMapper>();
        //public List<RequirementD> fakeRequirementData = new List<RequirementD>();
        //new struct setup
        public FPSimpleEventData FakeTestEventSO;
        //[FPNest] public FPSingleEventData FakeTestEventData;
        //
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
                eventRef.DataResolveAndActivate(
                    FakeTestEventSO.ModuleEventData.StartingEventState, 
                    FakeTestEventSO.ModuleEventData.TransitionMapperData 
                   );
            }
        }
        protected virtual void Update()
        {
            if (!KeyBoardActiveTesting) return;

            if (Input.GetKeyDown(TestForwardTransition))
            {
                TryForwardTransition();
            }
            if (Input.GetKeyDown(TestRequirementsUnlockTransition))
            {
                TryTransition(TransitionToUnlockRequirements, TestUnlockRequirements);
            }
            if (Input.GetKeyDown(TestRequirementsFinishedTransition))
            {
                TryTransition(TransitionToFinishRequirements, FinishRequirements);
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
