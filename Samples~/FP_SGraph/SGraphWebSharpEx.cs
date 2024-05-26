namespace FuzzPhyte.SGraph.Samples
{
    //Main Web Graph Data Class for the Sample
    public class SGraphWebSharpEx : WebSB<TransitionD,RequirementD>
    {
        public SGraphWebSharpEx(NodeSB<TransitionD,RequirementD> entryDataNode) : base(entryDataNode)
        {
        }
    }
}
