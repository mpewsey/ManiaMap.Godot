using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A feature to associate with a cell.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.Feature2DIcon)]
    public partial class Feature2D : CellChild2D
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        [Export] public string FeatureName { get; set; } = "<None>";
    }
}