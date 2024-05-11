#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Graphs;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class LayoutGraphNodeElement : GraphNode
    {
        [Export] public Label IdValueLabel { get; set; }
        [Export] public ColorPickerButton ColorPicker { get; set; }
        public LayoutGraphNode NodeResource { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            GuiInput += OnGuiInput;
            ColorPicker.ColorChanged += OnColorChanged;
            PositionOffsetChanged += OnPositionOffsetChanged;
        }

        private void OnGuiInput(InputEvent input)
        {
            if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.Right && mouseInput.Pressed)
                    GetViewport().SetInputAsHandled();
            }
        }

        private void OnPositionOffsetChanged()
        {
            NodeResource.Position = PositionOffset;
        }

        public void Initialize(LayoutGraphNode nodeResource)
        {
            NodeResource = nodeResource;
            PositionOffset = nodeResource.Position;
            nodeResource.Changed += OnResourceChanged;
            Populate();
        }

        public void Populate()
        {
            Title = NodeResource.Name;
            IdValueLabel.Text = NodeResource.Id.ToString();
            ColorPicker.Color = NodeResource.Color;
        }

        private void OnResourceChanged()
        {
            Populate();
        }

        private void OnColorChanged(Color color)
        {
            NodeResource.Color = color;
        }
    }
}
#endif