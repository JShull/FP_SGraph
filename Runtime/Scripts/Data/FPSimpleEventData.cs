namespace FuzzPhyte.SGraph
{
    using FuzzPhyte.Utility.Attributes;
    using FuzzPhyte.Utility;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FP Simple Event", menuName = "FuzzPhyte/SGraph/SimpleEvent", order = 20)]
    public class FPSimpleEventData : FP_Data
    {
        [FPNest] public string ModuleEventName;
        public FPSingleEventData ModuleEventData;
        public FP_Data SyncDataTag;
    }
}