using UnityEngine;
using UnityEngine.Events;

namespace FuzzPhyte.SGraph.Samples
{
    /// <summary>
    /// Demo how to enable/disable a component from the helper logic
    /// </summary>
    public class SimpleComponentEx : MonoBehaviour
    {
        public UnityEvent OnAwakeEvent;

        public void Awake()
        {
            OnAwakeEvent.Invoke();
        }
    }
}
