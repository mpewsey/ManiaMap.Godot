using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible collectable location within a RoomNode2D.
    /// 
    /// See ICollectableSpotExtensions for additional methods usable by this class.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.CollectableSpot2Dicon)]
    public partial class CollectableSpot2D : CellChild2D, ICollectableSpot
    {
        /// <inheritdoc/>
        [Export] public int Id { get; set; } = -1;

        /// <inheritdoc/>
        [Export] public CollectableGroup CollectableGroup { get; set; }

        /// <inheritdoc/>
        [Export(PropertyHint.Range, "0,2,0.1,or_greater")] public float Weight { get; set; } = 1;

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }
    }
}