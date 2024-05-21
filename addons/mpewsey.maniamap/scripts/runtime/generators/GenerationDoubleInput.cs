using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// Adds a double precision floating point input value to the GenerationPipeline.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class GenerationDoubleInput : GenerationInput
    {
        /// <summary>
        /// The input name.
        /// </summary>
        [Export] public string InputName { get; set; } = "<None>";

        /// <summary>
        /// The input value.
        /// </summary>
        [Export] public double Value { get; set; }

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add(InputName, Value);
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { InputName };
        }
    }
}