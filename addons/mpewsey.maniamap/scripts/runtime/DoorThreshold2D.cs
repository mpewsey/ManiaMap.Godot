using Godot;

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
        /// Converts a global position to a parameterized position on the interval [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2 ParameterizePosition(Vector2 position)
        {
            var topLeft = GlobalPosition - 0.5f * Size;
            var delta = position - topLeft;
            var x = Size.X > 0 ? Mathf.Clamp(delta.X / Size.X, 0, 1) : 0.5f;
            var y = Size.Y > 0 ? Mathf.Clamp(delta.Y / Size.Y, 0, 1) : 0.5f;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Converts a parameterized position to a global position. The returned position is clamped to the region.
        /// </summary>
        /// <param name="parameters">The parameterized position.</param>
        public Vector2 InterpolatePosition(Vector2 parameters)
        {
            var topLeft = GlobalPosition - 0.5f * Size;
            var bottomRight = topLeft + Size;

            var x = Mathf.Lerp(topLeft.X, bottomRight.X, Mathf.Clamp(parameters.X, 0, 1));
            var y = Mathf.Lerp(topLeft.Y, bottomRight.Y, Mathf.Clamp(parameters.Y, 0, 1));

            return new Vector2(x, y);
        }
    }
}