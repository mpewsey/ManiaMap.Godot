using Godot;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class Camera3DController : Camera3D
    {
        [Export] public float PanSpeed { get; set; } = 0.1f;
        [Export] public float RotationSpeed { get; set; } = 0.01f;
        [Export] public float RotationPointOffset { get; set; } = 5;
        [Export] public float ZoomSpeed { get; set; } = 50;

        private Vector3 InitialPosition { get; set; }
        private Vector3 InitialRotation { get; set; }

        public override void _Ready()
        {
            base._Ready();
            InitialPosition = Position;
            InitialRotation = Rotation;
        }

        public void ResetPosition()
        {
            Position = InitialPosition;
            Rotation = InitialRotation;
        }

        public override void _UnhandledInput(InputEvent input)
        {
            base._UnhandledInput(input);
            var delta = (float)GetProcessDeltaTime();

            if (input is InputEventMouseMotion mouseMotion)
            {
                if (mouseMotion.ButtonMask == MouseButtonMask.Left)
                {
                    var basis = GlobalTransform.Basis;
                    var x = -delta * PanSpeed * mouseMotion.Velocity.X * basis.X.Normalized();
                    var y = delta * PanSpeed * mouseMotion.Velocity.Y * basis.Y.Normalized();
                    Position += x + y;
                }
                else if (mouseMotion.ButtonMask == MouseButtonMask.Right)
                {
                    var position = Position;
                    var basis = GlobalTransform.Basis;
                    var x = delta * RotationSpeed * mouseMotion.Velocity.X * basis.X.Normalized();
                    var y = -delta * RotationSpeed * mouseMotion.Velocity.Y * basis.Y.Normalized();
                    var rotationPoint = Position + RotationPointOffset * basis.Z.Normalized();
                    LookAtFromPosition(rotationPoint, position + x + y);
                    Position = position;
                }
            }
            else if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.WheelUp)
                    Position -= delta * ZoomSpeed * GlobalTransform.Basis.Z.Normalized();
                else if (mouseInput.ButtonIndex == MouseButton.WheelDown)
                    Position += delta * ZoomSpeed * GlobalTransform.Basis.Z.Normalized();
                else if (mouseInput.ButtonIndex == MouseButton.Middle)
                    ResetPosition();
            }
        }
    }
}