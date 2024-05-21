using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// A GenerationStep that selects a random `LayoutGraph` from the provided input.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphSelectorStep : GenerationStep
    {
        /// <inheritdoc/>
        public override IPipelineStep CreateStep()
        {
            return new LayoutGraphSelector();
        }

        /// <inheritdoc/>
        public override string[] RequiredInputNames()
        {
            return new string[] { "LayoutGraphs", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}