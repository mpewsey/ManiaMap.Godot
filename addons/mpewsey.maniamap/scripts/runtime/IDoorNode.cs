using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible door location connecting two IRoomNode.
    /// </summary>
    public interface IDoorNode : ICellChild
    {
        /// <summary>
        /// If true, auto assigns the door direction based on its position when auto assign is run.
        /// Disable this flag if you wish to control this value manually.
        /// </summary>
        public bool AutoAssignDirection { get; }

        /// <summary>
        /// The direction where the door leads relative to its containing cell.
        /// </summary>
        public DoorDirection DoorDirection { get; }

        /// <summary>
        /// The door type. See the ManiaMap DoorType documentation for more information.
        /// </summary>
        public DoorType DoorType { get; }

        /// <summary>
        /// The door code to which this door may connect. Door codes can connect if they intersect.
        /// </summary>
        public int DoorCode { get; }

        /// <summary>
        /// The door's door and room connection information.
        /// </summary>
        public DoorConnection DoorConnection { get; }
    }
}