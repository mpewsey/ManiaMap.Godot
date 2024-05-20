using Godot;
using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class TemplateGroupEntry : Resource
    {
        [Export] public RoomTemplateResource RoomTemplate { get; set; }
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int MinQuantity { get; set; }
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int MaxQuantity { get; set; } = int.MaxValue;

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