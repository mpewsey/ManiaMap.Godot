namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible collectable location within an IRoomNode.
    /// </summary>
    public interface ICollectableSpot : ICellChild
    {
        /// <summary>
        /// The spot's unique ID. The ID must be unique within the scope of a room.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The assigned collectable group from which collectables will be procedurally pulled.
        /// </summary>
        public CollectableGroup CollectableGroup { get; }

        /// <summary>
        /// The manual draw weight assigned to the spot. A larger value increases the chance of the spot being used.
        /// </summary>
        public float Weight { get; }
    }
}