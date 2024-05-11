using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.Feature2DIcon)]
    public partial class Feature2D : CellChild2D
    {
        [Export] public string FeatureName { get; set; } = "<None>";
    }
}