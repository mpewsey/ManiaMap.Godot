using Godot;

namespace MPewsey.ManiaMapGodot.Drawing
{
    [GlobalClass]
    public partial class FeatureAtlasCoordinate : Resource
    {
        [Export] public string FeatureName { get; set; }
        [Export] public Vector2I AtlasCoordinate { get; set; }
    }
}