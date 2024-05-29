using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A feature to associate with a cell.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.Feature3DIcon)]
    public partial class Feature3D : CellChild3D, IFeature
    {
        /// <inheritdoc/>
        [Export] public string FeatureName { get; set; } = "<None>";
    }
}