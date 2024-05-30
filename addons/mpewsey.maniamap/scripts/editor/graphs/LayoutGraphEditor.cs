#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Editor;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Graphs.Editor
{
    [Tool]
    public partial class LayoutGraphEditor : Control
    {
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
        private List<LayoutGraphEdgeLine> EdgeLines { get; } = new List<LayoutGraphEdgeLine>();
        private HashSet<Resource> SelectedResources { get; } = new HashSet<Resource>();

        public override void _Ready()
        {
            base._Ready();
            GraphEdit.NodeSelected += OnNodeSelected;
            GraphEdit.NodeDeselected += OnNodeDeselected;
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

        private void DisplayNodes(bool visible)
        {
            foreach (var node in NodeElements.Values)
            {
                node.Visible = visible;

                if (!visible)
                    node.Selected = false;
            }
        }

        private void DisplayEdges(bool visible)
        {
            foreach (var edge in EdgeElements)
            {
                edge.Visible = visible;

                if (!visible)
                    edge.Selected = false;
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
                line.Populate(element, NodeElements, zoom);
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
                var line = EdgeLineScene.Instantiate<LayoutGraphEdgeLine>();
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
            else if (input.IsActionPressed(EditorInputs.DeleteAction))
            {
                DeleteSelectedElements();
                GetViewport().SetInputAsHandled();
            }
            else if (input.IsActionPressed(EditorInputs.SelectAllAction))
            {
                SelectAllElements();
                GetViewport().SetInputAsHandled();
            }
            else if (input.IsActionPressed(EditorInputs.SelectAllNodesAction))
            {
                SelectAllNodeElements();
                GetViewport().SetInputAsHandled();
            }
            else if (input.IsActionPressed(EditorInputs.SelectAllEdgesAction))
            {
                SelectAllEdgeElements();
                GetViewport().SetInputAsHandled();
            }
        }

        private void SelectAllNodeElements()
        {
            foreach (var node in NodeElements.Values)
            {
                node.Selected = true;
            }
        }

        private void SelectAllEdgeElements()
        {
            foreach (var edge in EdgeElements)
            {
                edge.Selected = EdgeDisplayButton.ButtonPressed;
            }
        }

        private void SelectAllElements()
        {
            SelectAllEdgeElements();
            SelectAllNodeElements();
        }

        private void DeleteSelectedElements()
        {
            SelectedResources.Clear();

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

        private static void ToggleButton(Button button, bool toggled)
        {
            button.SetPressedNoSignal(toggled);
            button.Flat = !toggled;
        }

        private void OnToggleEdgeDisplayButton(bool toggled)
        {
            ToggleButton(EdgeDisplayButton, toggled);
            DisplayEdges(toggled);
            DisplayResourceEditor();
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

            if (IsInstanceValid(ManiaMapPlugin.Current))
                ManiaMapPlugin.Current.HideGraphEditorDock();
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
            FileNameLabel.Text = GraphResource.ResourcePath.GetFile();
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

        private void OnNodeDeselected(Node node)
        {
            switch (node)
            {
                case LayoutGraphNodeElement nodeElement:
                    SelectedResources.Remove(nodeElement.NodeResource);
                    break;
                case LayoutGraphEdgeElement edgeElement:
                    SelectedResources.Remove(edgeElement.EdgeResource);
                    break;
            }

            DisplayResourceEditor();
        }

        private void OnNodeSelected(Node node)
        {
            switch (node)
            {
                case LayoutGraphNodeElement nodeElement:
                    SelectedResources.Add(nodeElement.NodeResource);
                    break;
                case LayoutGraphEdgeElement edgeElement:
                    SelectedResources.Add(edgeElement.EdgeResource);
                    break;
            }

            DisplayResourceEditor();
        }

        private void DisplayResourceEditor()
        {
            if (SelectedResources.Count > 1)
            {
                var multiEditor = new LayoutGraphMultiEditor(SelectedResources);
                EditorInterface.Singleton.EditResource(multiEditor);
                return;
            }

            if (SelectedResources.Count == 1)
            {
                switch (SelectedResources.First())
                {
                    case LayoutGraphNode nodeResource:
                        EditorInterface.Singleton.EditResource(nodeResource);
                        break;
                    case LayoutGraphEdge edgeResource:
                        EditorInterface.Singleton.EditResource(edgeResource);
                        break;
                }
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

            SelectedResources.Clear();
            NodeElements.Clear();
            EdgeElements.Clear();
            EdgeLines.Clear();
        }
    }
}
#endif