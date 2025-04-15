namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using System;
    //using System.Diagnostics;

    /// <summary>
    /// Abstract class for an 'Event' in the SGraph
    /// </summary>
    /// <typeparam name="R">requirement</typeparam>
    public abstract class StateMachineSB<R> where R : struct
    {
        public SequenceStatus CurrentState { get; protected set; }
        protected List<R> unlockRequirements;
        protected Dictionary<SequenceTransition, SequenceStatus> stateTransitions;
        protected Dictionary<int,List<Action>> stateActions;
        public delegate void StateEventHandler(StateMachineSB<R>sEvent);
        public event StateEventHandler OnFinish;
        public event StateEventHandler OnUnlocked;
        public event StateEventHandler OnLocked;
        public event StateEventHandler OnActive;
        protected bool runInitialStateSequence;

        #region Constructors
        public StateMachineSB(List<R>requirements)
        {
            CurrentState = SequenceStatus.Locked;
            unlockRequirements = requirements;
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<int, List<Action>>();
        }
       
        protected StateMachineSB(SequenceStatus startingState)
        {
            CurrentState = startingState;
            unlockRequirements = new List<R>();
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<int, List<Action>>();
        }
        protected StateMachineSB(SequenceStatus startingState, List<R> requirements)
        {
            CurrentState = startingState;
            unlockRequirements = requirements;
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<int, List<Action>>();
        }
        protected StateMachineSB(SequenceStatus startingState, List<R> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions)
        {
            CurrentState = startingState;
            unlockRequirements = requirements;
            stateTransitions = transitions;
            stateActions = new Dictionary<int, List<Action>>();
        }
        protected StateMachineSB(SequenceStatus startingState, List<R> requirements, Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<int, List<Action>> actions)
        {
            CurrentState = startingState;
            unlockRequirements = requirements;
            stateTransitions = transitions;
            stateActions = actions;
        }
        #endregion
        #region Decode/Code Unique Index by Transition and Outcome
        protected virtual int ReturnUniqueIndexByTransitionAndOutcome(SequenceTransition transition, SequenceStatus outcome)
        {
            return (int)transition * 100 + (int)outcome;
        }
        protected virtual (SequenceTransition, SequenceStatus) ReturnTransitionAndOutcomeByUniqueIndex(int index)
        {
            int transition = index / 100;
            int outcome = index % 100;
            return ((SequenceTransition)transition, (SequenceStatus)outcome);
        }
        #endregion
        #region Update State Transitions and Requirements
        public virtual void AddUnlockRequirement(R requirement)
        {
            if(!unlockRequirements.Contains(requirement))
            {
                unlockRequirements.Add(requirement);
            }
        }
        public virtual void RemoveUnlockRequirement(R requirement)
        {
            if(unlockRequirements.Contains(requirement))
            {
                unlockRequirements.Remove(requirement);
            }
        }
        public virtual void AddStateTransition(SequenceTransition transition, SequenceStatus newState)
        {
            if(!stateTransitions.ContainsKey(transition))
            {
                stateTransitions.Add(transition, newState);
            }
        }
        public virtual void RemoveStateTransition(SequenceTransition transition)
        {
            if(stateTransitions.ContainsKey(transition))
            {
                stateTransitions.Remove(transition);
            }
        }
        public virtual void AddStateAction(SequenceStatus state,SequenceTransition transition, Action action)
        {
            var index = ReturnUniqueIndexByTransitionAndOutcome(transition, state);
            if(!stateActions.ContainsKey(index))
            {
                stateActions.Add(index, new List<Action>());
            }
            stateActions[index].Add(action);
        }
        public virtual void RemoveStateAction(SequenceStatus state,SequenceTransition transition, Action action)
        {
            var index = ReturnUniqueIndexByTransitionAndOutcome(transition, state);
            if(stateActions.ContainsKey(index))
            {
                //check if the action is in the running list because we don't want to remove an action that isn't there
                if (stateActions[index].Contains(action))
                {
                    stateActions[index].Remove(action);
                }
                //clear the state if we have no actions left
                if (stateActions[index].Count == 0)
                {
                    stateActions.Remove(index);
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
        /// This will only run once and is designed to be called upon initialization/setup
        /// </summary>
        /// <returns></returns>
        public virtual bool TryInvokeEventInitialization()
        {
            if (!runInitialStateSequence)
            {

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
                runInitialStateSequence = true;
            }
            return false;
        }
        /// <summary>
        /// Public accessor to attempt to try a transition
        /// if there are any unlock requirements we will return false
        /// use the other method to pass in requirements
        /// </summary>
        /// <param name="transition"></param>
        public virtual (bool,SequenceStatus) TryTransition(SequenceTransition transition)
        {
            if(unlockRequirements.Count>0)
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
        /// Public accessor to attempt at trying to transition the state
        /// If we are successful this function will manage the firing of the delegate events
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public virtual (bool,SequenceStatus) TryTransition(SequenceTransition transition, List<R> requirementValue)
        {
            if (!UpdateUnlockCheckRequirementsList(requirementValue))
            {
                return (false,CurrentState);
            }
            //this logic is also in the other function
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
        public virtual (bool,SequenceStatus) TryTransition(SequenceTransition transition, R requirementValue)
        {
            if (!UpdateUnlockCheckRequirement(requirementValue))
            {
                return (false, CurrentState);
            }
            //this logic is also in the other function
            var transitionOutcome = InternalTransition(transition);
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
            return (false, CurrentState);
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
                var keyByTransitionAndState = ReturnUniqueIndexByTransitionAndOutcome(transition, CurrentState);
                //invoke actions in order
                if(stateActions.ContainsKey(keyByTransitionAndState))
                {
                    for(int i = 0; i < stateActions[keyByTransitionAndState].Count; i++)
                    {
                        stateActions[keyByTransitionAndState][i].Invoke();
                    }
                }
                return (true,CurrentState);
            }
            return (false,CurrentState);
        }

        /// <summary>
        /// Checks against our UnlockRequirements List to see if we have any left
        /// </summary>
        /// <returns></returns>
        public virtual bool MeetsRequirements()
        {
            if (unlockRequirements.Count == 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// update the requirements list with passed parameters for unlocking
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract bool UpdateUnlockCheckRequirementsList(List<R> parameters);

        public abstract bool UpdateUnlockCheckRequirement(R parameter);
        
    }
}
