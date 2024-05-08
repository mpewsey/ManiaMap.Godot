using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapGodot.Generators
{
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphSelectorStep : GenerationStep
    {
        public override IPipelineStep CreateStep()
        {
            return new LayoutGraphSelector();
        }

        public override string[] RequiredInputNames()
        {
            return new string[] { "LayoutGraphs", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}