using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class CollectableResource : Resource
    {
        [Export] public int Id { get; set; } = ManiaMapManager.GetRandomId();
    }
}