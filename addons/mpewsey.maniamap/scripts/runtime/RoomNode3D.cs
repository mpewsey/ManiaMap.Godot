using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Exceptions;
using System;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A node serving as the top level of a 3D room.
    /// 
    /// See IRoomNodeExtensions for additional methods usable by this class.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomNode3DIcon)]
    public partial class RoomNode3D : Node3D, IRoomNode
    {
        /// <summary>
        /// Signal emitted when a cell area is entered by a tracked area or body.
        /// </summary>
        /// <param name="cell">The entered cell.</param>
        /// <param name="collision">The entering object.</param>
        [Signal] public delegate void OnCellAreaEnteredEventHandler(CellArea3D cell, Node collision);
        public Error EmitOnCellAreaEntered(CellArea3D cell, Node collision) => EmitSignal(SignalName.OnCellAreaEntered, cell, collision);

        /// <summary>
        /// Signal emitted when a cell area is exited by a tracked area or body.
        /// </summary>
        /// <param name="cell">The exited cell.</param>
        /// <param name="collision">The exiting object.</param>
        [Signal] public delegate void OnCellAreaExitedEventHandler(CellArea3D cell, Node collision);
        public Error EmitOnCellAreaExited(CellArea3D cell, Node collection) => EmitSignal(SignalName.OnCellAreaExited, cell, collection);

        /// <summary>
        /// Signal emitted when the cell grid size or cell sizes are set.
        /// </summary>
        [Signal] public delegate void OnCellGridChangedEventHandler();
        public Error EmitOnCellGridChanged() => EmitSignal(SignalName.OnCellGridChanged);

        /// <inheritdoc/>
        [Export] public RoomTemplateResource RoomTemplate { get; set; }

        private int _rows = 1;
        /// <inheritdoc/>
        [Export(PropertyHint.Range, "1,10,1,or_greater")] public int Rows { get => _rows; set => SetSizeField(ref _rows, value); }

        private int _columns = 1;
        /// <inheritdoc/>
        [Export(PropertyHint.Range, "1,10,1,or_greater")] public int Columns { get => _columns; set => SetSizeField(ref _columns, value); }

        private Vector3 _cellSize = Vector3.One;
        /// <summary>
        /// The width and height of the room cells.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,0.001,or_greater")] public Vector3 CellSize { get => _cellSize; set => SetCellSizeField(ref _cellSize, value); }

        /// <inheritdoc/>
        [Export] public Godot.Collections.Array<Godot.Collections.Array<bool>> ActiveCells { get; set; } = new Godot.Collections.Array<Godot.Collections.Array<bool>>();

        /// <inheritdoc/>
        public LayoutPack LayoutPack { get; private set; }

        /// <inheritdoc/>
        public Room RoomLayout { get; private set; }

        /// <inheritdoc/>
        public RoomState RoomState { get; private set; }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

#if TOOLS
        private bool MouseButtonPressed { get; set; }
        private Vector2 MouseButtonDownPosition { get; set; }
#endif

        private void SetSizeField(ref int field, int value)
        {
            field = value;
            this.SizeActiveCells();
            EmitOnCellGridChanged();
        }

        private void SetCellSizeField(ref Vector3 field, Vector3 value)
        {
            field = new Vector3(Mathf.Max(value.X, 0.001f), Mathf.Max(value.Y, 0.001f), Mathf.Max(value.Z, 0.001f));
            EmitOnCellGridChanged();
        }

        public override void _Ready()
        {
            base._Ready();

#if TOOLS
            if (Engine.IsEditorHint())
            {
                this.SizeActiveCells();
                Editor.CellGrid3DGizmo.CreateInstance(this);
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
                ProcessEditCellInputs();
        }

        private static bool DisplayCells()
        {
            return Editor.ManiaMapPlugin.PluginIsValid()
                && Editor.ManiaMapPlugin.Current.RoomNode3DToolbar.DisplayCells;
        }

        private static CellActivity CellEditMode()
        {
            if (!Editor.ManiaMapPlugin.PluginIsValid())
                return CellActivity.None;
            return Editor.ManiaMapPlugin.Current.RoomNode3DToolbar.CellEditMode;
        }

        private void ProcessEditCellInputs()
        {
            if (!DisplayCells())
                return;

            if (Input.IsMouseButtonPressed(MouseButton.Left) && CameraIsLookingDown() && MouseIsInsideMainScreen())
            {
                if (!MouseButtonPressed)
                    MouseButtonDownPosition = GetMousePosition();

                MouseButtonPressed = true;
                return;
            }

            if (MouseButtonPressed && CameraIsLookingDown() && MouseIsInsideMainScreen())
            {
                var mousePosition = GetMousePosition();
                var startPosition = new Vector3(MouseButtonDownPosition.X, 0, MouseButtonDownPosition.Y);
                var endPosition = new Vector3(mousePosition.X, 0, mousePosition.Y);
                var startIndex = GlobalPositionToCellIndex(startPosition);
                var endIndex = GlobalPositionToCellIndex(endPosition);
                this.SetCellActivities(startIndex, endIndex, CellEditMode());
                EmitOnCellGridChanged();
            }

            MouseButtonPressed = false;
        }

        private static Vector2 GetMousePosition()
        {
            var viewport = EditorInterface.Singleton.GetEditorViewport3D();
            var camera = viewport.GetCamera3D();
            var origin = camera.ProjectRayOrigin(viewport.GetMousePosition());
            return new Vector2(origin.X, origin.Z);
        }

        private static bool CameraIsLookingDown()
        {
            var camera = EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D();
            var cameraDirection = camera.GlobalRotationDegrees;
            return cameraDirection.IsEqualApprox(new Vector3(-90, 0, 0));
        }

        private static bool MouseIsInsideMainScreen()
        {
            var mainScreen = EditorInterface.Singleton.GetEditorMainScreen();
            var mousePosition = mainScreen.GetViewport().GetMousePosition();
            var mainScreenRect = new Rect2(mainScreen.GlobalPosition, mainScreen.Size);
            return mainScreenRect.Encloses(new Rect2(mousePosition, Vector2.Zero));
        }
#endif

        /// <summary>
        /// Creates and initializes an instance of a room.
        /// The layout and layout state are assigned to the room based on the current ManiaMapManager.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="scene">The room scene.</param>
        /// <param name="parent">The node to which the room will be added as a child.</param>
        /// <param name="assignLayoutPosition">If true, the position of the room will be set to that of the room layout. Otherwise, it will be initialized at its original position.</param>
        public static RoomNode3D CreateInstance(Uid id, LayoutPack layoutPack, PackedScene scene, Node parent, bool assignLayoutPosition = false)
        {
            var roomLayout = layoutPack.Layout.Rooms[id];
            var roomState = layoutPack.LayoutState.RoomStates[id];
            var collisionMask = layoutPack.Settings.CellCollisionMask;
            return CreateInstance(scene, parent, layoutPack, roomLayout, roomState, collisionMask, assignLayoutPosition);
        }

        /// <summary>
        /// Creates and initializes an instance of a room.
        /// </summary>
        /// <param name="scene">The room scene.</param>
        /// <param name="parent">The node to which the room will be added as a child.</param>
        /// <param name="layout">The full layout.</param>
        /// <param name="layoutState">The full layout state.</param>
        /// <param name="roomLayout">The room's layout.</param>
        /// <param name="roomState">The room's state.</param>
        /// <param name="doorConnections">The room's door connections.</param>
        /// <param name="cellCollisionMask">The cell collision mask.</param>
        /// <param name="assignLayoutPosition">If true, the position of the room will be set to that of the room layout. Otherwise, it will be initialized at its original position.</param>
        public static RoomNode3D CreateInstance(PackedScene scene, Node parent, LayoutPack layoutPack,
            Room roomLayout, RoomState roomState, uint cellCollisionMask, bool assignLayoutPosition)
        {
            var room = scene.Instantiate<RoomNode3D>();
            room.Initialize(layoutPack, roomLayout, roomState, cellCollisionMask, assignLayoutPosition);
            parent.AddChild(room);
            return room;
        }

        /// <summary>
        /// Initializes the room. The room should be initialized before adding it to the scene tree since cell children
        /// often require this information on ready.
        /// 
        /// If the room has already been initialized, no action is taken, and this method returns false.
        /// Othwerwise, it returns true.
        /// </summary>
        /// <param name="layout">The full layout.</param>
        /// <param name="layoutState">The full layout state.</param>
        /// <param name="roomLayout">The room's layout.</param>
        /// <param name="roomState">The room's state.</param>
        /// <param name="doorConnections">The room's door connections.</param>
        /// <param name="cellCollisionMask">The cell collision mask.</param>
        /// <param name="assignLayoutPosition">If true, the position of the room will be set to that of the room layout. Otherwise, it will be initialized at its original position.</param>
        public bool Initialize(LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            uint cellCollisionMask, bool assignLayoutPosition)
        {
            if (IsInitialized)
                return false;

            LayoutPack = layoutPack;
            RoomLayout = roomLayout;
            RoomState = roomState;

            if (assignLayoutPosition)
                MoveToLayoutPosition();

            CreateCellAreas(cellCollisionMask);
            IsInitialized = true;
            return true;
        }

        /// <summary>
        /// Creates CellArea3D for all active cells.
        /// </summary>
        /// <param name="cellCollisionMask">The cell collision mask.</param>
        private void CreateCellAreas(uint cellCollisionMask)
        {
            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j])
                        CellArea3D.CreateInstance(i, j, this, cellCollisionMask);
                }
            }
        }

        /// <summary>
        /// Returns the cell center global position for the specified cell index.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column.</param>
        public Vector3 CellCenterGlobalPosition(int row, int column)
        {
            return CellCenterLocalPosition(row, column) + GlobalPosition;
        }

        /// <summary>
        /// Returns the cell center local position for the specified cell index.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column.</param>
        public Vector3 CellCenterLocalPosition(int row, int column)
        {
            return new Vector3(column * CellSize.X, 0, row * CellSize.Z) + 0.5f * CellSize;
        }

        /// <summary>
        /// Returns the row (x) and column (y) index corresponding to the specified global position.
        /// If the position is outside the room bounds, returns Vector2I(-1, -1).
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2I GlobalPositionToCellIndex(Vector3 position)
        {
            return LocalPositionToCellIndex(position - GlobalPosition);
        }

        /// <summary>
        /// Returns the row (x) and column (y) index corresponding to the specified local position.
        /// If the position is outside the room bounds, returns Vector2I(-1, -1).
        /// </summary>
        /// <param name="position">The local position.</param>
        public Vector2I LocalPositionToCellIndex(Vector3 position)
        {
            var column = Mathf.FloorToInt(position.X / CellSize.X);
            var row = Mathf.FloorToInt(position.Z / CellSize.Z);

            if (this.CellIndexExists(row, column))
                return new Vector2I(row, column);

            return new Vector2I(-1, -1);
        }

        /// <summary>
        /// Returns the closest active cell index to the specified global position.
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2I FindClosestActiveCellIndex(Vector3 position)
        {
            var fastIndex = GlobalPositionToCellIndex(position);

            if (this.CellIndexExists(fastIndex.X, fastIndex.Y) && ActiveCells[fastIndex.X][fastIndex.Y])
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
                        var distance = new Vector2(delta.X, delta.Z).LengthSquared();

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

        /// <summary>
        /// Returns the closest door direction based on the cell index and specified global position.
        /// The nearest direction is determined based on the change between the specified position and cell center.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column.</param>
        /// <param name="position">The global position.</param>
        public DoorDirection FindClosestDoorDirection(int row, int column, Vector3 position)
        {
            Span<DoorDirection> directions = stackalloc DoorDirection[]
            {
                DoorDirection.North,
                DoorDirection.East,
                DoorDirection.South,
                DoorDirection.West,
                DoorDirection.Top,
                DoorDirection.Bottom,
            };

            Span<Vector3> vectors = stackalloc Vector3[]
            {
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };

            var index = 0;
            var maxDistance = float.NegativeInfinity;
            var delta = (position - CellCenterGlobalPosition(row, column)) / CellSize;

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

        /// <summary>
        /// Moves the room to its position in the `Layout`.
        /// </summary>
        public void MoveToLayoutPosition()
        {
            var position = RoomLayout.Position;
            Position = new Vector3(CellSize.X * position.Y, CellSize.Y * position.Z, CellSize.Z * position.X);
        }
    }
}