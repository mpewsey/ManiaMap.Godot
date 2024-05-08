using Godot;
using System;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class TemplateGroup : Resource
    {
        [Export] public string Name { get; set; } = "<None>";
        [Export] public TemplateGroupEntry[] Entries { get; set; } = Array.Empty<TemplateGroupEntry>();
    }
}