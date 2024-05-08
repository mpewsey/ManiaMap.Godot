using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    [Tool]
    [GlobalClass]
    public partial class GenerationBooleanInput : GenerationInput
    {
        [Export] public string InputName { get; set; } = "<None>";
        [Export] public bool Value { get; set; }

        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add(InputName, Value);
        }

        public override string[] InputNames()
        {
            return new string[] { InputName };
        }
    }
}