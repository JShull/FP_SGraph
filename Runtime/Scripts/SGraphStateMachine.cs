namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using System;

    /// <summary>
    /// Abstract class for an 'Event' in the SGraph
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SGraphStateMachine<T> 
    {
        public SequenceStatus CurrentState { get; protected set; }
        protected List<IRequirement<T>> unlockRequirements;
        protected Dictionary<SequenceTransition, SequenceStatus> stateTransitions;
        protected Dictionary<SequenceStatus, List<Action>> stateActions;
        public delegate void StateEventHandler(SGraphStateMachine<T>sEvent);
        public event StateEventHandler OnFinish;
        public event StateEventHandler OnUnlocked;
        public event StateEventHandler OnLocked;
        public event StateEventHandler OnActive;

        #region Constructors
        protected SGraphStateMachine(SequenceStatus startingState)
        {
            CurrentState = startingState;
            unlockRequirements = new List<IRequirement<T>>();
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<SequenceStatus, List<Action>>();
        }
        protected SGraphStateMachine(SequenceStatus startingState, List<IRequirement<T>> requirements)
        {
            CurrentState = startingState;
            unlockRequirements = requirements;
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<SequenceStatus, List<Action>>();
        }
        protected SGraphStateMachine(SequenceStatus startingState, List<IRequirement<T>> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions)
        {
            CurrentState = startingState;
            unlockRequirements = requirements;
            stateTransitions = transitions;
            stateActions = new Dictionary<SequenceStatus, List<Action>>();
        }
        protected SGraphStateMachine(SequenceStatus startingState, List<IRequirement<T>> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<SequenceStatus, List<Action>> actions)
        {
            CurrentState = startingState;
            unlockRequirements = requirements;
            stateTransitions = transitions;
            stateActions = actions;
        }
        #endregion
        #region Update State Transitions and Requirements
        protected virtual void AddUnlockRequirement(IRequirement<T> requirement)
        {
            if(!unlockRequirements.Contains(requirement))
            {
                unlockRequirements.Add(requirement);
            }
        }
        protected virtual void RemoveUnlockRequirement(IRequirement<T> requirement)
        {
            if(unlockRequirements.Contains(requirement))
            {
                unlockRequirements.Remove(requirement);
            }
        }
        protected virtual void AddStateTransition(SequenceTransition transition, SequenceStatus newState)
        {
            if(!stateTransitions.ContainsKey(transition))
            {
                stateTransitions.Add(transition, newState);
            }
        }
        protected virtual void RemoveStateTransition(SequenceTransition transition)
        {
            if(stateTransitions.ContainsKey(transition))
            {
                stateTransitions.Remove(transition);
            }
        }
        protected virtual void AddStateAction(SequenceStatus state, Action action)
        {
            if(!stateActions.ContainsKey(state))
            {
                stateActions.Add(state, new List<Action>());
            }
            stateActions[state].Add(action);
        }
        protected virtual void RemoveStateAction(SequenceStatus state, Action action)
        {
            if(stateActions.ContainsKey(state))
            {
                //check if the action is in the running list because we don't want to remove an action that isn't there
                if (stateActions[state].Contains(action))
                {
                    stateActions[state].Remove(action);
                }
                //clear the state if we have no actions left
                if (stateActions[state].Count == 0)
                {
                    stateActions.Remove(state);
                }
            }
        }
        #endregion
        #region Delegate Events
        protected virtual void FinishEvent()
        {
            OnFinish?.Invoke(this);
        }
        protected virtual void UnlockEvent()
        {
            OnUnlocked?.Invoke(this);
        }
        protected virtual void LockEvent()
        {
            OnLocked?.Invoke(this);
        }
        protected virtual void ActiveEvent()
        {
            OnActive?.Invoke(this);
        }
        #endregion
        /// <summary>
        /// Public accessor to attempt at trying to transition the state
        /// If we are successful this function will manage the firing of the delegate events
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public virtual (bool,SequenceStatus) TryTransition(SequenceTransition transition)
        {
            if (!MeetsRequirements())
            {
                return (false,CurrentState);
            }
            var transitionOutcome= InternalTransition(transition);
            if (transitionOutcome.Item1)
            {
                //we successfully transitioned need to activate our Delegate events
                switch (CurrentState)
                {
                    case SequenceStatus.Finished:
                        FinishEvent();
                        break;
                    case SequenceStatus.Unlocked:
                        UnlockEvent();
                        break;
                    case SequenceStatus.Locked:
                        LockEvent();
                        break;
                    case SequenceStatus.Active:
                        ActiveEvent();
                        break;
                }
                return (true, CurrentState);
            }
            return (false,CurrentState);
        }
        /// <summary>
        /// Internal Transition logic
        /// Responsible for checking the transitions
        /// If we are successful, this will set the current state based on the transition information 
        /// and run the actions by transitions
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected virtual (bool,SequenceStatus) InternalTransition(SequenceTransition transition)
        { 
            if(stateTransitions.ContainsKey(transition))
            {
                CurrentState = stateTransitions[transition];
                //invoke actions in order
                if(stateActions.ContainsKey(CurrentState))
                {
                    for(int i = 0; i < stateActions[CurrentState].Count; i++)
                    {
                        stateActions[CurrentState][i].Invoke();
                    }
                }
                return (true,CurrentState);
            }
            return (false,CurrentState);
        }
        /// <summary>
        /// Requirements Logic for the list of Requirements
        /// </summary>
        /// <returns></returns>
        public abstract bool MeetsRequirements();
    }
}
