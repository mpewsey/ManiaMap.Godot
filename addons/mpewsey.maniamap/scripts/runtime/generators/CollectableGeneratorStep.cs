using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;
using System;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// A GenerationStep that randomly distributes collectables throughout a `Layout`.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class CollectableGeneratorStep : GenerationStep
    {
        /// <summary>
        /// The exponent used for the door weight.
        /// </summary>
        [Export] public float DoorPower { get; set; } = 2;

        /// <summary>
        /// The exponent used for the neighbor weight.
        /// </summary>
        [Export] public float NeighborPower { get; set; } = 1;

        /// <summary>
        /// The initial neighbor weight.
        /// </summary>
        [Export] public int InitialNeighborWeight { get; set; } = 1000;

        /// <inheritdoc/>
        public override IPipelineStep CreateStep()
        {
            return new CollectableGenerator(DoorPower, NeighborPower, InitialNeighborWeight);
        }

        /// <inheritdoc/>
        public override string[] RequiredInputNames()
        {
            return new string[] { "Layout", "CollectableGroups", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return Array.Empty<string>();
        }
    }
}