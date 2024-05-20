using Godot;
using MPewsey.ManiaMap.Exceptions;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A collection of collectables and quantities for distribution across associated CollectableSpot2D.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class CollectableGroup : Resource
    {
        /// <summary>
        /// The unique group name.
        /// </summary>
        [Export] public string GroupName { get; set; } = "<None>";

        /// <summary>
        /// An array of entries.
        /// </summary>
        [Export] public CollectableGroupEntry[] Entries { get; set; } = Array.Empty<CollectableGroupEntry>();

        /// <summary>
        /// Returns a list of collectable ID's in the quantities specified in the group's Entries.
        /// </summary>
        /// <exception cref="DuplicateIdException">Raised if a collectable has a duplicate ID.</exception>
        public List<int> GetCollectableIds()
        {
            var result = new List<int>();
            var set = new HashSet<int>();

            foreach (var entry in Entries)
            {
                if (!set.Add(entry.Collectable.Id))
                    throw new DuplicateIdException($"Duplicate collectable ID: {entry.Collectable.Id}.");

                for (int i = 0; i < entry.Quantity; i++)
                {
                    result.Add(entry.Collectable.Id);
                }
            }

            return result;
        }
    }
}