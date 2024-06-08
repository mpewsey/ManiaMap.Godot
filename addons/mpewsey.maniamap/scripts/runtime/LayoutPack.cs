using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Holds the current `Layout` and `LayoutState`.
    /// </summary>
    public class LayoutPack
    {
        /// <summary>
        /// The layout.
        /// </summary>
        public Layout Layout { get; }

        /// <summary>
        /// The layout state.
        /// </summary>
        public LayoutState LayoutState { get; }

        /// <summary>
        /// The settings.
        /// </summary>
        public ManiaMapSettings Settings { get; }

        /// <summary>
        /// A dictionary of door connections by room ID.
        /// </summary>
        private Dictionary<Uid, List<DoorConnection>> RoomConnections { get; } = new Dictionary<Uid, List<DoorConnection>>();

        /// <summary>
        /// Initializes a new layout pack.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        /// <param name="settingsPath">The path to the ManiaMapSettings resource.</param>
        public LayoutPack(Layout layout, LayoutState state, string settingsPath)
            : this(layout, state, ResourceLoader.Load<ManiaMapSettings>(settingsPath))
        {

        }

        /// <summary>
        /// Initializes a new layout pack.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        /// <param name="settings">The settings resource. If null, a new default instance of the settings will be used.</param>
        /// <exception cref="ArgumentNullException">Thrown if the layout or layout state is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the layout and layout state ID's do not match.</exception>
        public LayoutPack(Layout layout, LayoutState state, ManiaMapSettings settings = null)
        {
            ArgumentNullException.ThrowIfNull(layout, nameof(layout));
            ArgumentNullException.ThrowIfNull(state, nameof(state));

            if (layout.Id != state.Id)
                throw new ArgumentException($"Layout and layout state ID's do not match: (Layout ID = {layout.Id}, Layout State ID = {state.Id})");

            Layout = layout;
            LayoutState = state;
            Settings = settings;
            RoomConnections = layout.GetRoomConnections();
        }

        /// <summary>
        /// Returns a list of door connections with a room for the specified room ID.
        /// If the room ID does not exist, returns an empty list.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public IReadOnlyList<DoorConnection> GetDoorConnections(Uid id)
        {
            if (RoomConnections.TryGetValue(id, out var connections))
                return connections;
            return Array.Empty<DoorConnection>();
        }
    }
}