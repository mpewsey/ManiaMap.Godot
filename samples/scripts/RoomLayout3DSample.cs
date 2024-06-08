using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Generators;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class RoomLayout3DSample : Node
    {
        [Export] public Camera3DController Camera { get; set; }
        [Export] public Node3D Container { get; set; }
        [Export] public GenerationPipeline Pipeline { get; set; }
        [Export] public Button GenerateButton { get; set; }
        [Export] public RichTextLabel MessageLabel { get; set; }
        [Export] public RoomTemplateDatabase RoomTemplateDatabase { get; set; }
        [Export] public Vector3 CellSize { get; set; } = new Vector3(6, 6, 6);

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
            var layoutPack = new LayoutPack(layout, new LayoutState(layout), new ManiaMapSettings());
            ClearContainer();
            RoomTemplateDatabase.CreateRoom3DInstances(Container, layoutPack);
            Camera.ResetPosition();
        }
    }
}