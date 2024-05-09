#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Editor;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphEditor : Control
    {
        [Export] public GraphEdit GraphEdit { get; set; }
        [Export] public PackedScene NodeElementScene { get; set; }
        [Export] public PackedScene EdgeElementScene { get; set; }
        private LayoutGraphResource GraphResource { get; set; }
        public Dictionary<int, LayoutGraphNodeElement> NodeElements { get; } = new Dictionary<int, LayoutGraphNodeElement>();
        public Dictionary<Vector2I, LayoutGraphEdgeElement> EdgeElements { get; } = new Dictionary<Vector2I, LayoutGraphEdgeElement>();

        public override void _Ready()
        {
            base._Ready();
            GraphEdit.NodeSelected += OnNodeSelected;
        }

        public void Initialize(LayoutGraphResource graphResource)
        {
            GraphResource = graphResource;
            PopulateCanvas();
        }

        private void OnNodeSelected(Node node)
        {
            switch (node)
            {
                case LayoutGraphNodeElement nodeElement:
                    EditorInterface.Singleton.EditResource(nodeElement.NodeResource);
                    break;
                case LayoutGraphEdgeElement edgeElement:
                    EditorInterface.Singleton.EditResource(edgeElement.EdgeResource);
                    break;
            }
        }

        private void PopulateCanvas()
        {
            ClearCanvas();
            CreateNodeElements();
            CreateEdgeElements();
        }

        private void CreateNodeElements()
        {
            if (!IsInstanceValid(GraphResource))
                return;

            foreach (var node in GraphResource.Nodes.Values)
            {
                var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
                element.Initialize(node);
                GraphEdit.AddChild(element);
            }
        }

        private void CreateEdgeElements()
        {
            if (!IsInstanceValid(GraphResource))
                return;

            foreach (var edge in GraphResource.Edges.Values)
            {
                var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
                element.Initialize(this, edge);
                GraphEdit.AddChild(element);
            }
        }

        public void AddNode(Vector2 position)
        {
            if (!IsInstanceValid(GraphResource))
                return;

            var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
            var nodeResource = GraphResource.AddNode(position);
            element.Initialize(nodeResource);
            GraphEdit.AddChild(element);
        }

        public void AddEdge(int fromNode, int toNode)
        {
            if (!IsInstanceValid(GraphResource))
                return;

            var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
            var edgeResource = GraphResource.AddEdge(fromNode, toNode);
            element.Initialize(this, edgeResource);
            GraphEdit.AddChild(element);
        }

        private void ClearCanvas()
        {
            foreach (var element in NodeElements.Values)
            {
                element.QueueFree();
            }

            foreach (var element in EdgeElements.Values)
            {
                element.QueueFree();
            }

            NodeElements.Clear();
            EdgeElements.Clear();
        }
    }
}
#endif