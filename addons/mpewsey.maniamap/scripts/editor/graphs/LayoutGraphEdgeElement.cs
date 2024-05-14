#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Graphs.Editor
{
    [Tool]
    public partial class LayoutGraphEdgeElement : GraphNode
    {
        [Export] public Label EdgeValueLabel { get; set; }
        [Export] public ColorPickerButton ColorPicker { get; set; }
        public LayoutGraphEdge EdgeResource { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            GuiInput += OnGuiInput;
            ColorPicker.ColorChanged += OnColorChanged;
        }

        private void OnGuiInput(InputEvent input)
        {
            if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.Right && mouseInput.Pressed)
                    GetViewport().SetInputAsHandled();
            }
        }

        public void Initialize(LayoutGraphEdge edgeResource)
        {
            EdgeResource = edgeResource;
            edgeResource.Changed += OnResourceChanged;
            Populate();
        }

        public void Populate()
        {
            Title = EdgeResource.Name;
            EdgeValueLabel.Text = $"({EdgeResource.FromNode}, {EdgeResource.ToNode})";
            ColorPicker.Color = EdgeResource.Color;
        }

        private void OnResourceChanged()
        {
            Populate();
        }

        private void OnColorChanged(Color color)
        {
            EdgeResource.Color = color;
        }
    }
}
#endif