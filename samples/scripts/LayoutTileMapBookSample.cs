using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Generators;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapGodot.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class LayoutTileMapBookSample : Node
    {
        [Export] public CameraController Camera { get; set; }
        [Export] public LayoutTileMapBook Map { get; set; }
        [Export] public Gradient Gradient { get; set; }
        [Export] public Slider Slider { get; set; }
        [Export] public Button GenerateButton { get; set; }
        [Export] public RichTextLabel MessageLabel { get; set; }
        [Export] public Label ZLabel { get; set; }
        [Export] public Control SliderContainer { get; set; }
        [Export] public Vector2 CellSize { get; set; } = new Vector2(16, 16);

        public override void _Ready()
        {
            base._Ready();
            GenerateButton.GrabFocus();
            GenerateButton.Pressed += OnGenerateButtonPressed;
            Slider.ValueChanged += OnSliderValueChanged;
            SliderContainer.Visible = false;
        }

        private void OnSliderValueChanged(double value)
        {
            Map.SetOnionMapColors((float)value, Gradient);
            ZLabel.Text = value.ToString("0.00");
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
            var result = await GenerateLayout(seed);
            GenerateButton.Disabled = false;

            if (!result.Success)
            {
                MessageLabel.Text = $"[color=#ff0000]Generation FAILED (Seed = {seed})[/color]";
                return;
            }

            MessageLabel.Text = string.Empty;
            var layout = result.GetOutput<Layout>("Layout");
            Map.DrawPages(layout);
            Camera.CenterCameraView(layout, CellSize);

            var zs = Map.GetPageLayerCoordinates();
            SliderContainer.Visible = true;
            Slider.MinValue = zs[0];
            Slider.MaxValue = zs[zs.Count - 1];
            Slider.Value = Mathf.FloorToInt((Slider.MinValue + Slider.MaxValue) * 0.5f);
            OnSliderValueChanged(Slider.Value);
        }

        private static Task<PipelineResults> GenerateLayout(int seed)
        {
            var templateGroups = new TemplateGroups();
            templateGroups.Add("Default", TemplateLibrary.Miscellaneous.HyperSquareTemplate());

            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(seed) },
                { "LayoutGraph", GraphLibrary.StackedLoopGraph() },
                { "TemplateGroups", templateGroups },
            };

            var pipeline = new Pipeline(new LayoutGenerator());
            return pipeline.RunAsync(inputs);
        }
    }
}