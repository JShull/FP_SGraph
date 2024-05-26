namespace FuzzPhyte.SGraph
{
    using System;
    using FuzzPhyte.Utility.Meta;
    [Serializable]
    public struct SGraphRequirementData
    {
        public FP_Tag RequirementTag;
        public string RequirementName;
        public bool RequirementMet;
    }
}
