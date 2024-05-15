using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Generators;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class RoomLayoutSample : Node
    {
        [Export] public Camera2D Camera { get; set; }
        [Export] public Node2D Container { get; set; }
        [Export] public GenerationPipeline Pipeline { get; set; }
        [Export] public Button GenerateButton { get; set; }
        [Export] public RichTextLabel MessageLabel { get; set; }
        [Export] public TemplateGroupDatabase TemplateGroupDatabase { get; set; }
        [Export] public Vector2 CellSize { get; set; } = new Vector2(96, 96);
        [Export] public float ScrollSpeed { get; set; } = 1;
        [Export] public float ZoomSpeed { get; set; } = 1;

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
                    Camera.Position -= delta * ScrollSpeed * mouseMotion.Velocity;
            }
            else if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.WheelUp)
                    Camera.Zoom += delta * ZoomSpeed * Vector2.One;
                else if (mouseInput.ButtonIndex == MouseButton.WheelDown)
                    Camera.Zoom -= delta * ZoomSpeed * Vector2.One;
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
                MessageLabel.Text = $"[color=#ff0000]Generation FAILED (Seed = {seed})[/color]";
                return;
            }

            MessageLabel.Text = string.Empty;
            var layout = results.GetOutput<Layout>("Layout");
            DrawLayout(layout);
        }

        private void DrawLayout(Layout layout)
        {
            var settings = new ManiaMapSettings() { AssignLayoutPosition = true };
            ManiaMapManager.Initialize(layout, new LayoutState(layout), settings);
            ClearContainer();
            TemplateGroupDatabase.CreateRoom2DInstances(Container);
            SetCameraView(layout);
        }

        private void SetCameraView(Layout layout)
        {
            var bounds = layout.GetBounds();
            var screenSize = GetViewport().GetVisibleRect().Size;

            var x = (bounds.X + 0.5f * bounds.Width) * CellSize.X;
            var y = (bounds.Y + 0.5f * bounds.Height) * CellSize.Y;

            var zoomX = screenSize.X / (CellSize.X * bounds.Width + 2 * CellSize.X);
            var zoomY = screenSize.Y / (CellSize.Y * bounds.Height + 2 * CellSize.Y);

            Camera.Position = new Vector2(x, y);
            Camera.Zoom = Mathf.Min(zoomX, zoomY) * Vector2.One;
        }
    }
}