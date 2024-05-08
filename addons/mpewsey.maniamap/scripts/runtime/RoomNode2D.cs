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
        [Signal] public delegate void OnCellAreaEnteredEventHandler(CellArea2D cell, Node collision);
        public Error EmitOnCellAreaEntered(CellArea2D cell, Node collision) => EmitSignal(SignalName.OnCellAreaEntered, cell, collision);

#if TOOLS
        [Export] public bool RunAutoAssign { get => false; set => AutoAssign(value); }
        [Export] public bool UpdateRoomTemplate { get => false; set => UpdateRoomTemplateResource(value); }
        [Export] public bool DisplayCells { get; set; } = true;
        [Export] public CellActivity CellEditMode { get; set; }
        private bool MouseButtonPressed { get; set; }
        private Vector2 MouseButtonDownPosition { get; set; }
#endif

        [Export] public RoomTemplateResource RoomTemplate { get; set; }

        private int _rows = 1;
        [Export(PropertyHint.Range, "1,10,1,or_greater")] public int Rows { get => _rows; set => SetSizeField(ref _rows, value); }

        private int _columns = 1;
        [Export(PropertyHint.Range, "1,10,1,or_greater")] public int Columns { get => _columns; set => SetSizeField(ref _columns, value); }

        [Export(PropertyHint.Range, "0,100,0.1,or_greater")] public Vector2 CellSize { get; set; } = new Vector2(20, 20);
        [Export] public Godot.Collections.Array<Godot.Collections.Array<bool>> ActiveCells { get; set; } = new Godot.Collections.Array<Godot.Collections.Array<bool>>();

        public Layout Layout { get; private set; }
        public LayoutState LayoutState { get; private set; }
        public Room RoomLayout { get; private set; }
        public RoomState RoomState { get; private set; }
        public IReadOnlyList<DoorConnection> DoorConnections { get; private set; }
        public bool IsInitialized { get; private set; }

        private void SetSizeField<T>(ref T field, T value)
        {
            field = value;
            SizeActiveCells();
        }

        public override void _Ready()
        {
            base._Ready();

            if (!Engine.IsEditorHint())
            {
                if (!IsInitialized)
                    throw new Exception($"Room is not initialized: {this}");
            }
        }

#if TOOLS
        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.ActiveCells)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Engine.IsEditorHint())
            {
                QueueRedraw();
                SizeActiveCells();

                if (DisplayCells)
                    ProcessEditCellInputs();
            }
        }

        private void ProcessEditCellInputs()
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                if (!MouseButtonPressed)
                    MouseButtonDownPosition = GetViewport().GetMousePosition();

                MouseButtonPressed = true;
                return;
            }

            if (MouseButtonPressed)
                SetCellActivities(MouseButtonDownPosition, GetViewport().GetMousePosition(), CellEditMode);

            MouseButtonPressed = false;
        }

        public override void _Draw()
        {
            base._Draw();

            if (Engine.IsEditorHint())
            {
                if (DisplayCells)
                    DrawCells();
            }
        }

        private void DrawCells()
        {
            var activeFillColor = new Color(0, 0, 1, 0.1f);
            var inactiveFillColor = new Color(1, 0, 0, 0.1f);
            var activeLineColor = new Color(0.5f, 0.5f, 0.5f);
            var inactiveLineColor = new Color(0.5f, 0.5f, 0.5f);
            DrawCellRects(inactiveFillColor, inactiveLineColor, false);
            DrawCellXs(inactiveLineColor, false);
            DrawCellRects(activeFillColor, activeLineColor, true);
        }

        private void DrawCellRects(Color fillColor, Color lineColor, bool active)
        {
            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == active)
                    {
                        var rect = new Rect2(CellCenterGlobalPosition(i, j) - 0.5f * CellSize, CellSize);
                        DrawRect(rect, fillColor);
                        DrawRect(rect, lineColor, false);
                    }
                }
            }
        }

        private void DrawCellXs(Color lineColor, bool active)
        {
            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == active)
                    {
                        var topLeft = CellCenterGlobalPosition(i, j) - 0.5f * CellSize;
                        var bottomRight = topLeft + CellSize;
                        DrawLine(topLeft, bottomRight, lineColor);
                        DrawLine(new Vector2(topLeft.X, bottomRight.Y), new Vector2(bottomRight.X, topLeft.Y), lineColor);
                    }
                }
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

        public void AutoAssign(bool run = true)
        {
            if (!run)
                return;

            RoomTemplate ??= new RoomTemplateResource();
            RoomTemplate.Id = Rand.AutoAssignId(RoomTemplate.Id);
            SizeActiveCells();
            var nodes = FindChildren("*", nameof(CellChild2D));

            foreach (var node in nodes)
            {
                var child = (CellChild2D)node;
                child.AutoAssign(this);
            }

            GD.Print($"Performed auto assign on {nodes.Count} cell children.");
        }

        public void SetCellActivities(Vector2 startPosition, Vector2 endPosition, CellActivity activity)
        {
            var startIndex = GlobalPositionToCellIndex(startPosition);
            var endIndex = GlobalPositionToCellIndex(endPosition);
            var outsideIndex = new Vector2I(-1, -1);

            if (activity != CellActivity.None && startIndex != outsideIndex && endIndex != outsideIndex)
            {
                var startRow = Mathf.Min(startIndex.X, endIndex.X);
                var endRow = Mathf.Max(startIndex.X, endIndex.X);
                var startColumn = Mathf.Min(startIndex.Y, endIndex.Y);
                var endColumn = Mathf.Max(startIndex.Y, endIndex.Y);

                for (int i = startRow; i <= endRow; i++)
                {
                    for (int j = startColumn; j <= endColumn; j++)
                    {
                        switch (activity)
                        {
                            case CellActivity.Activate:
                                ActiveCells[i][j] = true;
                                break;
                            case CellActivity.Deactivate:
                                ActiveCells[i][j] = false;
                                break;
                            case CellActivity.Toggle:
                                ActiveCells[i][j] = !ActiveCells[i][j];
                                break;
                            default:
                                throw new NotImplementedException($"Unhandled cell activity: {activity}.");
                        }
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
                        var delta = CellCenterGlobalPosition(i, j) - position;
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

        public Vector2 CellCenterGlobalPosition(int row, int column)
        {
            return CellCenterLocalPosition(row, column) + GlobalPosition;
        }

        public Vector2 CellCenterLocalPosition(int row, int column)
        {
            return new Vector2(column * CellSize.X, row * CellSize.Y) + 0.5f * CellSize;
        }

        public Vector2I GlobalPositionToCellIndex(Vector2 position)
        {
            return LocalPositionToCellIndex(position - GlobalPosition);
        }

        public Vector2I LocalPositionToCellIndex(Vector2 position)
        {
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
                var index = new Vector2DInt(spot.Row, spot.Column);
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
                var cell = cells[door.Row, door.Column];
                cell.SetDoor(door.Direction, new Door(door.Type, (DoorCode)door.Code));
            }
        }

        private Array2D<Cell> CreateCellTemplates()
        {
            SizeActiveCells();
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
                var cell = cells[feature.Row, feature.Column];
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