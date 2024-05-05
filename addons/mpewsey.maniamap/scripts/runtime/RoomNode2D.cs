using Godot;
using MPewsey.Common.Collections;
using MPewsey.Common.Mathematics;
using MPewsey.Game;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class RoomNode2D : Node2D, IRoomNode
    {
        [Export] public bool RunAutoAssign { get => false; set => AutoAssign(value); }
        [Export] public bool UpdateRoomTemplate { get => false; set => UpdateRoomTemplateResource(value); }
        [Export] public bool DisplayCells { get; set; } = true;
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

            QueueRedraw();
            SizeActiveCells();
        }

        public override void _Draw()
        {
            base._Draw();

            if (!Engine.IsEditorHint())
                return;

            if (DisplayCells)
                DrawCells();
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

        public void AutoAssign(bool run = true)
        {
            if (!run)
                return;

            RoomTemplate ??= new RoomTemplateResource();
            RoomTemplate.Id = ManiaMapManager.AutoAssignId(RoomTemplate.Id);
            SizeActiveCells();
            var nodes = FindChildren("*", nameof(CellChild2D));

            foreach (var node in nodes)
            {
                var child = (CellChild2D)node;
                child.AutoAssign(this);
            }

            GD.Print($"Performed auto assign on {nodes.Count} cell children.");
        }

        public void ToggleCellActivity(Vector2I index)
        {
            if ((uint)index.X < (uint)ActiveCells.Count && (uint)index.Y < (uint)ActiveCells[index.X].Count)
                ActiveCells[index.X][index.Y] = !ActiveCells[index.X][index.Y];
        }

        private void DrawCells()
        {
            var activeFillColor = new Color(0, 0, 1, 0.1f);
            var inactiveFillColor = new Color(1, 0, 0, 0.1f);
            var activeLineColor = activeFillColor with { A = 1 };
            var inactiveLineColor = inactiveFillColor with { A = 1 };
            DrawCellLayer(inactiveFillColor, inactiveLineColor, false);
            DrawCellLayer(activeFillColor, activeLineColor, true);
        }

        private void DrawCellLayer(Color fillColor, Color lineColor, bool active)
        {
            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == active)
                    {
                        var position = CellPosition(i, j);
                        var rect = new Rect2(position, CellSize);
                        DrawRect(rect, fillColor);
                        DrawRect(rect, lineColor, false);
                    }
                }
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
                    row.Add(true);
                }
            }

            while (ActiveCells.Count < Rows)
            {
                ActiveCells.Add(new Godot.Collections.Array<bool>(Enumerable.Repeat(true, Columns)));
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
                        var delta = CellPosition(i, j) - position;
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

        public Vector2 CellPosition(Vector2I index)
        {
            return CellPosition(index.X, index.Y);
        }

        public Vector2 CellPosition(int row, int column)
        {
            return new Vector2(column * CellSize.X, row * CellSize.Y) + GlobalPosition;
        }

        public Vector2I PointToCellIndex(Vector2 position)
        {
            position -= GlobalPosition;
            var column = Mathf.FloorToInt(position.X / CellSize.X);
            var row = Mathf.FloorToInt(position.Y / CellSize.Y);

            if ((uint)row < (uint)Rows && (uint)column < (uint)Columns)
                return new Vector2I(row, column);

            return new Vector2I(-1, -1);
        }

        public RoomTemplate CreateRoomTemplate(int id, string name)
        {
            var cells = CreateCellTemplates();
            AddDoorTemplates(cells);
            AddFeatureTemplates(cells);
            var spots = CreateCollectableSpotTemplates();
            var template = new RoomTemplate(id, name, cells, spots);
            template.Validate();
            ValidateRoomFlags();
            return template;
        }

        private HashMap<int, CollectableSpot> CreateCollectableSpotTemplates()
        {
            var nodes = FindChildren("*", nameof(CollectableSpot2D));
            var result = new HashMap<int, CollectableSpot>();

            foreach (var node in nodes)
            {
                var spot = (CollectableSpot2D)node;
                var index = new Vector2DInt(spot.CellIndex.X, spot.CellIndex.Y);
                result.Add(spot.Id, new CollectableSpot(index, spot.CollectableGroup.GroupName, spot.Weight));
            }

            return result;
        }

        private void AddDoorTemplates(Array2D<Cell> cells)
        {
            var nodes = FindChildren("*", nameof(DoorNode2D));

            foreach (var node in nodes)
            {
                var door = (DoorNode2D)node;
                var cell = cells[door.CellIndex.X, door.CellIndex.Y];
                cell.SetDoor(door.Direction, new Door(door.Type, (DoorCode)door.Code));
            }
        }

        private Array2D<Cell> CreateCellTemplates()
        {
            var cells = new Array2D<Cell>(Rows, Columns);

            for (int i = 0; i < Rows; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < Columns; j++)
                {
                    if (row[j])
                        cells[i, j] = Cell.New;
                }
            }

            return cells;
        }

        private void AddFeatureTemplates(Array2D<Cell> cells)
        {
            var nodes = FindChildren("*", nameof(Feature2D));

            foreach (var node in nodes)
            {
                var feature = (Feature2D)node;
                var cell = cells[feature.CellIndex.X, feature.CellIndex.Y];
                cell.AddFeature(feature.FeatureName);
            }
        }

        private void ValidateRoomFlags()
        {
            var nodes = FindChildren("*", nameof(RoomFlag2D));
            var flags = new HashSet<int>(nodes.Count);

            foreach (var node in nodes)
            {
                var flag = (RoomFlag2D)node;

                if (!flags.Add(flag.Id))
                    throw new Exception($"Duplicate room flag: {flag}.");
            }
        }

        public void UpdateRoomTemplateResource(bool run = true)
        {
            if (!run)
                return;

            if (!SceneIsSavedToFile())
            {
                GD.PrintErr("Scene must be saved to file first.");
                return;
            }

            AutoAssign();
            RoomTemplate.Initialize(this);

            if (!ResourceIsSavedToFile(RoomTemplate))
            {
                var path = SceneFilePath.GetBaseName() + ".room_template.tres";
                ResourceSaver.Save(RoomTemplate, path);
                RoomTemplate = ResourceLoader.Load<RoomTemplateResource>(path);
                GD.Print($"Saved room template to: {path}");
            }

            GD.Print("Room template updated.");
        }

        private bool ResourceIsSavedToFile(Resource resource)
        {
            var path = resource.ResourcePath;
            return !string.IsNullOrEmpty(path) && !path.StartsWith(SceneFilePath);
        }

        private bool SceneIsSavedToFile()
        {
            return !string.IsNullOrEmpty(SceneFilePath);
        }
    }
}