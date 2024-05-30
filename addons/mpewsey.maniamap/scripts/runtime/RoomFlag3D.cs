using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A room flag that can be set or toggled to alter the `LayoutState`.
    /// 
    /// See IRoomFlagExtensions for additional methods usable by this class.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomFlag3DIcon)]
    public partial class RoomFlag3D : CellChild3D, IRoomFlag
    {
        /// <inheritdoc/>
        [Export] public int Id { get; set; } = -1;

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode3D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }
    }
}