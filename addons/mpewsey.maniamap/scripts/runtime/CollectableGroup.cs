using Godot;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class CollectableGroup : Resource
    {
        [Export] public string GroupName { get; set; }
        [Export] public CollectableGroupEntry[] Entries { get; set; } = Array.Empty<CollectableGroupEntry>();

        public List<int> GetCollectableIds()
        {
            var result = new List<int>();

            foreach (var entry in Entries)
            {
                for (int i = 0; i < entry.Quantity; i++)
                {
                    result.Add(entry.Collectable.Id);
                }
            }

            return result;
        }
    }
}