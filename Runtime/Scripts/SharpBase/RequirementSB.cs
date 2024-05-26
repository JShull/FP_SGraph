using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.SGraph
{
    using FuzzPhyte.Utility.Meta;
    public abstract class RequirementSB<OB> : IRequirement<OB> 
    {
        public OB RequiredType;
        public FP_Tag Tag;
        public RequirementSB(OB requiredType)
        {
            RequiredType = requiredType;
        }
        public virtual bool CheckRequirement(OB value)
        {
            return RequiredType.Equals(value);
        }
        
    }
}
