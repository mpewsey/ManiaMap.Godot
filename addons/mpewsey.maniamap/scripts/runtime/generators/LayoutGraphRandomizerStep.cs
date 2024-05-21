using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// A GenerationStep that applies variation group randomization to the `LayoutGraph` input.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphRandomizerStep : GenerationStep
    {
        /// <inheritdoc/>
        public override IPipelineStep CreateStep()
        {
            return new LayoutGraphRandomizer();
        }

        /// <inheritdoc/>
        public override string[] RequiredInputNames()
        {
            return new string[] { "LayoutGraph", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}