using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Extension methods for ICollectableSpot.
    /// </summary>
    public static class ICollectableSpotExtensions
    {
        /// <summary>
        /// Returns the collectable ID if the collectable spot ID exists within the `Layout`.
        /// If the collectable spot does not exist, returns -1.
        /// </summary>
        public static int CollectableId(this ICollectableSpot spot)
        {
            if (spot.RoomNode.RoomLayout.Collectables.TryGetValue(spot.Id, out var collectableId))
                return collectableId;
            return -1;
        }

        /// <summary>
        /// Returns true if the collectable spot ID exists within the `Layout`.
        /// </summary>
        public static bool CollectableExists(this ICollectableSpot spot)
        {
            return spot.RoomNode.RoomLayout.Collectables.ContainsKey(spot.Id);
        }

        /// <summary>
        /// Returns true if the collectable spot has already been acquired,
        /// i.e. if the collectable spot ID exists within the `LayoutState`.
        /// </summary>
        public static bool IsAcquired(this ICollectableSpot spot)
        {
            return spot.RoomNode.RoomState.AcquiredCollectables.Contains(spot.Id);
        }

        /// <summary>
        /// Returns true if the collectable can be acquired,
        /// i.e. that the collectable exists and has not been acquired.
        /// </summary>
        public static bool CanAcquire(this ICollectableSpot spot)
        {
            return spot.CollectableExists() && !spot.IsAcquired();
        }

        /// <summary>
        /// If the collectable can be acquired, adds it to the acquired collectables and returns true.
        /// Otherwise, returns false.
        /// </summary>
        public static bool Acquire(this ICollectableSpot spot)
        {
            if (spot.CanAcquire())
                return spot.RoomNode.RoomState.AcquiredCollectables.Add(spot.Id);
            return false;
        }

        /// <summary>
        /// Returns the ManiaMap collectable object used by the procedural generator.
        /// </summary>
        public static CollectableSpot GetMMCollectableSpot(this ICollectableSpot spot)
        {
            var index = new Vector2DInt(spot.Row, spot.Column);
            return new CollectableSpot(index, spot.CollectableGroup.GroupName, spot.Weight);
        }
    }
}