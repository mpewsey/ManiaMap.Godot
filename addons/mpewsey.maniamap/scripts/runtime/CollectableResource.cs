using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A reference for a collectable with a unique ID.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class CollectableResource : Resource
    {
        /// <summary>
        /// The unique ID associated with the collectable.
        /// </summary>
        [Export] public int Id { get; set; } = Rand.GetRandomId();
    }
}