using Godot;

namespace MPewsey.ManiaMapGodot
{
    public partial class CellArea2D : Area2D
    {
        private int Row { get; set; }
        private int Column { get; set; }
        private RoomNode2D Room { get; set; }

        public static CellArea2D CreateInstance(int row, int column, RoomNode2D room, uint collisionMask)
        {
            var size = room.CellSize;
            var x = column * size.X + 0.5f * size.X;
            var y = row * size.Y + 0.5f * size.Y;

            var cell = new CellArea2D()
            {
                Row = row,
                Column = column,
                Room = room,
                CollisionLayer = 0,
                CollisionMask = collisionMask,
                Position = new Vector2(x, y),
            };

            var shape = new RectangleShape2D() { Size = size };
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
        }

        private void OnBodyEntered(Node body)
        {
            SetCellVisibility();
        }

        private void OnAreaEntered(Area2D area)
        {
            SetCellVisibility();
        }

        private void SetCellVisibility()
        {
            Room.RoomState.SetCellVisibility(Row, Column, true);
        }
    }
}