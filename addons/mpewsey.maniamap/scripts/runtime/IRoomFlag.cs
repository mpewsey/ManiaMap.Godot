namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A room flag that can be set or toggled to alter the `LayoutState`.
    /// </summary>
    public interface IRoomFlag : ICellChild
    {
        /// <summary>
        /// The unique flag ID. The ID must be unique within a room.
        /// </summary>
        public int Id { get; }
    }
}
