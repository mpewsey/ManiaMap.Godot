using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    public interface IRoomNode
    {
        public RoomTemplate CreateRoomTemplate(int id, string name);
    }
}