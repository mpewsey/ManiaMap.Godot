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
        }

        private void OnBodyEntered(Node body)
        {
            EmitOnCellEntered(body);
        }

        private void OnAreaEntered(Area2D area)
        {
            EmitOnCellEntered(area);
        }

        private void EmitOnCellEntered(Node collision)
        {
            Room.RoomState.SetCellVisibility(Row, Column, true);
            Room.EmitOnCellAreaEntered(this, collision);
        }
    }
}