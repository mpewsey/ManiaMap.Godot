using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    public interface IRoomNode
    {
        public RoomTemplate CreateTemplate(int id, string name);
    }
}