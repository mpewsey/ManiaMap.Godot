using Godot;
using MPewsey.ManiaMap;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphEdgeResource : Resource
    {
        [Export] public int FromNode { get; set; }
        [Export] public int ToNode { get; set; }
        [Export] public string EdgeName { get; set; }
        [Export] public TemplateGroup TemplateGroup { get; set; }
        [Export] public EdgeDirection EdgeDirection { get; set; }
        [Export] public DoorCode Code { get; set; }
        [Export] public int Z { get; set; }
        [Export] public float RoomChance { get; set; }
        [Export] public bool RequireRoom { get; set; }
        [Export] public Color Color { get; set; }
        [Export] public string[] Tags { get; set; } = Array.Empty<string>();
    }
}