using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// The base class for GenerationPipeline input nodes.
    /// An input node adds an input used by the pipeline.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.GenerationInputIcon)]
    public abstract partial class GenerationInput : Node
    {
        /// <summary>
        /// An array of input names supplied by the node.
        /// </summary>
        public abstract string[] InputNames();

        /// <summary>
        /// Adds the input values to the supplied inputs dictionary.
        /// </summary>
        /// <param name="inputs">The inputs dictionary.</param>
        public abstract void AddInputs(Dictionary<string, object> inputs);
    }
}