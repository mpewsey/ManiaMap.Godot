using Godot;
using System;

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
        /// Gets the axis aligned bounding box for the threshold.
        /// </summary>
        private Aabb GetAABB()
        {
            var size = 0.5f * Size;
            var basis = Transform.Basis;

            var min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            Span<Vector3> corners = stackalloc Vector3[]
            {
                new Vector3(-1, -1, -1),
                new Vector3(1, -1, -1),
                new Vector3(1, 1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, 1, 1),
                new Vector3(-1, 1, 1),
            };

            foreach (var corner in corners)
            {
                var localPosition = corner * size;
                var x = basis.X.Dot(localPosition);
                var y = basis.Y.Dot(localPosition);
                var z = basis.Z.Dot(localPosition);
                min = new Vector3(Mathf.Min(x, min.X), Mathf.Min(y, min.Y), Mathf.Min(z, min.Z));
                max = new Vector3(Mathf.Max(x, max.X), Mathf.Max(y, max.Y), Mathf.Max(z, max.Z));
            }

            var delta = max - min;
            return new Aabb(GlobalPosition - 0.5f * delta, delta);
        }

        /// <summary>
        /// Converts a global position to a parameterized position on the interval [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector3 ParameterizePosition(Vector3 position)
        {
            var bounds = GetAABB();
            var size = bounds.Size;
            var delta = position - bounds.Position;
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
            var bounds = GetAABB();
            var size = bounds.Size;
            var topLeft = bounds.Position;
            var bottomRight = topLeft + size;

            var x = Mathf.Lerp(topLeft.X, bottomRight.X, Mathf.Clamp(parameters.X, 0, 1));
            var y = Mathf.Lerp(topLeft.Y, bottomRight.Y, Mathf.Clamp(parameters.Y, 0, 1));
            var z = Mathf.Lerp(topLeft.Z, bottomRight.Z, Mathf.Clamp(parameters.Z, 0, 1));

            return new Vector3(x, y, z);
        }
    }
}