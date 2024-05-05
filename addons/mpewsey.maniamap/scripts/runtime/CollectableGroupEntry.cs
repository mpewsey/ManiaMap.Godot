using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class CollectableGroupEntry : Resource
    {
        [Export] public CollectableResource Collectable { get; set; }
        [Export] public int Quantity { get; set; }
    }
}
