namespace FuzzPhyte.SGraph
{
    using FuzzPhyte.Utility;
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "HelperThresholdConfig", menuName = "FuzzPhyte/Helper/HelperThresholdConfig")]
    public class HelperThresholdConfig : FP_Data
    {
        public List<HelperThreshold> thresholds = new List<HelperThreshold>();

        /// <summary>
        /// Converts the list of thresholds into a dictionary for quick lookups.
        /// </summary>
        public Dictionary<(HelperCategory, SequenceStatus), float> ToDictionary()
        {
            var dictionary = new Dictionary<(HelperCategory, SequenceStatus), float>();
            foreach (var entry in thresholds)
            {
                dictionary[(entry.Category, entry.State)] = entry.MaxDelay;
            }
            return dictionary;
        }
    }
}
