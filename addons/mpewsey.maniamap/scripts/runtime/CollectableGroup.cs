using Godot;
using MPewsey.ManiaMap.Exceptions;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class CollectableGroup : Resource
    {
        [Export] public string GroupName { get; set; } = "<None>";
        [Export] public CollectableGroupEntry[] Entries { get; set; } = Array.Empty<CollectableGroupEntry>();

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