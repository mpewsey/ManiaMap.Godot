#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Graphs;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class LayoutGraphEdgeElement : GraphNode
    {
        [Export] public Label EdgeValueLabel { get; set; }
        private LayoutGraphEditor GraphEditor { get; set; }
        public LayoutGraphEdge EdgeResource { get; private set; }

        public void Initialize(LayoutGraphEditor graphEditor, LayoutGraphEdge edgeResource)
        {
            GraphEditor = graphEditor;
            EdgeResource = edgeResource;
            edgeResource.Changed += OnResourceChanged;
            Populate();
        }

        public void Populate()
        {
            Title = EdgeResource.Name;
            EdgeValueLabel.Text = $"({EdgeResource.FromNode}, {EdgeResource.ToNode})";
            SelfModulate = EdgeResource.Color;
        }

        private void OnResourceChanged()
        {
            Populate();
        }
    }
}
#endif