using Godot;

namespace MPewsey.ManiaMapGodot.Drawing
{
    /// <summary>
    /// A container for a feature name and its tile set atlas coordinate.
    /// </summary>
    [GlobalClass]
    public partial class FeatureAtlasCoordinate : Resource
    {
        /// <summary>
        /// The unique feature name.
        /// </summary>
        [Export] public string FeatureName { get; set; }

        /// <summary>
        /// The feature's tile set atlas coordinate.
        /// </summary>
        [Export] public Vector2I AtlasCoordinate { get; set; }
    }
}