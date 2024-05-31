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
        private float _width = 20;
        /// <summary>
        /// The width of the rectangular region.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,0.001,or_greater")] public float Width { get => _width; set => SetField(ref _width, value); }

        private float _height = 20;
        /// <summary>
        /// The height of the rectangular region.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,0.001,or_greater")] public float Height { get => _height; set => SetField(ref _height, value); }

        private float _depth = 20;
        /// <summary>
        /// The height of the rectangular region.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,0.001,or_greater")] public float Depth { get => _depth; set => SetField(ref _depth, value); }

        private void SetField<T>(ref T field, T value)
        {
            field = value;
        }

        /// <summary>
        /// Converts a global position to a parameterized position on the interval [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector3 ParameterizePosition(Vector3 position)
        {
            var size = new Vector3(Width, Height, Depth);
            var topLeft = GlobalPosition - 0.5f * size;
            var delta = position - topLeft;
            var x = size.X > 0 ? Mathf.Clamp(delta.X / size.X, 0, 1) : 0.5f;
            var y = size.Y > 0 ? Mathf.Clamp(delta.Y / size.Y, 0, 1) : 0.5f;
            var z = size.Z > 0 ? Mathf.Clamp(delta.Z / size.Z, 0, 1) : 0.5f;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Converts a parameterized position to a global position. The returned position is clamped to the region.
        /// </summary>
        /// <param name="parameters">The parameterized position.</param>
        public Vector3 InterpolatePosition(Vector3 parameters)
        {
            var size = new Vector3(Width, Height, Depth);
            var topLeft = GlobalPosition - 0.5f * size;
            var bottomRight = topLeft + size;

            var tx = Mathf.Clamp(parameters.X, 0, 1);
            var ty = Mathf.Clamp(parameters.Y, 0, 1);
            var tz = Mathf.Clamp(parameters.Z, 0, 1);

            var x = Mathf.Lerp(topLeft.X, bottomRight.X, tx);
            var y = Mathf.Lerp(topLeft.Y, bottomRight.Y, ty);
            var z = Mathf.Lerp(topLeft.Z, bottomRight.Z, tz);

            return new Vector3(x, y, z);
        }
    }
}