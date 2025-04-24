namespace FuzzPhyte.SGraph
{
    using System;
    using FuzzPhyte.Utility.Meta;
    using FuzzPhyte.Utility.Attributes;
    [Serializable]
    public struct RequirementD
    {
        [FPNest]public string RequirementName;
        public FP_Tag RequirementTag;
        public bool RequirementMet;
    }
}
