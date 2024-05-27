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
        public abstract List<M> DataNodes {get;set;}
        protected abstract void SetupEntryPoint(M data);
        public abstract GameObject ReturnNodeByData(M data, out bool foundMatch);
        public abstract UnityEvent ReturnUnityEventFromDataAction(System.Action action);
        
    }
}
