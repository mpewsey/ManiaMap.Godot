using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public abstract partial class CellChild2D : Node2D
    {
        [Export] public bool AutoAssignCell { get; set; } = true;
        [Export] public int Row { get; set; }
        [Export] public int Column { get; set; }
        [Export] public RoomNode2D Room { get; set; }

        public virtual void AutoAssign(RoomNode2D room)
        {
            Room = room;

            if (AutoAssignCell)
                (Row, Column) = room.FindClosestCellIndex(GlobalPosition);
        }
    }
}