using Godot;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphResource : Resource
    {
        [Export] public int Id { get; set; } = Rand.GetRandomId();
        [Export] public string Name { get; set; } = "<None>";
        [Export] public Godot.Collections.Dictionary<int, LayoutGraphNode> Nodes { get; set; } = new Godot.Collections.Dictionary<int, LayoutGraphNode>();
        [Export] public Godot.Collections.Dictionary<Vector2I, LayoutGraphEdge> Edges { get; set; } = new Godot.Collections.Dictionary<Vector2I, LayoutGraphEdge>();
        private bool IsDirty { get; set; }

        public void RegisterOnSubresourceChangedSignals()
        {
            foreach (var node in Nodes.Values)
            {
                node.Changed += OnSubresourceChanged;
            }

            foreach (var edge in Edges.Values)
            {
                edge.Changed += OnSubresourceChanged;
            }
        }

        private void OnSubresourceChanged()
        {
            SetDirty();
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public bool SaveIfDirty()
        {
            if (IsDirty && FileAccess.FileExists(ResourcePath))
            {
                ResourceSaver.Save(this);
                IsDirty = false;
                return true;
            }

            return false;
        }

        public bool ContainsEdge(int node1, int node2)
        {
            return Edges.ContainsKey(new Vector2I(node1, node2))
                || Edges.ContainsKey(new Vector2I(node2, node1));
        }

        public bool ContainsNodes(int node1, int node2)
        {
            return Nodes.ContainsKey(node1) && Nodes.ContainsKey(node2);
        }

        public LayoutGraphEdge GetEdge(int node1, int node2)
        {
            if (Edges.TryGetValue(new Vector2I(node1, node2), out var edge1))
                return edge1;
            if (Edges.TryGetValue(new Vector2I(node2, node1), out var edge2))
                return edge2;
            throw new Exception($"Edge does not exist: ({node1}, {node2}).");
        }

        public LayoutGraphNode AddNode(Vector2 position)
        {
            return AddNode(GetNewNodeId(), position);
        }

        private int GetNewNodeId()
        {
            var id = 1;

            while (Nodes.ContainsKey(id))
            {
                id++;
            }

            return id;
        }

        public LayoutGraphNode AddNode(int nodeId, Vector2 position)
        {
            if (Nodes.ContainsKey(nodeId))
                throw new Exception($"Node already exists: {nodeId}.");

            var node = new LayoutGraphNode(nodeId, position);
            Nodes.Add(nodeId, node);
            node.Changed += OnSubresourceChanged;
            SetDirty();
            return node;
        }

        public LayoutGraphEdge AddEdge(int fromNode, int toNode)
        {
            if (!ContainsEdge(fromNode, toNode))
            {
                var edge = new LayoutGraphEdge(fromNode, toNode);
                Edges.Add(new Vector2I(fromNode, toNode), edge);
                edge.Changed += OnSubresourceChanged;
                SetDirty();
                return edge;
            }

            return null;
        }
    }
}