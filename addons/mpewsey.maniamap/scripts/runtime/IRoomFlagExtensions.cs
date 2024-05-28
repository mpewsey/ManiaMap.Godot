namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Extension methods for IRoomFlag.
    /// </summary>
    public static class IRoomFlagExtensions
    {
        /// <summary>
        /// Returns true if the flag is currently set in the current `LayoutState`.
        /// </summary>
        public static bool FlagIsSet(this IRoomFlag flag)
        {
            return flag.RoomNode.RoomState.Flags.Contains(flag.Id);
        }

        /// <summary>
        /// Sets the flag in the current `LayoutState`.
        /// Returns true if the flag was added and thus not previously set. Otherwise, returns false.
        /// </summary>
        public static bool SetFlag(this IRoomFlag flag)
        {
            return flag.RoomNode.RoomState.Flags.Add(flag.Id);
        }

        /// <summary>
        /// Remove the flag from the current `LayoutState`.
        /// Returns true if the flag was removed and thus was previously set. Otherwise, returns false.
        /// </summary>
        public static bool RemoveFlag(this IRoomFlag flag)
        {
            return flag.RoomNode.RoomState.Flags.Remove(flag.Id);
        }

        /// <summary>
        /// Toggles the flag in the current `LayoutState`.
        /// Returns true if the flag is now set. Otherwise, returns false.
        /// </summary>
        public static bool ToggleFlag(this IRoomFlag flag)
        {
            if (flag.RoomNode.RoomState.Flags.Add(flag.Id))
                return true;

            flag.RoomNode.RoomState.Flags.Remove(flag.Id);
            return false;
        }
    }
}