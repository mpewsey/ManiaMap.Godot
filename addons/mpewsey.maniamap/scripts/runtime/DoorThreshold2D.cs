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
    public partial class DoorThreshold2D : Node2D
    {
        private Vector2 _size = new Vector2(20, 20);
        /// <summary>
        /// The width and height of the threshold.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public Vector2 Size { get => _size; set => SetSize(ref _size, value); }

        private void SetSize(ref Vector2 field, Vector2 value)
        {
            field = new Vector2(Mathf.Max(value.X, 0), Mathf.Max(value.Y, 0));

#if TOOLS
            if (Engine.IsEditorHint())
                QueueRedraw();
#endif
        }

#if TOOLS
        public override void _Ready()
        {
            base._Ready();

            if (Engine.IsEditorHint())
                QueueRedraw();
        }

        public override void _Draw()
        {
            base._Draw();

            if (Engine.IsEditorHint())
                DrawArea();
        }

        private void DrawArea()
        {
            var lineColor = Editor.ManiaMapProjectSettings.GetDoorThreshold2DLineColor();
            var fillColor = Editor.ManiaMapProjectSettings.GetDoorThreshold2DFillColor();
            var rect = new Rect2(GlobalPosition - 0.5f * Size, Size);
            DrawRect(rect, fillColor);
            DrawRect(rect, lineColor, false);
        }
#endif

        /// <summary>
        /// Gets the axis aligned bounding box for the threshold.
        /// </summary>
        private Rect2 GetAABB()
        {
            var size = 0.5f * Size;
            var transform = Transform;

            var min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            Span<Vector2> corners = stackalloc Vector2[]
            {
                new Vector2(1, 1),
                new Vector2(1, -1),
                new Vector2(-1, 1),
                new Vector2(-1, -1),
            };

            foreach (var corner in corners)
            {
                var localPosition = corner * size;
                var position = transform.BasisXform(localPosition);
                min = new Vector2(Mathf.Min(position.X, min.X), Mathf.Min(position.Y, min.Y));
                max = new Vector2(Mathf.Max(position.X, max.X), Mathf.Max(position.Y, max.Y));
            }

            var delta = max - min;
            return new Rect2(GlobalPosition - 0.5f * delta, delta);
        }

        /// <summary>
        /// Converts a global position to a parameterized position on the interval [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2 ParameterizePosition(Vector2 position)
        {
            var bounds = GetAABB();
            var size = bounds.Size;
            var delta = position - bounds.Position;
            var x = size.X > 0 ? Mathf.Clamp(delta.X / size.X, 0, 1) : 0.5f;
            var y = size.Y > 0 ? Mathf.Clamp(delta.Y / size.Y, 0, 1) : 0.5f;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Converts a parameterized position to a global position. The returned position is clamped to the region.
        /// </summary>
        /// <param name="parameters">The parameterized position.</param>
        public Vector2 InterpolatePosition(Vector2 parameters)
        {
            var bounds = GetAABB();
            var topLeft = bounds.Position;
            var bottomRight = topLeft + bounds.Size;

            var x = Mathf.Lerp(topLeft.X, bottomRight.X, Mathf.Clamp(parameters.X, 0, 1));
            var y = Mathf.Lerp(topLeft.Y, bottomRight.Y, Mathf.Clamp(parameters.Y, 0, 1));

            return new Vector2(x, y);
        }
    }
}