using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Generators;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class RoomLayoutSample : Node
    {
        [Export] public Node2D Container { get; set; }
        [Export] public GenerationPipeline Pipeline { get; set; }
        [Export] public Button GenerateButton { get; set; }
        [Export] public RichTextLabel MessageLabel { get; set; }
        [Export] public TemplateGroupDatabase TemplateGroupDatabase { get; set; }
        [Export] public float ZoomSpeed { get; set; } = 1;
        [Export] public float ScrollSpeed { get; set; } = 1;

        public override void _Ready()
        {
            base._Ready();
            GenerateButton.GrabFocus();
            GenerateButton.Pressed += OnGenerateButtonPressed;
        }

        public override void _UnhandledInput(InputEvent input)
        {
            base._UnhandledInput(input);
            var delta = (float)GetProcessDeltaTime();

            if (input is InputEventMouseMotion mouseMotion)
            {
                if (mouseMotion.ButtonMask == MouseButtonMask.Left)
                    Container.Position += delta * ScrollSpeed * mouseMotion.Velocity;
            }
            else if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.WheelUp)
                    Container.Scale += delta * ZoomSpeed * Vector2.One;
                else if (mouseInput.ButtonIndex == MouseButton.WheelDown)
                    Container.Scale -= delta * ZoomSpeed * Vector2.One;
            }
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Input.IsMouseButtonPressed(MouseButton.WheelUp))
            {

            }
            else if (Input.IsMouseButtonPressed(MouseButton.WheelUp))
            {

            }
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
                MessageLabel.Text = "[color=#ff0000]Generation FAILED.[/color]";
                return;
            }

            MessageLabel.Text = string.Empty;
            var layout = results.GetOutput<Layout>("Layout");
            var settings = new ManiaMapSettings() { AssignLayoutPosition = true };
            ManiaMapManager.Initialize(layout, new LayoutState(layout), settings);
            ClearContainer();
            TemplateGroupDatabase.CreateRoom2DInstances(Container);
            Container.Position = 0.5f * GetViewport().GetVisibleRect().Size;
            Container.Scale = new Vector2(0.5f, 0.5f);
        }
    }
}