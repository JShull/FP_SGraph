using System;
using FuzzPhyte.Utility;
using UnityEngine;

namespace FuzzPhyte.SGraph
{
    public class FPHelperTimer : MonoBehaviour,IFPTimer<EventHelperData>
    {
        protected PriorityQueue<EventHelperData> helpers = new PriorityQueue<EventHelperData>();

        protected static FPHelperTimer _instance;
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
        }
        protected PriorityQueue<EventHelperData> helperTimers = new PriorityQueue<EventHelperData>();
        protected virtual void Update()
        {
            while (helperTimers.Count > 0 && helperTimers.Peek().ActivationTime <= Time.time)
            {
                EventHelperData timerData = helperTimers.Dequeue();
                Debug.LogWarning($"Helper Timer Finished with Action: {timerData.onActivate.Method.Name}");
                timerData.onActivate();
            }
        }
        public void StartTimer(float time, Action onFinish,HelperCategory category,FPEventState gObject)
        {
            var eventData = StartTimer(time, onFinish);
            eventData.Category = category;
            eventData.EventState = gObject;
            helperTimers.Enqueue(eventData);
        }
        public void StartTimer(float time, int param,Action<int>onFinish,HelperCategory category,FPEventState gObject)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.EventState = gObject;
            helperTimers.Enqueue(eventData);
        }
        public void StartTimer(float time, float param,Action<float>onFinish,HelperCategory category,FPEventState gObject)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.EventState = gObject;
            helperTimers.Enqueue(eventData);
        }
        public void StartTimer(float time, string param, Action<string>onFinish,HelperCategory category,FPEventState gObject)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.EventState = gObject;
            helperTimers.Enqueue(eventData);
        }
        public void StartTimer(float time, FP_Data param, Action<FP_Data>onFinish,HelperCategory category,FPEventState gObject)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.EventState = gObject;
            helperTimers.Enqueue(eventData);
        }
        public void StartTimer(float time, GameObject param, Action<GameObject>onFinish,HelperCategory category,FPEventState gObject)
        {
            var eventData = StartTimer(time,param,onFinish);
            eventData.Category = category;
            eventData.EventState = gObject;
            helperTimers.Enqueue(eventData);
        }
        public EventHelperData StartTimer(float time, Action onFinish)
        {
            EventHelperData helperData = new EventHelperData
            {
                ActivationTime = Time.time + time,
                onActivate = onFinish
            };
            return helperData;
        }
        

        public EventHelperData StartTimer(float time, int param, Action<int> onFinish)
        {
            EventHelperData helperData = new EventHelperData
            {
                ActivationTime = Time.time + time,
                onActivate = () => onFinish(param)
            };
            return helperData;
        }

        public EventHelperData StartTimer(float time, float param, Action<float> onFinish)
        {
            EventHelperData helperData = new EventHelperData
            {
                ActivationTime = Time.time + time,
                onActivate = () => onFinish(param)
            };
            return helperData;
        }

        public EventHelperData StartTimer(float time, string param, Action<string> onFinish)
        {
            EventHelperData helperData = new EventHelperData
            {
                ActivationTime = Time.time + time,
                onActivate = () => onFinish(param)
            };
            return helperData;
        }

        public EventHelperData StartTimer(float time, FP_Data param, Action<FP_Data> onFinish)
        {
            EventHelperData helperData = new EventHelperData
            {
                ActivationTime = Time.time + time,
                onActivate = () => onFinish(param)
            };
            return helperData;
        }

        public EventHelperData StartTimer(float time, GameObject param, Action<GameObject> onFinish)
        {
            EventHelperData helperData = new EventHelperData
            {
                ActivationTime = Time.time + time,
                onActivate = () => onFinish(param)
            };
            return helperData;
        }
    }
}
