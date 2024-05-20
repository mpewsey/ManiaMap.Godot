using Godot;
using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible collectable location within a RoomNode2D.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.CollectableSpot2Dicon)]
    public partial class CollectableSpot2D : CellChild2D
    {
        /// <summary>
        /// The spot's unique ID. The ID must be unique within the scope of a room.
        /// </summary>
        [Export] public int Id { get; set; } = -1;

        /// <summary>
        /// The assigned collectable group from which collectables will be procedurally pulled.
        /// </summary>
        [Export] public CollectableGroup CollectableGroup { get; set; }

        /// <summary>
        /// The manual draw weight assigned to the spot. A larger value increases the chance of the spot being used.
        /// </summary>
        [Export(PropertyHint.Range, "0,2,0.1,or_greater")] public float Weight { get; set; } = 1;

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }

        /// <summary>
        /// Returns the ManiaMap collectable object used by the procedural generator.
        /// </summary>
        public CollectableSpot GetMMCollectableSpot()
        {
            var index = new Vector2DInt(Row, Column);
            return new CollectableSpot(index, CollectableGroup.GroupName, Weight);
        }

        /// <summary>
        /// Returns the collectable ID if the collectable spot ID exists within the `Layout`.
        /// If the collectable spot does not exist, returns -1.
        /// </summary>
        public int CollectableId()
        {
            if (Room.RoomLayout.Collectables.TryGetValue(Id, out var collectableId))
                return collectableId;
            return -1;
        }

        /// <summary>
        /// Returns true if the collectable spot ID exists within the `Layout`.
        /// </summary>
        public bool CollectableExists()
        {
            return Room.RoomLayout.Collectables.ContainsKey(Id);
        }

        /// <summary>
        /// Returns true if the collectable spot has already been acquired,
        /// i.e. if the collectable spot ID exists within the `LayoutState`.
        /// </summary>
        public bool IsAcquired()
        {
            return Room.RoomState.AcquiredCollectables.Contains(Id);
        }

        /// <summary>
        /// Returns true if the collectable can be acquired,
        /// i.e. that the collectable exists and has not been acquired.
        /// </summary>
        public bool CanAcquire()
        {
            return CollectableExists() && !IsAcquired();
        }

        /// <summary>
        /// If the collectable can be acquired, adds it to the acquired collectables and returns true.
        /// Otherwise, returns false.
        /// </summary>
        public bool Acquire()
        {
            if (CanAcquire())
                return Room.RoomState.AcquiredCollectables.Add(Id);
            return false;
        }
    }
}