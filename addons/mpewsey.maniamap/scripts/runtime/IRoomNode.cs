using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// The interface for a room node.
    /// </summary>
    public interface IRoomNode
    {
        /// <summary>
        /// Returns the ManiaMap room template used for procedural generation.
        /// </summary>
        /// <param name="id">The unique template ID.</param>
        /// <param name="name">The template name.</param>
        public RoomTemplate GetMMRoomTemplate(int id, string name);
    }
}