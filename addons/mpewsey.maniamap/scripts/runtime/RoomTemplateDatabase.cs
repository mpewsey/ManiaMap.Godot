using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Provides lookup of RoomTemplateResource by ID and initialization methods for rooms.
    /// 
    /// The database performs lazy initialization of its lookup dictionary.
    /// If you edit this resource at runtime, you should call the SetDirty method to ensure the database
    /// is updated before future queries.
    /// </summary>
    [GlobalClass]
    public partial class RoomTemplateDatabase : Resource
    {
        /// <summary>
        /// An array of template groups containing the rooms to include in queries.
        /// </summary>
        [Export] public TemplateGroup[] TemplateGroups { get; set; } = Array.Empty<TemplateGroup>();

        /// <summary>
        /// A dictionary of room templates by ID.
        /// </summary>
        private Dictionary<int, RoomTemplateResource> RoomTemplates { get; } = new Dictionary<int, RoomTemplateResource>();

        /// <summary>
        /// True if the object is currently dirty and requires update.
        /// </summary>
        public bool IsDirty { get; private set; } = true;

        /// <summary>
        /// Returns a read-only dictionary of room templates by ID.
        /// </summary>
        public IReadOnlyDictionary<int, RoomTemplateResource> GetRoomTemplates()
        {
            PopulateIfDirty();
            return RoomTemplates;
        }

        /// <summary>
        /// Sets the database as dirty and requiring update.
        /// </summary>
        public void SetDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Populates the lookup dictionary if the room is marked as dirty.
        /// </summary>
        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateRoomTemplates();
                IsDirty = false;
            }
        }

        /// <summary>
        /// Clears and populates the room templates dictionary.
        /// </summary>
        /// <exception cref="DuplicateIdException">Thrown if two different room templates have the same ID.</exception>
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
                        throw new DuplicateIdException($"Duplicate room template ID: {id}.");
                }
            }
        }

        /// <summary>
        /// Returns the room template for the specified ID.
        /// </summary>
        /// <param name="id">The template ID.</param>
        public RoomTemplateResource GetRoomTemplate(int id)
        {
            PopulateIfDirty();
            return RoomTemplates[id];
        }

        /// <summary>
        /// Returns the room template for the specified room ID.
        /// This method uses the layout from the current ManiaMapManager.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public RoomTemplateResource GetRoomTemplate(Uid id, LayoutPack layoutPack)
        {
            var room = layoutPack.Layout.Rooms[id];
            return GetRoomTemplate(room.Template.Id);
        }

        /// <summary>
        /// Loads the scenes for the specified rooms asynchronously and returns a dictionary of scenes by template ID.
        /// </summary>
        /// <param name="rooms">The rooms for which scenes will be loaded.</param>
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

        /// <summary>
        /// Asynchronously creates rooms at their layout positions for a layer of the current `Layout`.
        /// Returns a list of the instantiated rooms.
        /// </summary>
        /// <param name="parent">The node serving as the parent of the rooms.</param>
        /// <param name="z">The layer coordinate to instantiate.</param>
        public async Task<List<RoomNode2D>> CreateRoom2DInstancesAsync(Node parent, LayoutPack layoutPack, int? z = null, bool useSubThreads = false)
        {
            z ??= layoutPack.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var rooms = layoutPack.Layout.Rooms.Values.Where(x => x.Position.Z == z).ToList();
            var scenes = await LoadScenesAsync(rooms, useSubThreads);
            var result = new List<RoomNode2D>(rooms.Count);

            foreach (var room in rooms)
            {
                var scene = scenes[room.Template.Id];
                result.Add(RoomNode2D.CreateInstance(room.Id, layoutPack, scene, parent, true));
            }

            return result;
        }

        /// <summary>
        /// Asynchronously creates rooms at their layout positions for the current `Layout`.
        /// Returns a list of the instantiated rooms.
        /// </summary>
        /// <param name="parent">The node serving as the parent of the rooms.</param>
        public async Task<List<RoomNode3D>> CreateRoom3DInstancesAsync(Node parent, LayoutPack layoutPack, bool useSubThreads = false)
        {
            var rooms = layoutPack.Layout.Rooms.Values.ToList();
            var scenes = await LoadScenesAsync(rooms, useSubThreads);
            var result = new List<RoomNode3D>(rooms.Count);

            foreach (var room in rooms)
            {
                var scene = scenes[room.Template.Id];
                result.Add(RoomNode3D.CreateInstance(room.Id, layoutPack, scene, parent, true));
            }

            return result;
        }

        /// <summary>
        /// Creates rooms at their layout positions for a layer of the current `Layout`.
        /// Returns a list of the instantiated rooms.
        /// </summary>
        /// <param name="parent">The node serving as the parent of the rooms.</param>
        /// <param name="z">The layer coordinate to instantiate.</param>
        public List<RoomNode2D> CreateRoom2DInstances(Node parent, LayoutPack layoutPack, int? z = null)
        {
            z ??= layoutPack.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var result = new List<RoomNode2D>();

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var template = GetRoomTemplate(room.Template.Id);
                    result.Add(RoomNode2D.CreateInstance(room.Id, layoutPack, template.LoadScene(), parent, true));
                }
            }

            return result;
        }

        /// <summary>
        /// Creates rooms at their layout positions for the current `Layout`.
        /// Returns a list of the instantiated rooms.
        /// </summary>
        /// <param name="parent">The node serving as the parent of the rooms.</param>
        public List<RoomNode3D> CreateRoom3DInstances(Node parent, LayoutPack layoutPack)
        {
            var result = new List<RoomNode3D>();

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                var template = GetRoomTemplate(room.Template.Id);
                result.Add(RoomNode3D.CreateInstance(room.Id, layoutPack, template.LoadScene(), parent, true));
            }

            return result;
        }

        /// <summary>
        /// Asynchronously creates a room from the current `Layout`.
        /// Returns the instantiated room.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The node serving as the room's parent.</param>
        /// <param name="assignLayoutPosition">If true, the layout position will be assigned to the room.</param>
        public async Task<RoomNode2D> CreateRoom2DInstanceAsync(Uid id, LayoutPack layoutPack, Node parent, bool assignLayoutPosition = false, bool useSubThreads = false)
        {
            var scene = await GetRoomTemplate(id, layoutPack).LoadSceneAsync(useSubThreads);
            return RoomNode2D.CreateInstance(id, layoutPack, scene, parent, assignLayoutPosition);
        }

        /// <summary>
        /// Asynchronously creates a room from the current `Layout`.
        /// Returns the instantiated room.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The node serving as the room's parent.</param>
        /// <param name="assignLayoutPosition">If true, the layout position will be assigned to the room.</param>
        public async Task<RoomNode3D> CreateRoom3DInstanceAsync(Uid id, LayoutPack layoutPack, Node parent, bool assignLayoutPosition = false, bool useSubThreads = false)
        {
            var scene = await GetRoomTemplate(id, layoutPack).LoadSceneAsync(useSubThreads);
            return RoomNode3D.CreateInstance(id, layoutPack, scene, parent, assignLayoutPosition);
        }

        /// <summary>
        /// Creates a room from the current `Layout`.
        /// Returns the instantiated room.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The node serving as the room's parent.</param>
        /// <param name="assignLayoutPosition">If true, the layout position will be assigned to the room.</param>
        public RoomNode2D CreateRoom2DInstance(Uid id, LayoutPack layoutPack, Node parent, bool assignLayoutPosition = false)
        {
            var scene = GetRoomTemplate(id, layoutPack).LoadScene();
            return RoomNode2D.CreateInstance(id, layoutPack, scene, parent, assignLayoutPosition);
        }

        /// <summary>
        /// Creates a room from the current `Layout`.
        /// Returns the instantiated room.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The node serving as the room's parent.</param>
        /// <param name="assignLayoutPosition">If true, the layout position will be assigned to the room.</param>
        public RoomNode3D CreateRoom3DInstance(Uid id, LayoutPack layoutPack, Node parent, bool assignLayoutPosition = false)
        {
            var scene = GetRoomTemplate(id, layoutPack).LoadScene();
            return RoomNode3D.CreateInstance(id, layoutPack, scene, parent, assignLayoutPosition);
        }
    }
}