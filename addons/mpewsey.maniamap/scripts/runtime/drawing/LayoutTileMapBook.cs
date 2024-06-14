using Godot;
using MPewsey.ManiaMap;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Drawing
{
    /// <summary>
    /// A class for drawing all layers of a `Layout` onto tile maps.
    /// </summary>
    [GlobalClass]
    public partial class LayoutTileMapBook : LayoutTileMapBase
    {
        /// <summary>
        /// A list of tile maps corresponding to the pages of the book.
        /// </summary>
        private List<TileMap> Pages { get; } = new List<TileMap>();

        /// <summary>
        /// A list of layer (z) coordinates corresponding to the pages at the same index.
        /// </summary>
        private List<int> PageLayerCoordinates { get; set; } = new List<int>();

        /// <summary>
        /// Returns the read-only list of pages.
        /// </summary>
        public IReadOnlyList<TileMap> GetPages() => Pages;

        /// <summary>
        /// Returns the read-only list of page layer coordinates.
        /// </summary>
        public IReadOnlyList<int> GetPageLayerCoordinates() => PageLayerCoordinates;

        /// <summary>
        /// Draws all layout layers onto tile maps.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        public void DrawPages(LayoutPack layoutPack)
        {
            LayoutPack = layoutPack;
            PageLayerCoordinates = layoutPack.GetLayerCoordinates().ToList();
            PageLayerCoordinates.Sort();
            SizePages();

            for (int i = 0; i < Pages.Count; i++)
            {
                SetTiles(Pages[i], PageLayerCoordinates[i]);
            }
        }

        /// <summary>
        /// Draws all layout layers onto tile maps.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="layoutState">The layout state. If null, the full map will be drawn. Otherwise, the layout state cell visibilities will be applied.</param>
        public void DrawPages(Layout layout, LayoutState layoutState = null)
        {
            layoutState ??= CreateFullyVisibleLayoutState(layout);
            var layoutPack = new LayoutPack(layout, layoutState, new ManiaMapSettings());
            DrawPages(layoutPack);
        }

        /// <summary>
        /// Adds or frees tile maps from the pages list until it matches the number required by the layout.
        /// </summary>
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

        /// <summary>
        /// Sets the modulate color of the tile map pages based on a color gradient and view layer (z) coordinate.
        /// Based on the gradient provided, this allows the viewing of multiple layers in a semitranparent manner,
        /// similar to using onionskin paper.
        /// </summary>
        /// <param name="z">The viewer's layer position.</param>
        /// <param name="gradient">The color gradient.</param>
        /// <param name="drawDepth">The maximum draw depth forward and behind a given layer coordinate.</param>
        public float SetOnionskinColors(float z, Gradient gradient, float drawDepth = 1)
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