using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// The base class for elements tied to a RoomNode2D's cell index.
    /// </summary>
    [Tool]
    [GlobalClass]
    public abstract partial class CellChild2D : Node2D, ICellChild
    {
        /// <summary>
        /// The contained room.
        /// </summary>
        [Export] public RoomNode2D Room { get; set; }

        /// <inheritdoc/>
        public IRoomNode RoomNode => Room;

        /// <inheritdoc/>
        [Export] public bool AutoAssignCell { get; set; } = true;

        /// <inheritdoc/>
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public int Row { get; set; }

        /// <inheritdoc/>
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