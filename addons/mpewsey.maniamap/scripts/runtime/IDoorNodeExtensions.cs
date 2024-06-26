using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Extension methods for IDoorNode.
    /// </summary>
    public static class IDoorNodeExtensions
    {
        /// <summary>
        /// Returns true if a connection exists within the `Layout` for the door.
        /// </summary>
        public static bool DoorExists(this IDoorNode door)
        {
            return door.DoorConnection != null;
        }

        /// <summary>
        /// Returns the associated door connection within the `Layout`.
        /// If the door does not have a connect, returns null.
        /// </summary>
        public static DoorConnection FindDoorConnection(this IDoorNode door)
        {
            if (!door.RoomNode.IsInitialized)
                return null;

            var roomId = door.RoomNode.RoomLayout.Id;
            var position = new Vector2DInt(door.Row, door.Column);
            var direction = door.DoorDirection;
            return door.RoomNode.LayoutPack.FindDoorConnection(roomId, position, direction);
        }

        /// <summary>
        /// Returns the ID of the room to which this door connects.
        /// If this door does not exist, returns Uid(-1, -1, -1).
        /// </summary>
        public static Uid ToRoomId(this IDoorNode door)
        {
            if (!door.DoorExists())
                return new Uid(-1, -1, -1);

            return door.DoorConnection.GetConnectingRoom(door.RoomNode.RoomLayout.Id);
        }

        /// <summary>
        /// Returns the ManiaMap door used for procedural generation.
        /// </summary>
        public static Door GetMMDoor(this IDoorNode door)
        {
            return new Door(door.DoorType, (DoorCode)door.DoorCode);
        }
    }
}