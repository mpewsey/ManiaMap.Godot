using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapGodot.Generators
{
    [GlobalClass]
    public partial class LayoutGraphRandomizerStep : GenerationStep
    {
        public override IPipelineStep CreateStep()
        {
            return new LayoutGraphRandomizer();
        }

        public override string[] RequiredInputNames()
        {
            return new string[] { "LayoutGraph", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}