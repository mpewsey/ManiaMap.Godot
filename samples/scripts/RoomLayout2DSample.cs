using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Generators;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class RoomLayout2DSample : Node
    {
        [Export] public Camera2DController Camera { get; set; }
        [Export] public Node2D Container { get; set; }
        [Export] public GenerationPipeline Pipeline { get; set; }
        [Export] public Button GenerateButton { get; set; }
        [Export] public RichTextLabel MessageLabel { get; set; }
        [Export] public RoomTemplateDatabase RoomTemplateDatabase { get; set; }
        [Export] public Vector2 CellSize { get; set; } = new Vector2(96, 96);

        public override void _Ready()
        {
            base._Ready();
            GenerateButton.GrabFocus();
            GenerateButton.Pressed += OnGenerateButtonPressed;
        }

        private void ClearContainer()
        {
            var count = Container.GetChildCount();

            for (int i = 0; i < count; i++)
            {
                Container.GetChild(i).QueueFree();
            }
        }

        private void OnGenerateButtonPressed()
        {
            GenerateLayoutAsync();
        }

        private async void GenerateLayoutAsync()
        {
            MessageLabel.Text = "Generating...";
            GenerateButton.Disabled = true;
            var seed = Rand.Random.Next(1, int.MaxValue);
            var results = await Pipeline.RunAttemptsAsync(seed);
            GenerateButton.Disabled = false;

            if (!results.Success)
            {
                MessageLabel.Text = $"[color=#ff0000]Generation FAILED (Seed = {seed})[/color]";
                return;
            }

            MessageLabel.Text = string.Empty;
            var layout = results.GetOutput<Layout>("Layout");
            ManiaMapManager.Initialize(layout, new LayoutState(layout), new ManiaMapSettings());
            ClearContainer();
            RoomTemplateDatabase.CreateRoom2DInstances(Container);
            Camera.CenterCameraView(layout, CellSize);
        }
    }
}