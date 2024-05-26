using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FuzzPhyte.SGraph
{
    //Might not need access to T,R but it's there if you need it
    //M is the data class usually made up of <T,R>
    //T is our transition data which has to be a struct
    //R is the requirement data which can be whatever you need it to be
    public abstract class WebManagerMB<T,R,M> : MonoBehaviour where T : struct where R: struct
    {
        public List<M> DataNodes;

        public WebSB<T,R> TheWeb;
        /// <summary>
        // dictionary of the in scene unity nodes with their data class as the key
        /// </summary>
        protected Dictionary<M, GameObject> unityNodes;

        public Dictionary<M, GameObject> UnityNodes
        {
            get
            {
                return unityNodes;
            }
            set
            {
                unityNodes = value;
            }
        }

        protected abstract void SetupEntryPoint(M data);

        public virtual GameObject ReturnNodeByData(M data, out bool foundMatch)
        {
            if (unityNodes.ContainsKey(data))
            {
                foundMatch = true;
                return unityNodes[data];
            }
            foundMatch = false;
            return null;
        }
        protected UnityEvent ReturnUnityEventFromDataAction(System.Action action)
        {
            UnityEvent unityEvent = new UnityEvent();
            unityEvent.AddListener(() => { action(); });
            return unityEvent;
        }
        
    }
}
