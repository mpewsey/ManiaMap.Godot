using Godot;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphNode : Resource
    {
        [Export] public int Id { get; set; }
        [Export] public string Name { get; set; }
        [Export] public TemplateGroup TemplateGroup { get; set; }
        [Export] public int Z { get; set; }
        [Export] public Color Color { get; set; }
        [Export] public string VariationGroup { get; set; }
        [Export] public string[] Tags { get; set; } = Array.Empty<string>();
        [Export] public Vector2 Position { get; set; }

        public LayoutGraphNode()
        {

        }

        public LayoutGraphNode(int id, Vector2 position)
        {
            Id = id;
            Name = $"Node {id}";
            Position = position;
        }
    }
}