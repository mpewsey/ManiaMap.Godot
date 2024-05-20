using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// The base class for elements tied to a RoomNode2D's cell index.
    /// </summary>
    [Tool]
    [GlobalClass]
    public abstract partial class CellChild2D : Node2D
    {
        /// <summary>
        /// The contained room.
        /// </summary>
        [Export] public RoomNode2D Room { get; set; }

        /// <summary>
        /// If true, the cell row and column indices will be assigned when RoomNode2D.AutoAssign is run.
        /// Disable this flag if you wish to control these values manually.
        /// </summary>
        [Export] public bool AutoAssignCell { get; set; } = true;

        /// <summary>
        /// The cell row index.
        /// </summary>
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public int Row { get; set; }

        /// <summary>
        /// The cell column index.
        /// </summary>
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public int Column { get; set; }

        /// <summary>
        /// Assigns the room and any auto assigned values to the object.
        /// </summary>
        /// <param name="room">The containing room.</param>
        public virtual void AutoAssign(RoomNode2D room)
        {
            Room = room;

            if (AutoAssignCell)
                (Row, Column) = room.FindClosestActiveCellIndex(GlobalPosition);
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.Room)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }
    }
}