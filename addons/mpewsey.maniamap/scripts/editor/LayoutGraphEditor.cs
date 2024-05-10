#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Editor;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphEditor : Control
    {
        private static StringName DeleteAction { get; } = "ui_graph_delete";

        [Export] public GraphEdit GraphEdit { get; set; }
        [Export] public PackedScene NodeElementScene { get; set; }
        [Export] public PackedScene EdgeElementScene { get; set; }

        [Export] public Label FileNameLabel { get; set; }
        [Export] public Button CloseButton { get; set; }
        [Export] public Button SaveButton { get; set; }

        private LayoutGraphResource GraphResource { get; set; }
        public Dictionary<int, LayoutGraphNodeElement> NodeElements { get; } = new Dictionary<int, LayoutGraphNodeElement>();
        public HashSet<LayoutGraphEdgeElement> EdgeElements { get; } = new HashSet<LayoutGraphEdgeElement>();

        public override void _Ready()
        {
            base._Ready();
            GraphEdit.NodeSelected += OnNodeSelected;
            GraphEdit.GuiInput += OnGuiInput;
            GraphEdit.ConnectionRequest += OnConnectionRequest;
            SaveButton.Pressed += OnSubmitSaveButton;
            CloseButton.Pressed += OnSubmitCloseButton;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            CloseOutGraphResource();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            QueueRedraw();
        }

        public override void _Draw()
        {
            base._Draw();

            foreach (var edge in EdgeElements)
            {
                var fromFound = NodeElements.TryGetValue(edge.EdgeResource.FromNode, out var fromNode);
                var toFound = NodeElements.TryGetValue(edge.EdgeResource.ToNode, out var toNode);

                if (fromFound && toFound)
                {
                    var edgePosition = (fromNode.Position + toNode.Position) * 0.5f;
                    edge.PositionOffset = GetPositionOffset(edgePosition);
                }
            }
        }

        private void OnGuiInput(InputEvent input)
        {
            if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.Right && mouseInput.Pressed)
                {
                    AddNode(GetPositionOffset(mouseInput.Position));
                }
            }
            else if (input.IsActionPressed(DeleteAction))
            {
                DeleteSelectedElements();
            }
        }

        private void SelectAllElements()
        {
            foreach (var node in NodeElements.Values)
            {
                node.Selected = true;
            }

            foreach (var edge in EdgeElements)
            {
                edge.Selected = true;
            }
        }

        private void DeleteSelectedElements()
        {
            foreach (var edge in EdgeElements.Where(x => x.Selected).ToList())
            {
                RemoveEdge(edge);
            }

            foreach (var node in NodeElements.Values.Where(x => x.Selected).ToList())
            {
                RemoveNode(node);
            }
        }

        private void RemoveNode(LayoutGraphNodeElement node)
        {
            RemoveNodeEdges(node.NodeResource.Id);
            GraphResource.RemoveNode(node.NodeResource.Id);
            NodeElements.Remove(node.NodeResource.Id);
            node.QueueFree();
        }

        private void RemoveNodeEdges(int nodeId)
        {
            foreach (var edge in NodeEdges(nodeId))
            {
                RemoveEdge(edge);
            }
        }

        private List<LayoutGraphEdgeElement> NodeEdges(int nodeId)
        {
            var result = new List<LayoutGraphEdgeElement>();

            foreach (var edge in EdgeElements)
            {
                if (edge.EdgeResource.ContainsNode(nodeId))
                    result.Add(edge);
            }

            return result;
        }

        private void RemoveEdge(LayoutGraphEdgeElement element)
        {
            GraphResource.RemoveEdge(element.EdgeResource.FromNode, element.EdgeResource.ToNode);
            EdgeElements.Remove(element);
            element.QueueFree();
        }

        private void OnSubmitSaveButton()
        {
            GraphResource?.SaveIfDirty();
            SaveButton.Disabled = true;
        }

        private void OnSubmitCloseButton()
        {
            CloseOutGraphResource();
            ClearCanvas();
            ManiaMapPlugin.Current?.HideGraphEditorDock();
        }

        private void OnConnectionRequest(StringName fromNodeName, long fromSlot, StringName toNodeName, long toSlot)
        {
            var node1 = GraphEdit.GetNode<GraphNode>(fromNodeName.ToString());
            var node2 = GraphEdit.GetNode<GraphNode>(toNodeName.ToString());

            if (node1 is LayoutGraphNodeElement fromNode && node2 is LayoutGraphNodeElement toNode)
                AddEdge(fromNode.NodeResource.Id, toNode.NodeResource.Id);
        }

        private void OnGraphResourceChanged()
        {
            SaveButton.Disabled = false;
        }

        public Vector2 GetPositionOffset(Vector2 position)
        {
            return (position + GraphEdit.ScrollOffset) / GraphEdit.Zoom;
        }

        public void SetEditorTarget(LayoutGraphResource graphResource)
        {
            CloseOutGraphResource();
            GraphResource = graphResource;
            SaveButton.Disabled = false;
            FileNameLabel.Text = graphResource.ResourcePath.GetFile();
            RegisterChangedSignals();
            ClearCanvas();
            CreateEdgeElements();
            CreateNodeElements();
        }

        private void CloseOutGraphResource()
        {
            if (GraphResource != null)
            {
                GraphResource.SaveIfDirty();
                GraphResource.Changed -= OnGraphResourceChanged;
                GraphResource.UnregisterOnSubresourceChangedSignals();
                GraphResource = null;
            }
        }

        private void RegisterChangedSignals()
        {
            GraphResource.Changed += OnGraphResourceChanged;
            GraphResource.RegisterOnSubresourceChangedSignals();
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

        private LayoutGraphNodeElement CreateNodeElement(LayoutGraphNode node)
        {
            var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
            GraphEdit.AddChild(element);
            NodeElements.Add(node.Id, element);
            element.Initialize(node);
            return element;
        }

        private LayoutGraphEdgeElement CreateEdgeElement(LayoutGraphEdge edge)
        {
            var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
            GraphEdit.AddChild(element);
            EdgeElements.Add(element);
            element.Initialize(this, edge);
            return element;
        }

        private void CreateNodeElements()
        {
            foreach (var node in GraphResource.Nodes.Values)
            {
                CreateNodeElement(node);
            }
        }

        private void CreateEdgeElements()
        {
            foreach (var edge in GraphResource.Edges.Values)
            {
                CreateEdgeElement(edge);
            }
        }

        public void AddNode(Vector2 position)
        {
            var node = GraphResource.AddNode(position);
            CreateNodeElement(node);
            GraphResource.SetDirty();
        }

        public void AddEdge(int fromNode, int toNode)
        {
            var edge = GraphResource.AddEdge(fromNode, toNode);

            if (edge != null)
            {
                CreateEdgeElement(edge);
                GraphResource.SetDirty();
            }
        }

        public void ClearCanvas()
        {
            foreach (var node in NodeElements.Values)
            {
                node.QueueFree();
            }

            foreach (var edge in EdgeElements)
            {
                edge.QueueFree();
            }

            NodeElements.Clear();
            EdgeElements.Clear();
        }
    }
}
#endif