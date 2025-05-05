namespace FuzzPhyte.SGraph
{
    using System.Collections.Generic;
    using FuzzPhyte.Utility;
    using System;
    using System.Linq;

    /// <summary>
    /// Abstract class for an 'Event' in the SGraph
    /// </summary>
    /// <typeparam name="R">requirement</typeparam>
    public abstract class StateMachineSB<R> where R : struct
    {
        public SequenceStatus CurrentState { get; protected set; }
        protected Dictionary<SequenceTransition, SequenceStatus> stateTransitions;
        protected Dictionary<SequenceTransition, List<R>> transitionRequirements;
        protected Dictionary<int,List<Action>> stateActions;
        public delegate void StateEventHandler(StateMachineSB<R>sEvent);
        public event StateEventHandler OnFinish;
        public event StateEventHandler OnUnlocked;
        public event StateEventHandler OnLocked;
        public event StateEventHandler OnActive;
        protected bool runInitialStateSequence;

        #region Constructors
        public StateMachineSB()
        {
            CurrentState = SequenceStatus.Locked;
            //unlockRequirements = requirements;
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<int, List<Action>>();
            transitionRequirements = new Dictionary<SequenceTransition, List<R>>();
        }

        protected StateMachineSB(SequenceStatus startingState)
        {
            CurrentState = startingState;
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<int, List<Action>>();
            transitionRequirements = new Dictionary<SequenceTransition, List<R>>();

        }
        protected StateMachineSB(SequenceStatus startingState, Dictionary<SequenceTransition, SequenceStatus> transitions)
        {
            CurrentState = startingState;
            stateTransitions = transitions;
            stateActions = new Dictionary<int, List<Action>>();
            transitionRequirements = new Dictionary<SequenceTransition, List<R>>();
        }
        protected StateMachineSB(SequenceStatus startingState, Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<SequenceTransition, List<R>> additionalTransitionRequirements)
        {
            CurrentState = startingState;
            stateTransitions = transitions;
            stateActions = new Dictionary<int, List<Action>>();
            this.transitionRequirements = additionalTransitionRequirements;
        }
        protected StateMachineSB(SequenceStatus startingState, Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<int, List<Action>> actions)
        {
            CurrentState = startingState;
            stateTransitions = transitions;
            stateActions = actions;
            transitionRequirements = new Dictionary<SequenceTransition, List<R>>();
        }
        protected StateMachineSB(SequenceStatus startingState, Dictionary<SequenceTransition,SequenceStatus> transitions,Dictionary<int,List<Action>> actions, Dictionary<SequenceTransition, List<R>> additionalTransitionRequirements)
        {
            CurrentState = startingState;
            stateTransitions = transitions;
            stateActions = actions;
            this.transitionRequirements = additionalTransitionRequirements;
        }
        protected StateMachineSB(Dictionary<SequenceTransition, List<R>> additionalTransitionRequirements)
        {
            CurrentState = SequenceStatus.Locked;
            stateTransitions = new Dictionary<SequenceTransition, SequenceStatus>();
            stateActions = new Dictionary<int, List<Action>>();
            this.transitionRequirements = additionalTransitionRequirements;
        }
        protected StateMachineSB(Dictionary<SequenceTransition, SequenceStatus> transitions, Dictionary<SequenceTransition, List<R>> additionalTransitionRequirements)
        {
            CurrentState = SequenceStatus.Locked;
            stateTransitions = transitions;
            stateActions = new Dictionary<int, List<Action>>();
            this.transitionRequirements = additionalTransitionRequirements;
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
        #region Transition Requirements by Sequence Transitions
        public virtual void AddRequirementsForTransition(SequenceTransition transition, List<R> requirements)
        {
            for(int i=0; i < requirements.Count; i++)
            {
                var curTransition = requirements[i];
                AddRequirementForTransition(transition, curTransition);
            }
        }
        public virtual void RemoveRequirementsForTransition(SequenceTransition transition, List<R> requirements)
        {
            for (int i = 0; i < requirements.Count; i++)
            {
                var curTransition = requirements[i];
                RemoveRequirementForTransition(transition, curTransition);
            }
        }
        /// <summary>
        /// Adds a requirement for a transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="requirement"></param>
        public virtual void AddRequirementForTransition(SequenceTransition transition, R requirement)
        {
            //check if we have a transition in our dictionary
            if (!transitionRequirements.ContainsKey(transition))
            {
                transitionRequirements[transition] = new List<R>();
            }
            //check if we have a requirement in our list, if we, add a new one to it
            if (!transitionRequirements[transition].Contains(requirement))
            {
                transitionRequirements[transition].Add(requirement);
            }
        }
        /// <summary>
        /// Removes a requirement for a transition
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="requirement"></param>
        public virtual void RemoveRequirementForTransition(SequenceTransition transition, R requirement)
        {
            if (transitionRequirements.ContainsKey(transition))
            {
                transitionRequirements[transition].Remove(requirement);
                if (transitionRequirements[transition].Count == 0)
                {
                    transitionRequirements.Remove(transition);
                }
            }
        }
        #endregion
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
            if(transitionRequirements.ContainsKey(transition))
            {
                if (transitionRequirements[transition].Count > 0)
                {
                    return (false, CurrentState);
                }
            }
            return ProcessTransition(transition);
        }
        public virtual (bool,SequenceStatus) TryAnyTransition(SequenceTransition transition,R requirementValue)
        {
            if (!UpdateTransitionCheckRequirementsList(transition, new List<R>() { requirementValue }))
            {
                return (false, CurrentState);
            }
            return ProcessTransition(transition);
        }
        public virtual (bool, SequenceStatus) TryAnyTransition(SequenceTransition transition, List<R> requirementValues)
        {
            if (!UpdateTransitionCheckRequirementsList(transition, requirementValues))
            {
                return (false, CurrentState);
            }
            return ProcessTransition(transition);
        }
        /// <summary>
        /// internal function to now process the transition by sequenceTransition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        protected virtual (bool,SequenceStatus) ProcessTransition(SequenceTransition transition)
        {
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
            var TransitionKeys = transitionRequirements.Keys.ToList();
            for (int i = 0; i < TransitionKeys.Count; i++)
            {
                var curTransition = transitionRequirements[TransitionKeys[i]];
                if (curTransition.Count > 0)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public abstract bool UpdateTransitionCheckRequirementsList(SequenceTransition transition, List<R> parameters);
    }
}
