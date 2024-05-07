using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;
using System;

namespace MPewsey.ManiaMapGodot.Generators
{
    [GlobalClass]
    public partial class CollectableGeneratorStep : GenerationStep
    {
        [Export] public float DoorPower { get; set; } = 2;
        [Export] public float NeighborPower { get; set; } = 1;
        [Export] public int InitialNeighborWeight { get; set; } = 1000;

        public override IPipelineStep CreateStep()
        {
            return new CollectableGenerator(DoorPower, NeighborPower, InitialNeighborWeight);
        }

        public override string[] RequiredInputNames()
        {
            return new string[] { "Layout", "CollectableGroups", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return Array.Empty<string>();
        }
    }
}