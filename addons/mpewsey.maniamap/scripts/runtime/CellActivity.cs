namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Option for setting cell activities.
    /// </summary>
    public enum CellActivity
    {
        /// Performs no action.
        None,
        /// Activates a cell.
        Activate,
        /// Deactivates a cell.
        Deactivate,
        /// Toggles a cell's activity. Active becomes deactivated and vice versa.
        Toggle,
    }
}