using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    public partial class ManiaMapManager : GodotObject
    {
        public static ManiaMapManager Current { get; private set; }

        public Layout Layout { get; private set; }
        public LayoutState LayoutState { get; private set; }
        public ManiaMapSettings Settings { get; private set; }
        private Dictionary<Uid, List<DoorConnection>> RoomConnections { get; set; } = new Dictionary<Uid, List<DoorConnection>>();

        public static ManiaMapManager Initialize(Layout layout, LayoutState state, string settingsPath)
        {
            var settings = ResourceLoader.Load<ManiaMapSettings>(settingsPath);
            return Initialize(layout, state, settings);
        }

        public static ManiaMapManager Initialize(Layout layout, LayoutState state, ManiaMapSettings settings)
        {
            var manager = new ManiaMapManager()
            {
                Layout = layout,
                LayoutState = state,
                Settings = settings,
                RoomConnections = layout.GetRoomConnections(),
            };

            manager.SetAsCurrent();
            return manager;
        }

        private void SetAsCurrent()
        {
            Current = this;
        }

        public IReadOnlyList<DoorConnection> GetDoorConnections(Uid id)
        {
            if (RoomConnections.TryGetValue(id, out var connections))
                return connections;
            return Array.Empty<DoorConnection>();
        }
    }
}