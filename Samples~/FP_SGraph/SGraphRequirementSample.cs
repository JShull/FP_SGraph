using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.SGraph.Samples
{
    public class SGraphRequirementSample : RequirementSB<string>
    {
        public SGraphRequirementSample(string requiredType) : base(requiredType)
        {

        }

        public override bool CheckRequirement(string value)
        {
            if(value == RequiredType)
            {
                return true;
            }
            return false;
        }
    }
}
