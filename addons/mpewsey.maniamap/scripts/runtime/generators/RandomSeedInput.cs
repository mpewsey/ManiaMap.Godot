using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    [GlobalClass]
    public partial class RandomSeedInput : GenerationInput
    {
        [Export(PropertyHint.Range, "-1,1000,1,or_greater")] public int RandomSeed { get; set; } = -1;

        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("RandomSeed", Rand.CreateRandomSeed(RandomSeed));
        }

        public override string[] InputNames()
        {
            return new string[] { "RandomSeed" };
        }
    }
}