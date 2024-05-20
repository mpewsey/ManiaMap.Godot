using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A room flag that can be set or toggled to alter the `LayoutState`.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomFlag2DIcon)]
    public partial class RoomFlag2D : CellChild2D
    {
        /// <summary>
        /// The unique flag ID. The ID must be unique within a room.
        /// </summary>
        [Export] public int Id { get; set; } = -1;

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }

        /// <summary>
        /// Returns true if the flag is currently set in the current `LayoutState`.
        /// </summary>
        public bool FlagIsSet()
        {
            return Room.RoomState.Flags.Contains(Id);
        }

        /// <summary>
        /// Sets the flag in the current `LayoutState`.
        /// Returns true if the flag was added and thus not previously set. Otherwise, returns false.
        /// </summary>
        public bool SetFlag()
        {
            return Room.RoomState.Flags.Add(Id);
        }

        /// <summary>
        /// Remove the flag from the current `LayoutState`.
        /// Returns true if the flag was removed and thus was previously set. Otherwise, returns false.
        /// </summary>
        public bool RemoveFlag()
        {
            return Room.RoomState.Flags.Remove(Id);
        }

        /// <summary>
        /// Toggles the flag in the current `LayoutState`.
        /// Returns true if the flag is now set. Otherwise, returns false.
        /// </summary>
        public bool ToggleFlag()
        {
            if (Room.RoomState.Flags.Add(Id))
                return true;

            Room.RoomState.Flags.Remove(Id);
            return false;
        }
    }
}