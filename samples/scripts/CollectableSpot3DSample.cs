using Godot;

namespace MPewsey.ManiaMapGodot
{
    [GlobalClass]
    public partial class CollectableSpot3DSample : Node
    {
        private CollectableSpot3D CollectableSpot { get; set; }

        public override void _Ready()
        {
            base._Ready();
            CollectableSpot = GetParent<CollectableSpot3D>();
            CollectableSpot.Ready += OnCollectableSpotReady;
        }

        private void OnCollectableSpotReady()
        {
            if (!CollectableSpot.CollectableExists())
                CollectableSpot.QueueFree();
        }
    }
}