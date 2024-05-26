namespace FuzzPhyte.SGraph
{
    using System;
    using FuzzPhyte.Utility.Meta;
    [Serializable]
    public struct RequirementD
    {
        public FP_Tag RequirementTag;
        public string RequirementName;
        public bool RequirementMet;
    }
}
