using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A room flag that can be set or toggled to alter the `LayoutState`.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomFlag2DIcon)]
    public partial class RoomFlag2D : CellChild2D, IRoomFlag
    {
        /// <inheritdoc/>
        [Export] public int Id { get; set; } = -1;

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }
    }
}