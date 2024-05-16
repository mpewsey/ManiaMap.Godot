using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapGodot.Drawing;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class LayoutTileMapSample : Node
    {
        [Export] public CameraController Camera { get; set; }
        [Export] public LayoutTileMap Map { get; set; }

        public override void _Ready()
        {
            base._Ready();
            PopulateMapAsync();
        }

        private async void PopulateMapAsync()
        {
            var result = await BigLayoutSample.GenerateAsync(12345);

            if (result.Success)
            {
                var layout = result.GetOutput<Layout>("Layout");
                Map.DrawMap(layout);
            }
        }
    }
}