using Godot;
using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class CameraController : Camera2D
    {
        [Export] public float ScrollSpeed { get; set; } = 1;
        [Export] public float ZoomSpeed { get; set; } = 2;

        public override void _UnhandledInput(InputEvent input)
        {
            base._UnhandledInput(input);
            var delta = (float)GetProcessDeltaTime();

            if (input is InputEventMouseMotion mouseMotion)
            {
                if (mouseMotion.ButtonMask == MouseButtonMask.Left)
                    Position -= delta * ScrollSpeed * mouseMotion.Velocity;
            }
            else if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.WheelUp)
                    Zoom += delta * ZoomSpeed * Vector2.One;
                else if (mouseInput.ButtonIndex == MouseButton.WheelDown)
                    Zoom -= delta * ZoomSpeed * Vector2.One;
            }
        }

        public void CenterCameraView(Layout layout, Vector2 cellSize)
        {
            var bounds = layout.GetBounds();
            var screenSize = GetViewport().GetVisibleRect().Size;

            var x = (bounds.X + 0.5f * bounds.Width) * cellSize.X;
            var y = (bounds.Y + 0.5f * bounds.Height) * cellSize.Y;

            var zoomX = screenSize.X / (cellSize.X * bounds.Width + 2 * cellSize.X);
            var zoomY = screenSize.Y / (cellSize.Y * bounds.Height + 2 * cellSize.Y);
            var zoom = Mathf.Min(zoomX, zoomY);

            Position = new Vector2(x, y);
            Zoom = new Vector2(zoom, zoom);
        }
    }
}