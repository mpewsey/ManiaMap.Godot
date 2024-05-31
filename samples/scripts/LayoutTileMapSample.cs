using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapGodot.Drawing;
using System.Threading;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class LayoutTileMapSample : Node
    {
        [Export] public Camera2DController Camera { get; set; }
        [Export] public LayoutTileMap Map { get; set; }
        [Export] public Button GenerateButton { get; set; }
        [Export] public RichTextLabel MessageLabel { get; set; }
        [Export] public Vector2 CellSize { get; set; } = new Vector2(16, 16);

        public override void _Ready()
        {
            base._Ready();
            GenerateButton.GrabFocus();
            GenerateButton.Pressed += OnGenerateButtonPressed;
        }

        private void OnGenerateButtonPressed()
        {
            GenerateMapAsync();
        }

        private async void GenerateMapAsync()
        {
            MessageLabel.Text = "Generating...";
            GenerateButton.Disabled = true;
            var seed = Rand.Random.Next(1, int.MaxValue);
            var token = new CancellationTokenSource(5000).Token;
            var result = await BigLayoutSample.GenerateAsync(seed, cancellationToken: token);
            GenerateButton.Disabled = false;

            if (!result.Success)
            {
                MessageLabel.Text = $"[color=#ff0000]Generation FAILED (Seed = {seed})[/color]";
                return;
            }

            MessageLabel.Text = string.Empty;
            var layout = result.GetOutput<Layout>("Layout");
            Map.DrawMap(layout);
            Camera.CenterCameraView(layout, CellSize);
        }
    }
}