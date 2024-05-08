using Godot;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphNodeResource : Resource
    {
        [Export] public int Id { get; set; }
        [Export] public string NodeName { get; set; }
        [Export] public TemplateGroup TemplateGroup { get; set; }
        [Export] public int Z { get; set; }
        [Export] public Color Color { get; set; }
        [Export] public string VariationGroup { get; set; }
        [Export] public string[] Tags { get; set; } = Array.Empty<string>();
    }
}