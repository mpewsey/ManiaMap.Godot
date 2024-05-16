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

        public float SetOnionMapColors(float z, Gradient gradient, float drawDepth = 1)
        {
            if (PageLayerCoordinates.Count > 0)
            {
                var scale = 0.5f / drawDepth;
                var minZ = PageLayerCoordinates[0];
                var maxZ = PageLayerCoordinates[PageLayerCoordinates.Count - 1];
                z = Mathf.Clamp(z, minZ, maxZ);

                for (int i = 0; i < PageLayerCoordinates.Count; i++)
                {
                    var t = (PageLayerCoordinates[i] - z) * scale + 0.5f;
                    Pages[i].Modulate = gradient.Sample(Mathf.Clamp(t, 0, 1));
                }
            }

            return z;
        }
    }
}