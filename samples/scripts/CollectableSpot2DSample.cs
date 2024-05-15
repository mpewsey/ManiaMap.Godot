using Godot;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class CollectableSpot2DSample : Node
    {
        private CollectableSpot2D CollectableSpot { get; set; }

        public override void _Ready()
        {
            base._Ready();
            CollectableSpot = GetParent<CollectableSpot2D>();
            CollectableSpot.Ready += OnCollectableSpotReady;
        }

        private void OnCollectableSpotReady()
        {
            if (!CollectableSpot.CollectableExists())
                CollectableSpot.QueueFree();
        }
    }
}