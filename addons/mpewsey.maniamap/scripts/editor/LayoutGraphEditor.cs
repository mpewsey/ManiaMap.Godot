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

        [Export] public Label FileNameLabel { get; set; }
        [Export] public Button CloseButton { get; set; }
        [Export] public Button SaveButton { get; set; }

        private LayoutGraphResource GraphResource { get; set; }
        public Dictionary<int, LayoutGraphNodeElement> NodeElements { get; } = new Dictionary<int, LayoutGraphNodeElement>();
        public Dictionary<Vector2I, LayoutGraphEdgeElement> EdgeElements { get; } = new Dictionary<Vector2I, LayoutGraphEdgeElement>();

        public override void _Ready()
        {
            base._Ready();
            GraphEdit.NodeSelected += OnNodeSelected;
            GraphEdit.GuiInput += OnGuiInput;
            SaveButton.Pressed += OnSubmitSaveButton;
            CloseButton.Pressed += OnSubmitCloseButton;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            CloseOutGraphResource();
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

        private void OnGraphResourceChanged()
        {
            SaveButton.Disabled = false;
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

        private void CreateNodeElements()
        {
            foreach (var node in GraphResource.Nodes.Values)
            {
                var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
                GraphEdit.AddChild(element);
                element.Initialize(node);
            }
        }

        private void CreateEdgeElements()
        {
            foreach (var edge in GraphResource.Edges.Values)
            {
                var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
                GraphEdit.AddChild(element);
                element.Initialize(this, edge);
            }
        }

        public void AddNode(Vector2 position)
        {
            var nodeResource = GraphResource.AddNode(position);
            var element = NodeElementScene.Instantiate<LayoutGraphNodeElement>();
            GraphEdit.AddChild(element);
            element.Initialize(nodeResource);
            GraphResource.SetDirty();
        }

        public void AddEdge(int fromNode, int toNode)
        {
            var edgeResource = GraphResource.AddEdge(fromNode, toNode);

            if (edgeResource != null)
            {
                var element = EdgeElementScene.Instantiate<LayoutGraphEdgeElement>();
                GraphEdit.AddChild(element);
                element.Initialize(this, edgeResource);
                GraphResource.SetDirty();
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