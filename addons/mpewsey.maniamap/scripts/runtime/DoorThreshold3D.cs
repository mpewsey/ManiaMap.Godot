using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A rectangular region for parameterizing and interpolating positions when
    /// passing through doors in different scenes.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class DoorThreshold3D : Node3D
    {
        [Signal] public delegate void OnSizeChangedEventHandler(Vector3 size);
        public Error EmitOnSizeChanged(Vector3 size) => EmitSignal(SignalName.OnSizeChanged, size);

        private Vector3 _size = new Vector3(1, 1, 1);
        /// <summary>
        /// The width, height, and depth of the threshold.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,0.001,or_greater")] public Vector3 Size { get => _size; set => SetSize(ref _size, value); }

        private void SetSize(ref Vector3 field, Vector3 value)
        {
            field = new Vector3(Mathf.Max(value.X, 0), Mathf.Max(value.Y, 0), Mathf.Max(value.Z, 0));
            EmitOnSizeChanged(field);
        }

#if TOOLS
        public override void _Ready()
        {
            base._Ready();

            if (Engine.IsEditorHint())
                Editor.BoxGizmo.CreateInstance(this);
        }
#endif

        /// <summary>
        /// Converts a global position to a parameterized position on the interval [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector3 ParameterizePosition(Vector3 position)
        {
            var topLeft = GlobalPosition - 0.5f * Size;
            var delta = position - topLeft;
            var x = Size.X > 0 ? Mathf.Clamp(delta.X / Size.X, 0, 1) : 0.5f;
            var y = Size.Y > 0 ? Mathf.Clamp(delta.Y / Size.Y, 0, 1) : 0.5f;
            var z = Size.Z > 0 ? Mathf.Clamp(delta.Z / Size.Z, 0, 1) : 0.5f;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Converts a parameterized position to a global position. The returned position is clamped to the region.
        /// </summary>
        /// <param name="parameters">The parameterized position.</param>
        public Vector3 InterpolatePosition(Vector3 parameters)
        {
            var topLeft = GlobalPosition - 0.5f * Size;
            var bottomRight = topLeft + Size;

            var x = Mathf.Lerp(topLeft.X, bottomRight.X, Mathf.Clamp(parameters.X, 0, 1));
            var y = Mathf.Lerp(topLeft.Y, bottomRight.Y, Mathf.Clamp(parameters.Y, 0, 1));
            var z = Mathf.Lerp(topLeft.Z, bottomRight.Z, Mathf.Clamp(parameters.Z, 0, 1));

            return new Vector3(x, y, z);
        }
    }
}