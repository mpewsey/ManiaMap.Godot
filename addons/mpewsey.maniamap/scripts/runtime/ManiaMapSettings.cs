using Godot;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class ManiaMapSettings : Resource
    {
        [Export(PropertyHint.Layers2DPhysics)] public uint CellCollisionMask { get; set; }
    }
}