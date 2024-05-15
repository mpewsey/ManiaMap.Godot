using Godot;
using MPewsey.ManiaMap;
using System;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class TemplateGroup : Resource
    {
        [Export] public string Name { get; set; } = "<None>";
        [Export] public TemplateGroupEntry[] Entries { get; set; } = Array.Empty<TemplateGroupEntry>();

        public TemplateGroupsEntry[] GetEntries()
        {
            var result = new TemplateGroupsEntry[Entries.Length];

            for (int i = 0; i < Entries.Length; i++)
            {
                var entry = Entries[i];
                result[i] = new TemplateGroupsEntry(entry.RoomTemplate.Template, entry.MinQuantity, entry.MaxQuantity);
            }

            return result;
        }
    }
}