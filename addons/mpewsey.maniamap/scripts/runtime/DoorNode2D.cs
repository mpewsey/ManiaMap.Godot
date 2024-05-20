using Godot;
using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible door location connecting two RoomNode2D.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.DoorNode2DIcon)]
    public partial class DoorNode2D : CellChild2D
    {
        /// <summary>
        /// A dictionary of doors in the scene by room ID.
        /// </summary>
        private static Dictionary<Uid, LinkedList<DoorNode2D>> ActiveRoomDoors { get; } = new Dictionary<Uid, LinkedList<DoorNode2D>>();

        /// <summary>
        /// If true, auto assigns the door direction based on its position when auto assign is run.
        /// Disable this flag if you wish to control this value manually.
        /// </summary>
        [Export] public bool AutoAssignDirection { get; set; } = true;

        /// <summary>
        /// The direction where the door leads relative to its containing cell.
        /// </summary>
        [Export] public DoorDirection DoorDirection { get; set; }

        /// <summary>
        /// The door type. See the ManiaMap DoorType documentation for more information.
        /// </summary>
        [Export] public DoorType DoorType { get; set; }

        /// <summary>
        /// The door code to which this door may connect. Door codes can connect if they intersect.
        /// </summary>
        [ExportGroup("Door Code")]
        [Export(PropertyHint.Flags, ManiaMapResources.Enums.DoorCodeFlags)] public int DoorCode { get; set; }

        /// <summary>
        /// The door's door and room connection information.
        /// </summary>
        public DoorConnection DoorConnection { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            if (!Engine.IsEditorHint() && Room.IsInitialized)
                DoorConnection = FindDoorConnection();
        }

        public override void _EnterTree()
        {
            base._EnterTree();

            if (!Engine.IsEditorHint())
                AddToActiveRoomDoors();
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            if (!Engine.IsEditorHint())
                RemoveFromActiveRoomDoors();
        }

        /// <summary>
        /// Returns the ManiaMap door used for procedural generation.
        /// </summary>
        public Door GetMMDoor()
        {
            return new Door(DoorType, (DoorCode)DoorCode);
        }

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);

            if (AutoAssignDirection)
                DoorDirection = room.FindClosestDoorDirection(Row, Column, GlobalPosition);
        }

        /// <summary>
        /// Returns true if a connection exists within the `Layout` for the door.
        /// </summary>
        public bool DoorExists()
        {
            return DoorConnection != null;
        }

        /// <summary>
        /// Returns the associated door connection within the `Layout`.
        /// If the door does not have a connect, returns null.
        /// </summary>
        private DoorConnection FindDoorConnection()
        {
            var roomId = Room.RoomLayout.Id;
            var position = new Vector2DInt(Row, Column);

            foreach (var connection in Room.DoorConnections)
            {
                if (connection.ContainsDoor(roomId, position, DoorDirection))
                    return connection;
            }

            return null;
        }

        /// <summary>
        /// Returns the ID of the room to which this door connects.
        /// If this door does not exist, returns Uid(-1, -1, -1).
        /// </summary>
        public Uid ToRoomId()
        {
            if (!DoorExists())
                return new Uid(-1, -1, -1);

            return DoorConnection.GetConnectingRoom(Room.RoomLayout.Id);
        }

        /// <summary>
        /// Adds the door to the active room doors dictionary.
        /// </summary>
        private void AddToActiveRoomDoors()
        {
            if (Room.IsInitialized)
            {
                var roomId = Room.RoomLayout.Id;

                if (!ActiveRoomDoors.TryGetValue(roomId, out var doors))
                {
                    doors = new LinkedList<DoorNode2D>();
                    ActiveRoomDoors.Add(roomId, doors);
                }

                doors.AddLast(this);
            }
        }

        /// <summary>
        /// Removes the door from the active room doors dictionary.
        /// </summary>
        private void RemoveFromActiveRoomDoors()
        {
            if (Room.IsInitialized)
            {
                var roomId = Room.RoomLayout.Id;

                if (ActiveRoomDoors.TryGetValue(roomId, out var doors))
                {
                    doors.Remove(this);

                    if (doors.Count == 0)
                        ActiveRoomDoors.Remove(roomId);
                }
            }
        }

        /// <summary>
        /// Returns the door in the scene with the matching room ID and door connection.
        /// </summary>
        /// <param name="roomId">The door's room ID.</param>
        /// <param name="doorConnection">The door's connection.</param>
        public static DoorNode2D FindActiveDoor(Uid roomId, DoorConnection doorConnection)
        {
            if (doorConnection != null && ActiveRoomDoors.TryGetValue(roomId, out var doors))
            {
                foreach (var door in doors)
                {
                    if (door.DoorConnection == doorConnection)
                        return door;
                }
            }

            return null;
        }
    }
}