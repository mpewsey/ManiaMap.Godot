using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// A GenerationInput that adds a random seed to the pipeline inputs.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class RandomSeedInput : GenerationInput
    {
        /// <summary>
        /// The random seed. If the value is less than or equal to zero, then a random positive integer will be used instead.
        /// </summary>
        [Export(PropertyHint.Range, "-1,1000,1,or_greater")] public int RandomSeed { get; set; } = -1;

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("RandomSeed", Rand.CreateRandomSeed(RandomSeed));
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "RandomSeed" };
        }
    }
}