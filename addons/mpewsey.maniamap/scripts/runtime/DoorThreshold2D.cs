using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class DoorThreshold2D : Node2D
    {
        private float _width = 20;
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public float Width { get => _width; set => SetField(ref _width, value); }

        private float _height = 20;
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public float Height { get => _height; set => SetField(ref _height, value); }

        private void SetField<T>(ref T field, T value)
        {
            field = value;

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
            var size = new Vector2(Width, Height);
            var lineColor = new Color(1, 1, 0);
            var fillColor = new Color(1, 1, 0, 0.1f);
            var rect = new Rect2(GlobalPosition - 0.5f * size, size);
            DrawRect(rect, fillColor);
            DrawRect(rect, lineColor, false);
        }
#endif

        public Vector2 ParameterizePosition(Vector2 position)
        {
            var size = new Vector2(Width, Height);
            var topLeft = GlobalPosition - 0.5f * size;
            var delta = position - topLeft;
            var x = size.X > 0 ? Mathf.Clamp(delta.X / size.X, 0, 1) : 0.5f;
            var y = size.Y > 0 ? Mathf.Clamp(delta.Y / size.Y, 0, 1) : 0.5f;
            return new Vector2(x, y);
        }

        public Vector2 InterpolatePosition(Vector2 parameters)
        {
            var size = new Vector2(Width, Height);
            var topLeft = GlobalPosition - 0.5f * size;
            var bottomRight = topLeft + size;

            var tx = Mathf.Clamp(parameters.X, 0, 1);
            var ty = Mathf.Clamp(parameters.Y, 0, 1);

            var x = Mathf.Lerp(topLeft.X, bottomRight.X, tx);
            var y = Mathf.Lerp(topLeft.Y, bottomRight.Y, ty);

            return new Vector2(x, y);
        }
    }
}