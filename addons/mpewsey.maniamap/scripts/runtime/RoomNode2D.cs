using Godot;
using MPewsey.Game;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class RoomNode2D : Node2D
    {
        [Export] public bool RunAutoAssign { get; set; }
        [Export] public bool SaveRoomTemplate { get; set; }
        [Export] public RoomTemplateResource RoomTemplate { get; set; }
        [Export(PropertyHint.Range, "1,10,1,or_greater")] public int Rows { get; set; } = 1;
        [Export(PropertyHint.Range, "1,10,1,or_greater")] public int Columns { get; set; } = 1;
        [Export(PropertyHint.Range, "0,100,0.1,or_greater")] public Vector2 CellSize { get; set; } = new Vector2(20, 20);
        [Export] public Godot.Collections.Array<Godot.Collections.Array<bool>> ActiveCells { get; set; } = new Godot.Collections.Array<Godot.Collections.Array<bool>>();

        public Layout Layout { get; private set; }
        public LayoutState LayoutState { get; private set; }
        public Room RoomLayout { get; private set; }
        public RoomState RoomState { get; private set; }
        public IReadOnlyList<DoorConnection> DoorConnections { get; private set; }
        public bool IsInitialized { get; private set; }

        public override void _Ready()
        {
            base._Ready();

#if TOOLS
            if (Engine.IsEditorHint())
                return;
#endif

            if (!IsInitialized)
                throw new Exception($"Room is not initialized: {this}");
        }

#if TOOLS
        public override void _Process(double delta)
        {
            base._Process(delta);

            if (!Engine.IsEditorHint())
                return;

            SizeActiveCells();

            if (RunAutoAssign)
            {
                RunAutoAssign = false;
                AutoAssign();
            }
        }
#endif

        public static RoomNode2D CreateInstance(Uid id, string scenePath, Node parent)
        {
            var scene = ResourceLoader.Load<PackedScene>(scenePath);
            return CreateInstance(id, scene, parent);
        }

        public static RoomNode2D CreateInstance(Uid id, PackedScene scene, Node parent)
        {
            var manager = ManiaMapManager.Current;
            var layout = manager.Layout;
            var layoutState = manager.LayoutState;
            var roomLayout = layout.Rooms[id];
            var roomState = layoutState.RoomStates[id];
            var doorConnections = manager.GetDoorConnections(id);
            var collisionMask = manager.Settings.CellCollisionMask;
            var assignLayoutPosition = manager.Settings.AssignLayoutPosition;
            return CreateInstance(scene, parent, layout, layoutState, roomLayout, roomState, doorConnections, collisionMask, assignLayoutPosition);
        }

        public static RoomNode2D CreateInstance(PackedScene scene, Node parent, Layout layout, LayoutState layoutState,
            Room roomLayout, RoomState roomState, IReadOnlyList<DoorConnection> doorConnections,
            uint cellCollisionMask, bool assignLayoutPosition)
        {
            var room = scene.Instantiate<RoomNode2D>();
            room.Initialize(layout, layoutState, roomLayout, roomState, doorConnections, cellCollisionMask, assignLayoutPosition);
            parent.AddChild(room);
            return room;
        }

        public void Initialize(Layout layout, LayoutState layoutState, Room roomLayout, RoomState roomState,
            IReadOnlyList<DoorConnection> doorConnections, uint cellCollisionMask, bool assignLayoutPosition)
        {
            if (IsInitialized)
                throw new Exception($"Room is already initialized: {this}.");

            Layout = layout;
            LayoutState = layoutState;
            RoomLayout = roomLayout;
            RoomState = roomState;
            DoorConnections = doorConnections;

            if (assignLayoutPosition)
                MoveToLayoutPosition();

            CreateCellAreas(cellCollisionMask);
            IsInitialized = true;
        }

        public void AutoAssign()
        {
            SizeActiveCells();
            var nodes = FindChildren("*", nameof(CellChild2D));

            foreach (var node in nodes)
            {
                if (node is CellChild2D child)
                    child.AutoAssign(this);
            }
        }

        public void SizeActiveCells()
        {
            while (ActiveCells.Count > Rows)
            {
                ActiveCells.RemoveAt(ActiveCells.Count - 1);
            }

            foreach (var row in ActiveCells)
            {
                while (row.Count > Columns)
                {
                    row.RemoveAt(row.Count - 1);
                }

                while (row.Count < Columns)
                {
                    row.Add(false);
                }
            }

            while (ActiveCells.Count < Rows)
            {
                ActiveCells.Add(new Godot.Collections.Array<bool>(Enumerable.Repeat(false, Columns)));
            }
        }

        public Vector2I FindClosestCellIndex(Vector2 position)
        {
            var index = Vector2I.Zero;
            var minDistance = float.PositiveInfinity;

            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j])
                    {
                        var cellPosition = new Vector2(j * CellSize.X, i * CellSize.Y) + GlobalPosition;
                        var delta = cellPosition - position;
                        var distance = delta.LengthSquared();

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            index = new Vector2I(i, j);
                        }
                    }
                }
            }

            return index;
        }

        private void CreateCellAreas(uint cellCollisionMask)
        {
            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j])
                        CellArea2D.CreateInstance(i, j, this, cellCollisionMask);
                }
            }
        }

        public void MoveToLayoutPosition()
        {
            var position = RoomLayout.Position;
            Position = new Vector2(CellSize.X * position.Y, CellSize.Y * position.X);
        }
    }
}