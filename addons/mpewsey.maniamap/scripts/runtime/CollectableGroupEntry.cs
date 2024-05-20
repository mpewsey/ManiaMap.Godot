using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// An entry in a CollectableGroup.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class CollectableGroupEntry : Resource
    {
        /// <summary>
        /// The referenced collectable.
        /// </summary>
        [Export] public CollectableResource Collectable { get; set; }

        /// <summary>
        /// The quantity of collectables for distribution throughout a `Layout`.
        /// </summary>
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int Quantity { get; set; }
    }
}
