using Godot;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class ManiaMapSettings : Resource
    {
        [Export] public bool AssignLayoutPosition { get; set; }
        [Export(PropertyHint.Layers2DPhysics)] public uint CellCollisionMask { get; set; }
    }
}