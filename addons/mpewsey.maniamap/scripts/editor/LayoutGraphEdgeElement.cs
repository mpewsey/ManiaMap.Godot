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
            Populate();
        }

        public void Populate()
        {
            Title = EdgeResource.Name;
            EdgeValueLabel.Text = $"({EdgeResource.FromNode}, {EdgeResource.ToNode})";
            QueueRedraw();
        }

        public override void _Draw()
        {
            base._Draw();
            DrawEdgeLine();
        }

        private void DrawEdgeLine()
        {
            if (!IsInstanceValid(GraphEditor))
                return;

            GraphEditor.NodeElements.TryGetValue(EdgeResource.FromNode, out var fromNode);
            GraphEditor.NodeElements.TryGetValue(EdgeResource.ToNode, out var toNode);

            if (fromNode != null && toNode != null)
            {
                var fromPosition = fromNode.GlobalPosition + 0.5f * fromNode.Size;
                var toPosition = toNode.GlobalPosition + 0.5f * toNode.Size;
                DrawLine(fromPosition, toPosition, EdgeResource.Color);
            }
        }
    }
}
#endif