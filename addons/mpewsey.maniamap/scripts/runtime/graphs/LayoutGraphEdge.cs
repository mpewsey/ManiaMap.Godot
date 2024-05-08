using Godot;
using MPewsey.ManiaMap;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphEdge : Resource
    {
        [Export] public int FromNode { get; set; }
        [Export] public int ToNode { get; set; }
        [Export] public string Name { get; set; }
        [Export] public TemplateGroup TemplateGroup { get; set; }
        [Export] public EdgeDirection EdgeDirection { get; set; }
        [Export] public int Z { get; set; }
        [Export] public float RoomChance { get; set; }
        [Export] public bool RequireRoom { get; set; }
        [Export] public Color Color { get; set; }
        [Export] public string[] Tags { get; set; } = Array.Empty<string>();

        [ExportGroup("Code")]
        [Export] public DoorCode Code { get; set; }

        public LayoutGraphEdge()
        {

        }

        public LayoutGraphEdge(int fromNode, int toNode)
        {
            FromNode = fromNode;
            ToNode = toNode;
            Name = $"Edge ({fromNode}, {toNode})";
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.FromNode || name == PropertyName.ToNode)
                property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly);
        }
    }
}