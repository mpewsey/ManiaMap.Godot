#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Graphs;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class LayoutGraphNodeElement : GraphNode
    {
        [Export] public Label IdValueLabel { get; set; }
        public LayoutGraphNode NodeResource { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            PositionOffsetChanged += OnPositionOffsetChanged;
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
        }

        private void OnResourceChanged()
        {
            Populate();
        }
    }
}
#endif