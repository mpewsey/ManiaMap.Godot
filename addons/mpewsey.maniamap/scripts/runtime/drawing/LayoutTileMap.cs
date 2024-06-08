using Godot;
using MPewsey.ManiaMap;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Drawing
{
    /// <summary>
    /// A class for drawing a single layer of a `Layout` onto a tile map.
    /// </summary>
    [GlobalClass]
    public partial class LayoutTileMap : LayoutTileMapBase
    {
        /// <summary>
        /// The target tile map.
        /// </summary>
        public TileMap TileMap { get; private set; }

        /// <summary>
        /// The currently drawn layer (z) coordinate.
        /// </summary>
        public int LayerCoordinate { get; private set; }

        /// <inheritdoc/>
        protected override void Initialize(Layout layout, LayoutState layoutState)
        {
            base.Initialize(layout, layoutState);
            TileMap ??= CreateTileMap();
        }

        /// <summary>
        /// Draws the map for the specified layer (z) coordinate.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="z">The layer coordinate. If null, the minimum layer coordinate in the layout will be used.</param>
        public void DrawMap(LayoutPack layoutPack, int? z = null)
        {
            DrawMap(layoutPack.Layout, layoutPack.LayoutState, z);
        }

        /// <summary>
        /// Draws the map for the specified layer (z) coordinate.
        /// </summary>
        /// <param name="layout">The drawn layout.</param>
        /// <param name="layoutState">The layout state. If null, the full map will be drawn.</param>
        /// <param name="z">The layer coordinate. If null, the minimum layer coordinate in the layout will be used.</param>
        public void DrawMap(Layout layout, LayoutState layoutState = null, int? z = null)
        {
            Initialize(layout, layoutState);
            LayerCoordinate = z ?? RoomsByLayer.Keys.Order().First();
            SetTiles(TileMap, LayerCoordinate);
        }
    }
}