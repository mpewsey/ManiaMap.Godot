using Godot;
using MPewsey.ManiaMap;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Drawing
{
    [GlobalClass]
    public partial class LayoutTileMap : LayoutTileMapBase
    {
        public TileMap TileMap { get; private set; }
        public int LayerCoordinate { get; private set; }

        protected override void Initialize(Layout layout, LayoutState layoutState)
        {
            base.Initialize(layout, layoutState);
            TileMap ??= CreateTileMap();
        }

        public void DrawMap(int? z = null)
        {
            var manager = ManiaMapManager.Current;
            DrawMap(manager.Layout, manager.LayoutState, z);
        }

        public void DrawMap(Layout layout, LayoutState layoutState = null, int? z = null)
        {
            Initialize(layout, layoutState);
            LayerCoordinate = z ?? RoomsByLayer.Keys.Order().First();
            SetTiles(TileMap, LayerCoordinate);
        }
    }
}