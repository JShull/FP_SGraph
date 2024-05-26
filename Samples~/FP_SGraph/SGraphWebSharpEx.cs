namespace FuzzPhyte.SGraph.Samples
{
    //Main Web Graph Data Class for the Sample
    public class SGraphWebSharpEx : SGraphWebSharpBase<SGraphTransitionData,SGraphRequirementData>
    {
        public SGraphWebSharpEx(SGraphNodeSharpBase<SGraphTransitionData,SGraphRequirementData> entryDataNode) : base(entryDataNode)
        {
        }
    }
}
