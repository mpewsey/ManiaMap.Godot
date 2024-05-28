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
            var roomId = door.RoomNode.RoomLayout.Id;
            var position = new Vector2DInt(door.Row, door.Column);
            var direction = door.DoorDirection;

            foreach (var connection in door.RoomNode.DoorConnections)
            {
                if (connection.ContainsDoor(roomId, position, direction))
                    return connection;
            }

            return null;
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
    }
}