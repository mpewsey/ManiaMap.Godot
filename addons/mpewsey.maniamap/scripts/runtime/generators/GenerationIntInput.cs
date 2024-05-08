using Godot;
using MPewsey.ManiaMapGodot.Generators;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class GenerationIntInput : GenerationInput
    {
        [Export] public string InputName { get; set; } = "<None>";
        [Export] public int Value { get; set; }

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