using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A group of RoomTemplateResource and their procedural generation constraints.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class TemplateGroup : Resource
    {
        /// <summary>
        /// The unique group name.
        /// </summary>
        [Export] public string Name { get; set; } = "<None>";

        /// <summary>
        /// An array of room templates and constraints.
        /// </summary>
        [Export] public TemplateGroupEntry[] Entries { get; set; } = Array.Empty<TemplateGroupEntry>();

        /// <summary>
        /// Returns an array of ManiaMap template groups for the procedural generator.
        /// </summary>
        /// <param name="templateCache">A dictionary of cached room templates by resource.</param>
        public TemplateGroupsEntry[] GetMMTemplateGroupEntries(Dictionary<RoomTemplateResource, RoomTemplate> templateCache)
        {
            var result = new TemplateGroupsEntry[Entries.Length];

            for (int i = 0; i < Entries.Length; i++)
            {
                result[i] = Entries[i].GetMMTemplateGroupsEntry(templateCache);
            }

            return result;
        }
    }
}