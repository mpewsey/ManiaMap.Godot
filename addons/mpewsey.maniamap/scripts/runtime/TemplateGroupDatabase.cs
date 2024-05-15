using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class TemplateGroupDatabase : Resource
    {
        [Export] public TemplateGroup[] TemplateGroups { get; set; } = Array.Empty<TemplateGroup>();

        public Dictionary<int, RoomTemplateResource> GetRoomTemplatesById()
        {
            var dict = new Dictionary<int, RoomTemplateResource>();

            foreach (var group in TemplateGroups)
            {
                foreach (var entry in group.Entries)
                {
                    var id = entry.RoomTemplate.Id;

                    if (!dict.TryGetValue(id, out var template))
                        dict.Add(id, entry.RoomTemplate);
                    else if (template != entry.RoomTemplate)
                        throw new Exception($"Duplicate room template ID: {id}.");
                }
            }

            return dict;
        }

        public void CreateRoom2DInstances(Node parent, int? z = null)
        {
            var manager = ManiaMapManager.Current;
            z ??= manager.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var templates = GetRoomTemplatesById();

            foreach (var room in manager.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var template = templates[room.Template.Id];
                    RoomNode2D.CreateInstance(room.Id, template.LoadScene(), parent);
                }
            }
        }
    }
}