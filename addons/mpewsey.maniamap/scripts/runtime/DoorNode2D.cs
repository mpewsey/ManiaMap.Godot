using Godot;
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
    public partial class DoorNode2D : CellChild2D, IDoorNode
    {
        /// <summary>
        /// A dictionary of doors in the scene by room ID.
        /// </summary>
        private static Dictionary<Uid, LinkedList<DoorNode2D>> ActiveRoomDoors { get; } = new Dictionary<Uid, LinkedList<DoorNode2D>>();

        /// <inheritdoc/>
        [Export] public bool AutoAssignDirection { get; set; } = true;

        /// <inheritdoc/>
        [Export] public DoorDirection DoorDirection { get; set; }

        /// <inheritdoc/>
        [Export] public DoorType DoorType { get; set; }

        /// <inheritdoc/>
        [ExportGroup("Door Code")]
        [Export(PropertyHint.Flags, ManiaMapResources.Enums.DoorCodeFlags)] public int DoorCode { get; set; }

        /// <inheritdoc/>
        public DoorConnection DoorConnection { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            if (!Engine.IsEditorHint() && Room.IsInitialized)
                DoorConnection = this.FindDoorConnection();
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