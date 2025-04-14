namespace FuzzPhyte.SGraph
{
    using System;
    using FuzzPhyte.Utility;
    using UnityEngine;
    using System.Collections.Generic;
    public class FPHelperTimer : MonoBehaviour,IFPTimer<EventHelperData>
    {
        #region Helper Configuration
        [SerializeField]
        protected HelperThresholdConfig helperThresholdConfig;
        protected Dictionary<(HelperCategory, SequenceStatus), float> triggerThresholds;
        protected Dictionary<(HelperCategory, SequenceStatus), float> lastTriggeredTimes = new Dictionary<(HelperCategory, SequenceStatus), float>();
        protected Dictionary<string, EventHelperData> pastHelperEventData = new Dictionary<string, EventHelperData>();
        protected uint runningHelperIndex=0;
    #endregion
        //protected PriorityQueue<EventHelperData> helpers = new PriorityQueue<EventHelperData>();
        protected static FPHelperTimer _instance;
        [Tooltip("If we want this GO to persist across scenes")]
        public bool DontDestroy = false;
        [Tooltip("If we want to set up the helper timer on awake")]
        public bool SetupOnAwake = true;
        protected bool isInitialized = false;
        public static FPHelperTimer HelperTimer { get { return _instance; } }
        public virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            if (SetupOnAwake)
            {
                if (helperThresholdConfig != null)
                {
                    triggerThresholds = helperThresholdConfig.ToDictionary();
                    isInitialized = true;
                }
                else
                {
                    Debug.LogError("No HelperThresholdConfig assigned to FPHelperTimer - you should assign one!");
                    triggerThresholds = new Dictionary<(HelperCategory, SequenceStatus), float>();
                    isInitialized = false;
                }
            }
            
            if (DontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        protected PriorityQueue<EventHelperData> helperTimers = new PriorityQueue<EventHelperData>();
        
        public void ResetSetupHelperTimer(HelperThresholdConfig configurationFile)
        {
            helperTimers.ResetAndClear();
            helperThresholdConfig = configurationFile;
            triggerThresholds = helperThresholdConfig.ToDictionary();
            isInitialized = true;
        }
        protected virtual void Update()
        {
            if (!isInitialized)
            {
                return;
            }
            while (helperTimers.Count > 0 && helperTimers.Peek().ActivationTime <= Time.time)
            {
                EventHelperData helperData = helperTimers.Dequeue();
                var key = (helperData.Category, helperData.EventState.CurrentState);
                //Smart Logic can Happen Here or when we call the StartTimer functions
                //
                // we can get our gameobject back from the FPEventState 
                var gOthatStarted = helperData.EventState.UnityObject;
                // Check if the state has changed
                if (!helperData.IsHelperStateUnchanged())
                {
                    Debug.LogWarning("Helper timer cancelled due to state change.");
                    continue;
                }
                // Check time since last trigger for this category and state
                // Determine the threshold for this helper's category and state
                float maxDelay = triggerThresholds.ContainsKey(key) ? triggerThresholds[key] : helperData.ActivationTime;

                // Check if enough time has passed since the last trigger for this category and state
                if (lastTriggeredTimes.TryGetValue(key, out float lastTime) && (Time.time - lastTime) < maxDelay)
                {
                    Debug.LogWarning("Helper not triggered: minimum time interval not met.");
                    continue;
                }

                // Update last triggered time and run the helper
                lastTriggeredTimes[key] = Time.time;
                Debug.LogWarning($"Helper Timer Finished with Action: {helperData.onActivate.Method.Name}");
                // logic needed
                helperData.onActivate();
            }
        }
        public bool HasRecentlyTriggered((HelperCategory, SequenceStatus) key, float delay)
        {
            // Check if the last trigger time exists for this key
            if (lastTriggeredTimes.TryGetValue(key, out float lastTime))
            {
                // If it exists, check if the time elapsed since last trigger is less than the delay
                return (Time.time - lastTime) < delay;
            }
            // If no entry exists, it means this helper has never been triggered, so return false
            return false;
        }
        public void StartTimer(float time, Action onFinish,HelperCategory category,FPEventState gObject, string uniqueName="null", HelperAction hAction = HelperAction.NA)
        {
            var eventData = StartTimer(time, onFinish);
            eventData.Category = category;
            eventData.HelperAction = hAction;
            eventData.SetupEventState(gObject);
            if (uniqueName != "null")
            {
                pastHelperEventData.Add(uniqueName, eventData);
            }
            helperTimers.Enqueue(eventData);
            runningHelperIndex++;
        }
        public void StartTimer(float time, int param,Action<int>onFinish,HelperCategory category,FPEventState gObject, string uniqueName = "null", HelperAction hAction = HelperAction.NA)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.HelperAction = hAction;
            eventData.SetupEventState(gObject);
            if (uniqueName != "null")
            {
                pastHelperEventData.Add(uniqueName, eventData);
            }
            helperTimers.Enqueue(eventData);
            runningHelperIndex++;
        }
        public void StartTimer(float time, float param,Action<float>onFinish,HelperCategory category,FPEventState gObject, string uniqueName = "null", HelperAction hAction = HelperAction.NA)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.HelperAction = hAction;
            eventData.SetupEventState(gObject);
            if (uniqueName != "null")
            {
                pastHelperEventData.Add(uniqueName, eventData);
            }
            helperTimers.Enqueue(eventData);
            runningHelperIndex++;
        }
        public void StartTimer(float time, string param, Action<string>onFinish,HelperCategory category,FPEventState gObject, string uniqueName = "null", HelperAction hAction = HelperAction.NA)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.HelperAction = hAction;
            eventData.SetupEventState(gObject);
            if (uniqueName != "null")
            {
                pastHelperEventData.Add(uniqueName, eventData);
            }
            helperTimers.Enqueue(eventData);
            runningHelperIndex++;
        }
        public void StartTimer(float time, FP_Data param, Action<FP_Data>onFinish,HelperCategory category,FPEventState gObject, string uniqueName = "null", HelperAction hAction = HelperAction.NA)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.HelperAction = hAction;
            eventData.SetupEventState(gObject);
            if (uniqueName != "null")
            {
                pastHelperEventData.Add(uniqueName, eventData);
            }
            helperTimers.Enqueue(eventData);
            runningHelperIndex++;
        }
        public void StartTimer(float time, GameObject param, Action<GameObject>onFinish,HelperCategory category,FPEventState gObject, string uniqueName = "null", HelperAction hAction = HelperAction.NA)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.HelperAction = hAction;
            eventData.SetupEventState(gObject);
            if (uniqueName != "null")
            {
                pastHelperEventData.Add(uniqueName, eventData);
            }
            helperTimers.Enqueue(eventData);
            runningHelperIndex++;
        }
        
        public (bool,EventHelperData) ContainTimerByUniqueName(string helperName)
        {
            if (pastHelperEventData.ContainsKey(helperName))
            {
                var data = pastHelperEventData[helperName];
                return (helperTimers.Contains(data),data);
            }
            return (false,null);
        }
        public (bool,EventHelperData) TimerActiveByUniqueName(string helperName)
        {
            if (pastHelperEventData.ContainsKey(helperName))
            {
                var data = pastHelperEventData[helperName];
                if (helperTimers.Contains(data))
                {
                    return (helperTimers.IsActive(data),data);
                }
            }
            return (false,null);
        }
        public EventHelperData StartTimer(float time, Action onFinish)
        {
            return  new EventHelperData(Time.time + time, onFinish);
        }
        public EventHelperData StartTimer(float time, int param, Action<int> onFinish)
        {
            return new EventHelperData(Time.time + time, () => onFinish(param));
        }
        public EventHelperData StartTimer(float time, float param, Action<float> onFinish)
        {
            return new EventHelperData(Time.time + time, () => onFinish(param));
        }
        public EventHelperData StartTimer(float time, string param, Action<string> onFinish)
        {
            return new EventHelperData(Time.time + time, () => onFinish(param));
        }
        public EventHelperData StartTimer(float time, FP_Data param, Action<FP_Data> onFinish)
        {
            return new EventHelperData(Time.time + time, () => onFinish(param)); 
        }
        public EventHelperData StartTimer(float time, GameObject param, Action<GameObject> onFinish)
        {
            return new EventHelperData(Time.time + time, () => onFinish(param));
        }
    }
}
