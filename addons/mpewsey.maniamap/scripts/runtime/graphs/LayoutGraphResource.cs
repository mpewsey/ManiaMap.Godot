using Godot;
using MPewsey.ManiaMap.Graphs;
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

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.Nodes || name == PropertyName.Edges)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }

        public LayoutGraph CreateGraph()
        {
            return new LayoutGraph(Id, Name);
        }

        public LayoutGraphNode AddNode(Vector2 position)
        {
            var node = new LayoutGraphNode(GetNewNodeId()) { Position = position };
            Nodes.Add(node.Id, node);
            return node;
        }

        public LayoutGraphEdge AddEdge(int fromNode, int toNode)
        {
            if (Nodes.ContainsKey(fromNode))
                throw new Exception($"From node does not exist: {fromNode}");
            if (Nodes.ContainsKey(toNode))
                throw new Exception($"To node does not exist: {toNode}");
            if (EdgeExists(fromNode, toNode))
                throw new Exception($"Edge already exists: ({fromNode}, {toNode})");

            var edge = new LayoutGraphEdge(fromNode, toNode);
            Edges.Add(new Vector2I(fromNode, toNode), edge);
            return edge;
        }

        public bool EdgeExists(int fromNode, int toNode)
        {
            return Edges.ContainsKey(new Vector2I(fromNode, toNode))
                || Edges.ContainsKey(new Vector2I(toNode, fromNode));
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
    }
}