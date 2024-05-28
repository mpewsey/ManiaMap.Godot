namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// The interface for elements tied to an IRoomNode's cell index.
    /// </summary>
    public interface ICellChild
    {
        /// <summary>
        /// The containing room node.
        /// </summary>
        public IRoomNode RoomNode { get; }

        /// <summary>
        /// If true, the cell row and column indices will be automatically assigned when auto assign is run.
        /// Disable this flag if you wish to control these values manually.
        /// </summary>
        public bool AutoAssignCell { get; }

        /// <summary>
        /// The cell row index.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// The cell column index.
        /// </summary>
        public int Column { get; }
    }
}