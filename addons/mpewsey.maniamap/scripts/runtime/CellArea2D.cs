using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Provides area and body entering and exiting detection for a cell.
    /// Register to the RoomNode2D.OnCellAreaEntered and RoomNode2D.OnCellAreaExited signals to monitor these events.
    /// When the area detects an entering object, the `RoomLayoutState` cell visibility is automatically updated.
    /// </summary>
    public partial class CellArea2D : Area2D
    {
        /// <summary>
        /// The cell row in the room.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// The cell column in the room.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// The containing room.
        /// </summary>
        public RoomNode2D Room { get; private set; }

        /// <summary>
        /// Creates a new instance with child collision shape and adds it as a child of the specified room.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column.</param>
        /// <param name="room">The containing room.</param>
        /// <param name="collisionMask">The monitored collision mask. Typically, this should match the collision layer of the player character.</param>
        public static CellArea2D CreateInstance(int row, int column, RoomNode2D room, uint collisionMask)
        {
            var cell = new CellArea2D()
            {
                Row = row,
                Column = column,
                Room = room,
                CollisionLayer = 0,
                CollisionMask = collisionMask,
                Position = room.CellCenterLocalPosition(row, column),
            };

            var shape = new RectangleShape2D() { Size = room.CellSize };
            var collisionShape = new CollisionShape2D() { Shape = shape };
            cell.AddChild(collisionShape);
            room.AddChild(cell);
            return cell;
        }

        public override void _Ready()
        {
            base._Ready();
            BodyEntered += OnBodyEntered;
            AreaEntered += OnAreaEntered;
            BodyExited += OnBodyExited;
            AreaExited += OnAreaExited;
        }

        private void OnBodyEntered(Node body)
        {
            EmitOnCellEntered(body);
        }

        private void OnAreaEntered(Area2D area)
        {
            EmitOnCellEntered(area);
        }

        private void OnBodyExited(Node body)
        {
            EmitOnCellExited(body);
        }

        private void OnAreaExited(Area2D area)
        {
            EmitOnCellExited(area);
        }

        private void EmitOnCellEntered(Node collision)
        {
            Room?.RoomState?.SetCellVisibility(Row, Column, true);
            Room?.EmitOnCellAreaEntered(this, collision);
        }

        private void EmitOnCellExited(Node collision)
        {
            Room?.EmitOnCellAreaExited(this, collision);
        }
    }
}