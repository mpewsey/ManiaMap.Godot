using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// The interface for a room node.
    /// </summary>
    public interface IRoomNode
    {
        /// <summary>
        /// The room template used by the procedural generator.
        /// </summary>
        public RoomTemplateResource RoomTemplate { get; set; }

        /// <summary>
        /// The number or cell rows in the room.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// The number of cell columns in the room.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// A nested array of room cell activities.
        /// </summary>
        public Godot.Collections.Array<Godot.Collections.Array<bool>> ActiveCells { get; set; }

        /// <summary>
        /// The current layout.
        /// </summary>
        public Layout Layout { get; }

        /// <summary>
        /// The current layout state.
        /// </summary>
        public LayoutState LayoutState { get; }

        /// <summary>
        /// This room's layout.
        /// </summary>
        public Room RoomLayout { get; }

        /// <summary>
        /// This room's layout state.
        /// </summary>
        public RoomState RoomState { get; }

        /// <summary>
        /// A list of door connections for the room.
        /// </summary>
        public IReadOnlyList<DoorConnection> DoorConnections { get; }

        /// <summary>
        /// True if the room has been initialized.
        /// </summary>
        public bool IsInitialized { get; }
    }
}