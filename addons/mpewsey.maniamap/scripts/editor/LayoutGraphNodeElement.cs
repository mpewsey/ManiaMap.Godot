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

        public void Initialize(LayoutGraphNode nodeResource)
        {
            NodeResource = nodeResource;
            Populate();
        }

        public void Populate()
        {
            Title = NodeResource.Name;
            IdValueLabel.Text = NodeResource.Id.ToString();
        }
    }
}
#endif