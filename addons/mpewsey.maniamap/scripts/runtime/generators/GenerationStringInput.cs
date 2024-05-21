using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// Adds an string input value to the GenerationPipeline.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class GenerationStringInput : GenerationInput
    {
        /// <summary>
        /// The input name.
        /// </summary>
        [Export] public string InputName { get; set; } = "<None>";

        /// <summary>
        /// The input value.
        /// </summary>
        [Export] public string Value { get; set; }

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