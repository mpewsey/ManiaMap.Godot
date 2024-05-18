using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class RoomTemplateDatabase : Resource
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

        public RoomTemplateResource GetRoomTemplate(Uid id)
        {
            var manager = ManiaMapManager.Current;
            var room = manager.Layout.Rooms[id];
            return GetRoomTemplate(room.Template.Id);
        }

        private async Task<Dictionary<int, PackedScene>> LoadScenesAsync(IEnumerable<Room> rooms, bool useSubThreads = false)
        {
            var tasks = new Dictionary<int, Task<PackedScene>>();

            foreach (var room in rooms)
            {
                var id = room.Template.Id;

                if (!tasks.ContainsKey(id))
                    tasks.Add(id, GetRoomTemplate(id).LoadSceneAsync(useSubThreads));
            }

            await Task.WhenAll(tasks.Values);
            var result = new Dictionary<int, PackedScene>(tasks.Count);

            foreach (var pair in tasks)
            {
                result.Add(pair.Key, pair.Value.Result);
            }

            return result;
        }

        public async Task<List<RoomNode2D>> CreateRoom2DInstancesAsync(Node parent, int? z = null, bool useSubThreads = false)
        {
            var manager = ManiaMapManager.Current;
            z ??= manager.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var rooms = manager.Layout.Rooms.Values.Where(x => x.Position.Z == z).ToList();
            var scenes = await LoadScenesAsync(rooms, useSubThreads);
            var result = new List<RoomNode2D>(rooms.Count);

            foreach (var room in rooms)
            {
                var scene = scenes[room.Template.Id];
                result.Add(RoomNode2D.CreateInstance(room.Id, scene, parent, true));
            }

            return result;
        }

        public List<RoomNode2D> CreateRoom2DInstances(Node parent, int? z = null)
        {
            var manager = ManiaMapManager.Current;
            z ??= manager.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var result = new List<RoomNode2D>();

            foreach (var room in manager.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var template = GetRoomTemplate(room.Template.Id);
                    result.Add(RoomNode2D.CreateInstance(room.Id, template.LoadScene(), parent, true));
                }
            }

            return result;
        }

        public async Task<RoomNode2D> CreateRoom2DInstanceAsync(Uid id, Node parent, bool assignLayoutPosition = false, bool useSubThreads = false)
        {
            var scene = await GetRoomTemplate(id).LoadSceneAsync(useSubThreads);
            return RoomNode2D.CreateInstance(id, scene, parent, assignLayoutPosition);
        }

        public RoomNode2D CreateRoom2DInstance(Uid id, Node parent, bool assignLayoutPosition = false)
        {
            return RoomNode2D.CreateInstance(id, GetRoomTemplate(id).LoadScene(), parent, assignLayoutPosition);
        }
    }
}