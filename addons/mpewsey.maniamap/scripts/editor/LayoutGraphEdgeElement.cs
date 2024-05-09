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
            QueueRedraw();
        }

        private void OnResourceChanged()
        {
            Populate();
        }

        public override void _Draw()
        {
            base._Draw();
            DrawEdgeLine();
        }

        private void DrawEdgeLine()
        {
            if (GraphEditor != null)
            {
                var fromFound = GraphEditor.NodeElements.TryGetValue(EdgeResource.FromNode, out var fromNode);
                var toFound = GraphEditor.NodeElements.TryGetValue(EdgeResource.ToNode, out var toNode);

                if (fromFound && toFound)
                {
                    var fromPosition = fromNode.CenterPosition();
                    var toPosition = toNode.CenterPosition();
                    DrawLine(fromPosition, toPosition, EdgeResource.Color);
                }
            }
        }
    }
}
#endif