using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class TemplateGroupDatabase : Resource
    {
        [Export] public TemplateGroup[] TemplateGroups { get; set; } = Array.Empty<TemplateGroup>();
        private Dictionary<int, RoomTemplateResource> RoomTemplates { get; } = new Dictionary<int, RoomTemplateResource>();
        public bool IsDirty { get; private set; } = true;

        public IReadOnlyDictionary<int, RoomTemplateResource> GetRoomTemplates()
        {
            PopulateIfDirty();
            return RoomTemplates;
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateRoomTemplates();
                IsDirty = false;
            }
        }

        private void PopulateRoomTemplates()
        {
            RoomTemplates.Clear();

            foreach (var group in TemplateGroups)
            {
                foreach (var entry in group.Entries)
                {
                    var id = entry.RoomTemplate.Id;

                    if (!RoomTemplates.TryGetValue(id, out var template))
                        RoomTemplates.Add(id, entry.RoomTemplate);
                    else if (template != entry.RoomTemplate)
                        throw new Exception($"Duplicate room template ID: {id}.");
                }
            }
        }

        public RoomTemplateResource GetRoomTemplate(int id)
        {
            PopulateIfDirty();
            return RoomTemplates[id];
        }

        public void CreateRoom2DInstances(Node parent, int? z = null)
        {
            var manager = ManiaMapManager.Current;
            z ??= manager.Layout.Rooms.Values.Select(x => x.Position.Z).First();

            foreach (var room in manager.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var template = GetRoomTemplate(room.Template.Id);
                    RoomNode2D.CreateInstance(room.Id, template.LoadScene(), parent, true);
                }
            }
        }

        public RoomNode2D CreateRoom2DInstance(Uid id, Node parent)
        {
            var manager = ManiaMapManager.Current;
            var room = manager.Layout.Rooms[id];
            var template = GetRoomTemplate(room.Template.Id);
            return RoomNode2D.CreateInstance(id, template.LoadScene(), parent);
        }
    }
}