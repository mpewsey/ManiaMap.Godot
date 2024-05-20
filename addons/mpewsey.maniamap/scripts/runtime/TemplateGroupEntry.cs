using Godot;
using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// An TemplateGroup entry with a room template and its constraints.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class TemplateGroupEntry : Resource
    {
        /// <summary>
        /// The room template.
        /// </summary>
        [Export] public RoomTemplateResource RoomTemplate { get; set; }

        /// <summary>
        /// The minimum quantity of rooms with this template to include in the layout.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int MinQuantity { get; set; }

        /// <summary>
        /// The maximum quantity of rooms with this template to include in the layout.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int MaxQuantity { get; set; } = int.MaxValue;

        /// <summary>
        /// Returns a new ManiaMap template groups entry for use by the procedural generator.
        /// </summary>
        /// <param name="templateCache">A dictionary of cached room templates by resource.</param>
        public TemplateGroupsEntry GetMMTemplateGroupsEntry(Dictionary<RoomTemplateResource, RoomTemplate> templateCache)
        {
            if (!templateCache.TryGetValue(RoomTemplate, out var template))
            {
                template = RoomTemplate.GetMMRoomTemplate();
                templateCache.Add(RoomTemplate, template);
            }

            return new TemplateGroupsEntry(template, MinQuantity, MaxQuantity);
        }
    }
}