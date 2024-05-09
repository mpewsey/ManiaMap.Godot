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
            GraphEdit.GuiInput += OnGuiInput;
        }

        private void OnGuiInput(InputEvent input)
        {
            if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.Right && mouseInput.Pressed)
                {
                    AddNode((mouseInput.Position + GraphEdit.ScrollOffset) / GraphEdit.Zoom);
                }
            }
        }

        public void Initialize(LayoutGraphResource graphResource)
        {
            GraphResource = graphResource;
            ClearCanvas();
            CreateEdgeElements();
            CreateNodeElements();
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

        private void CreateNodeElements()
        {
            if (IsInstanceValid(GraphResource))
            {
                foreach (var node in GraphResource.Nodes.Values)
                {
                    var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
                    GraphEdit.AddChild(element);
                    element.Initialize(node);
                }
            }
        }

        private void CreateEdgeElements()
        {
            if (IsInstanceValid(GraphResource))
            {
                foreach (var edge in GraphResource.Edges.Values)
                {
                    var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
                    GraphEdit.AddChild(element);
                    element.Initialize(this, edge);
                }
            }
        }

        public void AddNode(Vector2 position)
        {
            if (IsInstanceValid(GraphResource))
            {
                var nodeResource = GraphResource.AddNode(position);
                var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
                GraphEdit.AddChild(element);
                element.Initialize(nodeResource);
            }
        }

        public void AddEdge(int fromNode, int toNode)
        {
            if (IsInstanceValid(GraphResource))
            {
                var edgeResource = GraphResource.AddEdge(fromNode, toNode);

                if (IsInstanceValid(edgeResource))
                {
                    var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
                    GraphEdit.AddChild(element);
                    element.Initialize(this, edgeResource);
                }
            }
        }

        public void ClearCanvas()
        {
            var nodes = GraphEdit.FindChildren("*", nameof(GraphNode), true, false);

            foreach (var node in nodes)
            {
                node.QueueFree();
            }

            NodeElements.Clear();
            EdgeElements.Clear();
        }
    }
}
#endif