using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class CollectableGroupEntry : Resource
    {
        [Export] public CollectableResource Collectable { get; set; }
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int Quantity { get; set; }
    }
}
