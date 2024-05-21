using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// Adds the ManiaMap collectable groups input to the GenerationPipeline inputs.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class CollectableGroupsInput : GenerationInput
    {
        /// <summary>
        /// An array of collectable groups.
        /// </summary>
        [Export] public CollectableGroup[] CollectableGroups { get; set; } = Array.Empty<CollectableGroup>();

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("CollectableGroups", GetMMCollectableGroups());
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "CollectableGroups" };
        }

        /// <summary>
        /// Returns the ManiaMap collectable groups used by the procedural generator.
        /// </summary>
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