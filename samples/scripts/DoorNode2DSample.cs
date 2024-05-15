using Godot;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class DoorNode2DSample : Node
    {
        private DoorNode2D Door { get; set; }

        public override void _Ready()
        {
            base._Ready();
            Door = GetParent<DoorNode2D>();
            Door.Ready += OnDoorReady;
        }

        private void OnDoorReady()
        {
            var tileMap = (TileMap)Door.FindChild(nameof(TileMap));
            var layerId = Door.DoorExists() ? 0 : 1;
            tileMap.SetLayerEnabled(layerId, false);
        }
    }
}