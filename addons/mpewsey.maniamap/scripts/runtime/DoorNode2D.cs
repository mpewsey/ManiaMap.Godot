using Godot;
using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.DoorNode2DIcon)]
    public partial class DoorNode2D : CellChild2D
    {
        private static Dictionary<Uid, LinkedList<DoorNode2D>> ActiveRoomDoors { get; } = new Dictionary<Uid, LinkedList<DoorNode2D>>();

        [Export] public bool AutoAssignDirection { get; set; } = true;
        [Export] public DoorDirection DoorDirection { get; set; }
        [Export] public DoorType DoorType { get; set; }

        [ExportGroup("Door Code")]
        [Export(PropertyHint.Flags, ManiaMapResources.Enums.DoorCodeFlags)] public int DoorCode { get; set; }

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

        public Door GetMMDoor()
        {
            return new Door(DoorType, (DoorCode)DoorCode);
        }

        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);

            if (AutoAssignDirection)
                DoorDirection = room.FindClosestDoorDirection(Row, Column, GlobalPosition);
        }

        public bool DoorExists()
        {
            return DoorConnection != null;
        }

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

        public Uid ToRoomId()
        {
            if (!DoorExists())
                return new Uid(-1, -1, -1);

            return DoorConnection.GetConnectingRoom(Room.RoomLayout.Id);
        }

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