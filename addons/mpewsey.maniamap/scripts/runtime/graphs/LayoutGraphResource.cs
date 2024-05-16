using Godot;
using MPewsey.ManiaMap.Graphs;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphResource : Resource
    {
        private int _id = Rand.GetRandomId();
        [Export] public int Id { get => _id; set => SetField(ref _id, value); }

        private string _name = "<None>";
        [Export] public string Name { get => _name; set => SetField(ref _name, value); }

        [Export] public Godot.Collections.Dictionary<int, LayoutGraphNode> Nodes { get; set; } = new Godot.Collections.Dictionary<int, LayoutGraphNode>();
        [Export] public Godot.Collections.Dictionary<Vector2I, LayoutGraphEdge> Edges { get; set; } = new Godot.Collections.Dictionary<Vector2I, LayoutGraphEdge>();
        private bool IsDirty { get; set; }

        private void SetField<T>(ref T field, T value)
        {
            field = value;
            EmitChanged();
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();
            var usage = property["usage"].As<PropertyUsageFlags>();

            if (name == PropertyName.Nodes || name == PropertyName.Edges)
                property["usage"] = (int)(usage & ~PropertyUsageFlags.Editor);
        }

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

        public void UnregisterOnSubresourceChangedSignals()
        {
            foreach (var node in Nodes.Values)
            {
                node.Changed -= OnSubresourceChanged;
            }

            foreach (var edge in Edges.Values)
            {
                edge.Changed -= OnSubresourceChanged;
            }
        }

        private void OnSubresourceChanged()
        {
            SetDirty();
        }

        public void SetDirty()
        {
            IsDirty = true;
            EmitChanged();
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

        public LayoutGraphEdge GetEdge(int node1, int node2)
        {
            if (Edges.TryGetValue(new Vector2I(node1, node2), out var edge1))
                return edge1;
            if (Edges.TryGetValue(new Vector2I(node2, node1), out var edge2))
                return edge2;
            throw new ArgumentException($"Edge does not exist: ({node1}, {node2}).");
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
                throw new ArgumentException($"Node already exists: {nodeId}.");

            var node = new LayoutGraphNode(nodeId, position);
            Nodes.Add(nodeId, node);
            node.Changed += OnSubresourceChanged;
            SetDirty();
            return node;
        }

        public bool RemoveNode(int nodeId)
        {
            RemoveNodeEdges(nodeId);

            if (Nodes.Remove(nodeId))
            {
                SetDirty();
                return true;
            }

            return false;
        }

        public bool RemoveNodeEdges(int nodeId)
        {
            var result = false;

            foreach (var index in NodeEdgeIndexes(nodeId))
            {
                result |= Edges.Remove(index);
            }

            if (result)
                SetDirty();

            return result;
        }

        public List<Vector2I> NodeEdgeIndexes(int nodeId)
        {
            var result = new List<Vector2I>();

            foreach (var pair in Edges)
            {
                if (pair.Value.ContainsNode(nodeId))
                    result.Add(pair.Key);
            }

            return result;
        }

        public LayoutGraphEdge AddEdge(int fromNode, int toNode)
        {
            if (!Nodes.ContainsKey(fromNode))
                throw new ArgumentException($"{nameof(fromNode)} does not exist: {fromNode}.");
            if (!Nodes.ContainsKey(toNode))
                throw new ArgumentException($"{nameof(toNode)} does not exist: {toNode}.");

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

        public bool RemoveEdge(int node1, int node2)
        {
            if (Edges.Remove(new Vector2I(node1, node2)) || Edges.Remove(new Vector2I(node2, node1)))
            {
                SetDirty();
                return true;
            }

            return false;
        }

        public HashSet<TemplateGroup> GetTemplateGroups()
        {
            var result = new HashSet<TemplateGroup>();

            foreach (var node in Nodes.Values)
            {
                result.Add(node.TemplateGroup);
            }

            foreach (var edge in Edges.Values)
            {
                if (edge.TemplateGroup != null)
                    result.Add(edge.TemplateGroup);
            }

            return result;
        }

        public LayoutGraph GetMMLayoutGraph()
        {
            var graph = new LayoutGraph(Id, Name);
            AddMMLayoutNodes(graph);
            AddMMLayoutEdges(graph);
            graph.Validate();
            return graph;
        }

        private void AddMMLayoutNodes(LayoutGraph graph)
        {
            foreach (var node in Nodes.Values)
            {
                node.AddMMLayoutNode(graph);
            }
        }

        private void AddMMLayoutEdges(LayoutGraph graph)
        {
            foreach (var edge in Edges.Values)
            {
                edge.AddMMLayoutEdge(graph);
            }
        }
    }
}