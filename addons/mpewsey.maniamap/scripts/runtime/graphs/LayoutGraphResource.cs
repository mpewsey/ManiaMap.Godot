using Godot;
using MPewsey.ManiaMap.Graphs;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Graphs
{
    /// <summary>
    /// A graph of nodes and edges representing rooms. Used to guide the procedural generator.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphResource : Resource
    {
        private int _id = Rand.GetRandomId();
        /// <summary>
        /// The unique graph ID.
        /// </summary>
        [Export] public int Id { get => _id; set => SetField(ref _id, value); }

        private string _name = "<None>";
        /// <summary>
        /// The graph name.
        /// </summary>
        [Export] public string Name { get => _name; set => SetField(ref _name, value); }

        /// <summary>
        /// A dictionary of nodes by ID.
        /// </summary>
        [Export] public Godot.Collections.Dictionary<int, LayoutGraphNode> Nodes { get; set; } = new Godot.Collections.Dictionary<int, LayoutGraphNode>();

        /// <summary>
        /// A dictionary of edges by node ID's.
        /// </summary>
        [Export] public Godot.Collections.Dictionary<Vector2I, LayoutGraphEdge> Edges { get; set; } = new Godot.Collections.Dictionary<Vector2I, LayoutGraphEdge>();

        /// <summary>
        /// If true, the graph has unsaved changes.
        /// </summary>
        public bool IsDirty { get; private set; }

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

        /// <summary>
        /// Adds the `OnSubresourceChanged` signal to all nodes and edges.
        /// </summary>
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

        /// <summary>
        /// Removes the `OnSubresourceChanged` signal from all nodes and edges.
        /// </summary>
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

        /// <summary>
        /// Marks the graph as dirty.
        /// </summary>
        public void SetDirty()
        {
            IsDirty = true;
            EmitChanged();
        }

        /// <summary>
        /// Saves the graph if it is dirty and the resource file still exists.
        /// </summary>
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

        /// <summary>
        /// Returns true if the graph contains the edge.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public bool ContainsEdge(int node1, int node2)
        {
            return Edges.ContainsKey(new Vector2I(node1, node2))
                || Edges.ContainsKey(new Vector2I(node2, node1));
        }

        /// <summary>
        /// Returns the edge connected by the nodes.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        /// <exception cref="ArgumentException">Raised if the edge does not exist.</exception>
        public LayoutGraphEdge GetEdge(int node1, int node2)
        {
            if (Edges.TryGetValue(new Vector2I(node1, node2), out var edge1))
                return edge1;
            if (Edges.TryGetValue(new Vector2I(node2, node1), out var edge2))
                return edge2;
            throw new ArgumentException($"Edge does not exist: ({node1}, {node2}).");
        }

        /// <summary>
        /// Adds a new node with the next available ID to the graph.
        /// </summary>
        /// <param name="position">The node position.</param>
        public LayoutGraphNode AddNode(Vector2 position)
        {
            return AddNode(GetNewNodeId(), position);
        }

        /// <summary>
        /// Returns the next available node ID.
        /// </summary>
        private int GetNewNodeId()
        {
            var id = 1;

            while (Nodes.ContainsKey(id))
            {
                id++;
            }

            return id;
        }

        /// <summary>
        /// Adds a new node to the graph.
        /// </summary>
        /// <param name="nodeId">The node ID.</param>
        /// <param name="position">The node position.</param>
        /// <exception cref="ArgumentException">Raised if the node ID already exists.</exception>
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

        /// <summary>
        /// Removes the node and any attached edges from the graph.
        /// Returns true if successful.
        /// </summary>
        /// <param name="nodeId">The node ID.</param>
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

        /// <summary>
        /// Returns all edges attached to the node from the graph.
        /// Returns true if a change is made.
        /// </summary>
        /// <param name="nodeId">The node ID.</param>
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

        /// <summary>
        /// Returns a list of all edge indexes associated with the node.
        /// </summary>
        /// <param name="nodeId">The node ID.</param>
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

        /// <summary>
        /// Adds an edge to the graph.
        /// </summary>
        /// <param name="fromNode">The from node ID.</param>
        /// <param name="toNode">The to node ID.</param>
        /// <exception cref="ArgumentException">Raised if either the from node ID or to node ID does not exist.</exception>
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

        /// <summary>
        /// Removes the edge from the graph.
        /// Returns true if successful.
        /// </summary>
        /// <param name="node1">The first node.</param>
        /// <param name="node2">The second node.</param>
        public bool RemoveEdge(int node1, int node2)
        {
            if (Edges.Remove(new Vector2I(node1, node2)) || Edges.Remove(new Vector2I(node2, node1)))
            {
                SetDirty();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a set of unique template groups contained in the graph.
        /// </summary>
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

        /// <summary>
        /// Returns the ManiaMap layout graph used by the procedural generator.
        /// </summary>
        public LayoutGraph GetMMLayoutGraph()
        {
            var graph = new LayoutGraph(Id, Name);
            AddMMLayoutNodes(graph);
            AddMMLayoutEdges(graph);
            graph.Validate();
            return graph;
        }

        /// <summary>
        /// Adds the ManiaMap layout nodes to the specified graph.
        /// </summary>
        /// <param name="graph">The ManiaMap graph.</param>
        private void AddMMLayoutNodes(LayoutGraph graph)
        {
            foreach (var node in Nodes.Values)
            {
                node.AddMMLayoutNode(graph);
            }
        }

        /// <summary>
        /// Adds the ManiaMap layout edges to the specified graph.
        /// </summary>
        /// <param name="graph">The ManiaMap graph.</param>
        private void AddMMLayoutEdges(LayoutGraph graph)
        {
            foreach (var edge in Edges.Values)
            {
                edge.AddMMLayoutEdge(graph);
            }
        }
    }
}