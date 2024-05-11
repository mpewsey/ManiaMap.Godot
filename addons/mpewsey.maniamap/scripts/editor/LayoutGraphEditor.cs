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
        private static StringName SelectAllAction { get; } = "ui_text_select_all";

        [Export] public GraphEdit GraphEdit { get; set; }
        [Export] public Node EdgeLineContainer { get; set; }
        [Export] public PackedScene NodeElementScene { get; set; }
        [Export] public PackedScene EdgeElementScene { get; set; }
        [Export] public PackedScene EdgeLineScene { get; set; }

        [Export] public Label FileNameLabel { get; set; }
        [Export] public Button CloseButton { get; set; }
        [Export] public Button SaveButton { get; set; }
        [Export] public Button EdgeDisplayButton { get; set; }

        private LayoutGraphResource GraphResource { get; set; }
        public Dictionary<int, LayoutGraphNodeElement> NodeElements { get; } = new Dictionary<int, LayoutGraphNodeElement>();
        public HashSet<LayoutGraphEdgeElement> EdgeElements { get; } = new HashSet<LayoutGraphEdgeElement>();
        private List<Line2D> EdgeLines { get; } = new List<Line2D>();
        private float LineWidth { get; set; }

        public override void _Ready()
        {
            base._Ready();
            GraphEdit.NodeSelected += OnNodeSelected;
            GraphEdit.GuiInput += OnGuiInput;
            GraphEdit.ConnectionRequest += OnConnectionRequest;
            SaveButton.Pressed += OnSubmitSaveButton;
            CloseButton.Pressed += OnSubmitCloseButton;
            EdgeDisplayButton.Toggled += OnToggleEdgeDisplayButton;
            OnToggleEdgeDisplayButton(true);
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            CloseOutGraphResource();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            MoveEdgesToMidpointsOfNodes();
            PopulateEdgeLines();
        }

        private void DisplayEdges(bool visible)
        {
            foreach (var edge in EdgeElements)
            {
                edge.Visible = visible;
            }
        }

        private void MoveEdgesToMidpointsOfNodes()
        {
            foreach (var element in EdgeElements)
            {
                var edge = element.EdgeResource;
                var fromFound = NodeElements.TryGetValue(edge.FromNode, out var fromNode);
                var toFound = NodeElements.TryGetValue(edge.ToNode, out var toNode);

                if (fromFound && toFound)
                {
                    var zoom = GraphEdit.Zoom;
                    var fromPosition = fromNode.Position + 0.5f * zoom * fromNode.Size;
                    var toPosition = toNode.Position + 0.5f * zoom * toNode.Size;
                    var edgePosition = (fromPosition + toPosition) * 0.5f - 0.5f * zoom * element.Size;
                    element.PositionOffset = GetPositionOffset(edgePosition);
                }
            }
        }

        private void PopulateEdgeLines()
        {
            SizeEdgeLinesList();
            var index = 0;
            var zoom = GraphEdit.Zoom;

            foreach (var element in EdgeElements)
            {
                var line = EdgeLines[index++];
                var edge = element.EdgeResource;
                var fromPosition = Vector2.Zero;
                var toPosition = Vector2.Zero;
                var fromFound = NodeElements.TryGetValue(edge.FromNode, out var fromNode);
                var toFound = NodeElements.TryGetValue(edge.ToNode, out var toNode);

                if (fromFound && toFound)
                {
                    fromPosition = fromNode.Position + 0.5f * zoom * fromNode.Size;
                    toPosition = toNode.Position + 0.5f * zoom * toNode.Size;
                }

                if (line.Points.Length != 2)
                    line.Points = new Vector2[2];

                line.Width = Mathf.Max(LineWidth * zoom, 1);
                line.DefaultColor = edge.Color;
                line.SetPointPosition(0, fromPosition);
                line.SetPointPosition(1, toPosition);
            }
        }

        private void SizeEdgeLinesList()
        {
            while (EdgeLines.Count > EdgeElements.Count)
            {
                var index = EdgeLines.Count - 1;
                EdgeLines[index].QueueFree();
                EdgeLines.RemoveAt(index);
            }

            while (EdgeLines.Count < EdgeElements.Count)
            {
                var line = EdgeLineScene.Instantiate<Line2D>();
                LineWidth = line.Width;
                EdgeLineContainer.AddChild(line);
                EdgeLines.Add(line);
            }
        }

        private void OnGuiInput(InputEvent input)
        {
            if (input is InputEventMouseButton mouseInput)
            {
                if (mouseInput.ButtonIndex == MouseButton.Right && mouseInput.Pressed)
                {
                    AddNode(GetPositionOffset(mouseInput.Position));
                    GetViewport().SetInputAsHandled();
                }
            }
            else if (input.IsActionPressed(DeleteAction))
            {
                DeleteSelectedElements();
                GetViewport().SetInputAsHandled();
            }
            else if (input.IsActionPressed(SelectAllAction))
            {
                SelectAllElements();
                GetViewport().SetInputAsHandled();
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

        private void OnToggleEdgeDisplayButton(bool toggled)
        {
            EdgeDisplayButton.SetPressedNoSignal(toggled);
            EdgeDisplayButton.Flat = !toggled;
            DisplayEdges(toggled);
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
            GraphEdit.MoveChild(element, 1);
            EdgeElements.Add(element);
            element.Initialize(edge);
            element.Visible = EdgeDisplayButton.ButtonPressed;
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

            foreach (var line in EdgeLines)
            {
                line.QueueFree();
            }

            NodeElements.Clear();
            EdgeElements.Clear();
            EdgeLines.Clear();
        }
    }
}
#endif