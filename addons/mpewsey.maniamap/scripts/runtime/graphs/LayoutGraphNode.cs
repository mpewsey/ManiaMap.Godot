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
        [Export] public Vector2 Position { get; set; }
        [Export] public string[] Tags { get; set; } = Array.Empty<string>();

        public LayoutGraphNode()
        {

        }

        public LayoutGraphNode(int id)
        {
            Id = id;
            Name = $"Node {id}";
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.Id || name == PropertyName.Position)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }
    }
}