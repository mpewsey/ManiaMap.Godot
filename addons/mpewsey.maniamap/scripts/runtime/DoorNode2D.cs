using Godot;
using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class DoorNode2D : CellChild2D
    {
        public const string DoorCodeFlags = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
        private static Dictionary<Uid, LinkedList<DoorNode2D>> ActiveRoomDoors { get; } = new Dictionary<Uid, LinkedList<DoorNode2D>>();

        [Export] public bool AutoAssignDirection { get; set; } = true;
        [Export] public DoorDirection Direction { get; set; }
        [Export] public DoorType Type { get; set; }

        [ExportGroup("Code")]
        [Export(PropertyHint.Flags, DoorCodeFlags)] public int Code { get; set; }
        public DoorConnection DoorConnection { get; private set; }

        public override void _Ready()
        {
            base._Ready();

#if TOOLS
            if (Engine.IsEditorHint())
                return;
#endif

            DoorConnection = FindDoorConnection();
        }

        public override void _EnterTree()
        {
            base._EnterTree();

#if TOOLS
            if (Engine.IsEditorHint())
                return;
#endif

            AddToActiveRoomDoors();
        }

        public override void _ExitTree()
        {
            base._ExitTree();

#if TOOLS
            if (Engine.IsEditorHint())
                return;
#endif

            RemoveFromActiveRoomDoors();
        }

        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);

            if (AutoAssignDirection)
                Direction = FindClosestDirection(room.CellPosition(CellIndex));
        }

        public bool DoorExists()
        {
            return DoorConnection != null;
        }

        private DoorConnection FindDoorConnection()
        {
            var roomId = Room.RoomLayout.Id;
            var position = new Vector2DInt(CellIndex.X, CellIndex.Y);

            foreach (var connection in Room.DoorConnections)
            {
                if (connection.ContainsDoor(roomId, position, Direction))
                    return connection;
            }

            return null;
        }

        public Uid ToRoomId()
        {
            if (DoorExists())
            {
                var roomId = Room.RoomLayout.Id;

                if (DoorConnection.FromRoom == roomId)
                    return DoorConnection.ToRoom;
                if (DoorConnection.ToRoom == roomId)
                    return DoorConnection.FromRoom;
            }

            return new Uid(-1, -1, -1);
        }

        private void AddToActiveRoomDoors()
        {
            var roomId = Room.RoomLayout.Id;

            if (!ActiveRoomDoors.TryGetValue(roomId, out var doors))
            {
                doors = new LinkedList<DoorNode2D>();
                ActiveRoomDoors.Add(roomId, doors);
            }

            doors.AddLast(this);
        }

        private void RemoveFromActiveRoomDoors()
        {
            var roomId = Room.RoomLayout.Id;

            if (ActiveRoomDoors.TryGetValue(roomId, out var doors))
            {
                if (doors.Remove(this) && doors.Count == 0)
                    ActiveRoomDoors.Remove(roomId);
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

        public DoorDirection FindClosestDirection(Vector2 position)
        {
            Span<DoorDirection> directions = stackalloc DoorDirection[]
            {
                DoorDirection.North,
                DoorDirection.East,
                DoorDirection.South,
                DoorDirection.West,
            };

            Span<Vector2> vectors = stackalloc Vector2[]
            {
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(-1, 0),
            };

            var index = 0;
            var maxDistance = float.NegativeInfinity;
            var delta = GlobalPosition - position;

            for (int i = 0; i < vectors.Length; i++)
            {
                if (position.Dot(vectors[i]) > maxDistance)
                    index = i;
            }

            return directions[index];
        }
    }
}