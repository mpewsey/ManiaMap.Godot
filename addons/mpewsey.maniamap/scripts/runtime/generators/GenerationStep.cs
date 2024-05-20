using Godot;
using MPewsey.Common.Pipelines;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// The base class for GenerationPipeline step nodes.
    /// A step node transforms inputs or adds outputs to the pipeline results.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.GenerationStepIcon)]
    public abstract partial class GenerationStep : Node
    {
        /// <summary>
        /// An array of required input names.
        /// </summary>
        public abstract string[] RequiredInputNames();

        /// <summary>
        /// An array of added output names.
        /// </summary>
        public abstract string[] OutputNames();

        /// <summary>
        /// Creates the pipeline step used by the pipeline.
        /// </summary>
        public abstract IPipelineStep CreateStep();
    }
}