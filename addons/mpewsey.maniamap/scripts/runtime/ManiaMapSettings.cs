using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Contains various runtime settings.
    /// </summary>
    [GlobalClass]
    public partial class ManiaMapSettings : Resource
    {
        /// <summary>
        /// The cell collision mask used for detecting objects entering or exiting a CellArea2D.
        /// This should typically be set to the collision layer of the player.
        /// </summary>
        [Export(PropertyHint.Layers2DPhysics)] public uint CellCollisionMask { get; set; }
    }
}