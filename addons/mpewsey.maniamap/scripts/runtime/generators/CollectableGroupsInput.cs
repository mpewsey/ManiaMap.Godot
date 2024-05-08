using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot;
using MPewsey.ManiaMapGodot.Generators;
using System;
using System.Collections.Generic;

namespace MPewsey.Game
{
    [Tool]
    [GlobalClass]
    public partial class CollectableGroupsInput : GenerationInput
    {
        [Export] public CollectableGroup[] CollectableGroups { get; set; } = Array.Empty<CollectableGroup>();

        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("CollectableGroups", CreateGroups());
        }

        public override string[] InputNames()
        {
            return new string[] { "CollectableGroups" };
        }

        private CollectableGroups CreateGroups()
        {
            var groups = new CollectableGroups();

            foreach (var group in CollectableGroups)
            {
                groups.Add(group.GroupName, group.GetCollectableIds());
            }

            return groups;
        }
    }
}