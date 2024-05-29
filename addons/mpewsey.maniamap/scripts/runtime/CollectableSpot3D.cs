using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible collectable location within a RoomNode3D.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.CollectableSpot3Dicon)]
    public partial class CollectableSpot3D : CellChild3D, ICollectableSpot
    {
        /// <inheritdoc/>
        [Export] public int Id { get; set; } = -1;

        /// <inheritdoc/>
        [Export] public CollectableGroup CollectableGroup { get; set; }

        /// <inheritdoc/>
        [Export(PropertyHint.Range, "0,2,0.1,or_greater")] public float Weight { get; set; } = 1;

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode3D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }
    }
}