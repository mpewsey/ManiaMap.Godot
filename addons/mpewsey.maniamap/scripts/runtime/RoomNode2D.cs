using Godot;
using MPewsey.Common.Collections;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using MPewsey.ManiaMapGodot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomNode2DIcon)]
    public partial class RoomNode2D : Node2D, IRoomNode
    {
        [Signal] public delegate void OnCellAreaEnteredEventHandler(CellArea2D cell, Node collision);
        public Error EmitOnCellAreaEntered(CellArea2D cell, Node collision) => EmitSignal(SignalName.OnCellAreaEntered, cell, collision);

        [Signal] public delegate void OnCellAreaExitedEventHandler(CellArea2D cell, Node collision);
        public Error EmitOnCellAreaExited(CellArea2D cell, Node collection) => EmitSignal(SignalName.OnCellAreaExited, cell, collection);

        [Signal] public delegate void OnCellGridChangedEventHandler();
        public Error EmitOnCellGridChanged() => EmitSignal(SignalName.OnCellGridChanged);

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

        private Vector2 _cellSize = new Vector2(96, 96);
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public Vector2 CellSize { get => _cellSize; set => SetCellSizeField(ref _cellSize, value); }

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
            EmitOnCellGridChanged();
        }

        private void SetCellSizeField<T>(ref T field, T value)
        {
            field = value;
            EmitOnCellGridChanged();
        }

        public override void _Ready()
        {
            base._Ready();

#if TOOLS
            if (Engine.IsEditorHint())
            {
                SizeActiveCells();
                Editor.CellGrid2D.CreateInstance(this);
                return;
            }
#endif

            if (!IsInitialized)
                throw new RoomNotInitializedException($"Room is not initialized: {this}");
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();
            var usage = property["usage"].As<PropertyUsageFlags>();

            if (name == PropertyName.ActiveCells)
                property["usage"] = (int)(usage & ~PropertyUsageFlags.Editor);
        }

#if TOOLS
        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Engine.IsEditorHint())
            {
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
            {
                var mousePosition = GetViewport().GetMousePosition();
                SetCellActivities(MouseButtonDownPosition, mousePosition, CellEditMode);
                EmitOnCellGridChanged();
            }

            MouseButtonPressed = false;
        }
#endif

        public static RoomNode2D CreateInstance(Uid id, PackedScene scene, Node parent, bool assignLayoutPosition = false)
        {
            var manager = ManiaMapManager.Current;
            var layout = manager.Layout;
            var layoutState = manager.LayoutState;
            var roomLayout = layout.Rooms[id];
            var roomState = layoutState.RoomStates[id];
            var doorConnections = manager.GetDoorConnections(id);
            var collisionMask = manager.Settings.CellCollisionMask;
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

        public bool Initialize(Layout layout, LayoutState layoutState, Room roomLayout, RoomState roomState,
            IReadOnlyList<DoorConnection> doorConnections, uint cellCollisionMask, bool assignLayoutPosition)
        {
            if (IsInitialized)
                return false;

            Layout = layout;
            LayoutState = layoutState;
            RoomLayout = roomLayout;
            RoomState = roomState;
            DoorConnections = doorConnections;

            if (assignLayoutPosition)
                MoveToLayoutPosition();

            CreateCellAreas(cellCollisionMask);
            IsInitialized = true;
            return true;
        }

        public bool AutoAssign(bool run = true)
        {
            if (!run)
                return false;

            RoomTemplate ??= new RoomTemplateResource() { TemplateName = Name };
            RoomTemplate.Id = Rand.AutoAssignId(RoomTemplate.Id);
            SizeActiveCells();
            var nodes = FindChildren("*", nameof(CellChild2D), true, false);

            foreach (var node in nodes)
            {
                var child = (CellChild2D)node;
                child.AutoAssign(this);
            }

            GD.PrintRich($"[color=#00ff00]Performed auto assign on {nodes.Count} cell children.[/color]");
            return true;
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
                    var row = ActiveCells[i];

                    for (int j = startColumn; j <= endColumn; j++)
                    {
                        switch (activity)
                        {
                            case CellActivity.Activate:
                                row[j] = true;
                                break;
                            case CellActivity.Deactivate:
                                row[j] = false;
                                break;
                            case CellActivity.Toggle:
                                row[j] = !row[j];
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

        public Vector2I FindClosestActiveCellIndex(Vector2 position)
        {
            var fastIndex = GlobalPositionToCellIndex(position);

            if (CellIndexExists(fastIndex.X, fastIndex.Y) && ActiveCells[fastIndex.X][fastIndex.Y])
                return fastIndex;

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

        public DoorDirection FindClosestDoorDirection(int row, int column, Vector2 position)
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
                new Vector2(0, -1),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(-1, 0),
            };

            var index = 0;
            var maxDistance = float.NegativeInfinity;
            var delta = position - CellCenterGlobalPosition(row, column);

            for (int i = 0; i < vectors.Length; i++)
            {
                var distance = delta.Dot(vectors[i]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    index = i;
                }
            }

            return directions[index];
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

            if (CellIndexExists(row, column))
                return new Vector2I(row, column);

            return new Vector2I(-1, -1);
        }

        public bool CellIndexExists(int row, int column)
        {
            return (uint)row < (uint)Rows && (uint)column < (uint)Columns;
        }

        public RoomTemplate GetMMRoomTemplate(int id, string name)
        {
            var cells = GetMMCells();
            AddMMDoors(cells);
            AddMMFeatures(cells);
            var spots = GetMMCollectableSpots();
            var template = new RoomTemplate(id, name, cells, spots);
            template.Validate();
            ValidateRoomFlags();
            return template;
        }

        private HashMap<int, CollectableSpot> GetMMCollectableSpots()
        {
            var nodes = FindChildren("*", nameof(CollectableSpot2D), true, false);
            var result = new HashMap<int, CollectableSpot>();

            foreach (var node in nodes)
            {
                var spot = (CollectableSpot2D)node;
                result.Add(spot.Id, spot.GetMMCollectableSpot());
            }

            return result;
        }

        private void AddMMDoors(Array2D<Cell> cells)
        {
            var nodes = FindChildren("*", nameof(DoorNode2D), true, false);

            foreach (var node in nodes)
            {
                var door = (DoorNode2D)node;
                var cell = cells[door.Row, door.Column];
                cell.SetDoor(door.DoorDirection, door.GetMMDoor());
            }
        }

        private Array2D<Cell> GetMMCells()
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

        private void AddMMFeatures(Array2D<Cell> cells)
        {
            var nodes = FindChildren("*", nameof(Feature2D), true, false);

            foreach (var node in nodes)
            {
                var feature = (Feature2D)node;
                var cell = cells[feature.Row, feature.Column];
                cell.AddFeature(feature.FeatureName);
            }
        }

        public void ValidateRoomFlags()
        {
            var nodes = FindChildren("*", nameof(RoomFlag2D), true, false);
            var flags = new HashSet<int>(nodes.Count);

            foreach (var node in nodes)
            {
                var flag = (RoomFlag2D)node;

                if (!flags.Add(flag.Id))
                    throw new DuplicateIdException($"Duplicate room flag ID: (ID = {flag.Id}, Path = {GetPathTo(flag)}.");
            }
        }

        public bool UpdateRoomTemplateResource(bool run = true)
        {
            if (!run)
                return false;

            if (!SceneIsSavedToFile())
            {
                GD.PrintErr("Scene must be saved to file first.");
                return false;
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
            return true;
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