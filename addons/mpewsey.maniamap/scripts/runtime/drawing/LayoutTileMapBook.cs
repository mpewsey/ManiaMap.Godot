using Godot;
using MPewsey.ManiaMap;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Drawing
{
    [GlobalClass]
    public partial class LayoutTileMapBook : LayoutTileMapBase
    {
        private List<TileMap> Pages { get; } = new List<TileMap>();
        private List<int> PageLayerCoordinates { get; set; } = new List<int>();

        public IReadOnlyList<TileMap> GetPages() => Pages;
        public IReadOnlyList<int> GetPageLayerCoordinates() => PageLayerCoordinates;

        protected override void Initialize(Layout layout, LayoutState layoutState)
        {
            base.Initialize(layout, layoutState);
            PageLayerCoordinates = RoomsByLayer.Keys.ToList();
            PageLayerCoordinates.Sort();
            SizePages();
        }

        public void DrawPages()
        {
            var manager = ManiaMapManager.Current;
            DrawPages(manager.Layout, manager.LayoutState);
        }

        public void DrawPages(Layout layout, LayoutState layoutState = null)
        {
            Initialize(layout, layoutState);

            for (int i = 0; i < Pages.Count; i++)
            {
                SetTiles(Pages[i], PageLayerCoordinates[i]);
            }
        }

        private void SizePages()
        {
            while (Pages.Count > PageLayerCoordinates.Count)
            {
                var index = Pages.Count - 1;
                Pages[index].QueueFree();
                Pages.RemoveAt(index);
            }

            while (Pages.Count < PageLayerCoordinates.Count)
            {
                Pages.Add(CreateTileMap());
            }
        }
    }
}