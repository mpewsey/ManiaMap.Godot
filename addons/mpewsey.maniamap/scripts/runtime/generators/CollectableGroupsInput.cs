using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    [Tool]
    [GlobalClass]
    public partial class CollectableGroupsInput : GenerationInput
    {
        [Export] public CollectableGroup[] CollectableGroups { get; set; } = Array.Empty<CollectableGroup>();

        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("CollectableGroups", GetMMCollectableGroups());
        }

        public override string[] InputNames()
        {
            return new string[] { "CollectableGroups" };
        }

        private CollectableGroups GetMMCollectableGroups()
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