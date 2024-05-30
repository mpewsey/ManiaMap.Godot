using Godot;

namespace MPewsey.ManiaMapGodot.Samples
{
    [GlobalClass]
    public partial class DoorNode3DSample : Node
    {
        [Export] public Node3D DoorContainer { get; set; }
        [Export] public Node3D WallContainer { get; set; }
        private DoorNode3D Door { get; set; }

        public override void _Ready()
        {
            base._Ready();
            Door = GetParent<DoorNode3D>();
            Door.Ready += OnDoorReady;
        }

        private void OnDoorReady()
        {
            var doorIsVisible = Door.DoorExists();
            DoorContainer.Visible = doorIsVisible;
            WallContainer.Visible = !doorIsVisible;
        }
    }
}