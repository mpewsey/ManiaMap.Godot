using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.CollectableSpot2Dicon)]
    public partial class CollectableSpot2D : CellChild2D
    {
        [Export] public int Id { get; set; } = Rand.GetRandomId();
        [Export] public CollectableGroup CollectableGroup { get; set; }
        [Export(PropertyHint.Range, "0,2,0.1,or_greater")] public float Weight { get; set; } = 1;

        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }

        public int CollectableId()
        {
            if (Room.RoomLayout.Collectables.TryGetValue(Id, out var collectableId))
                return collectableId;
            return int.MinValue;
        }

        public bool CollectableExists()
        {
            return Room.RoomLayout.Collectables.ContainsKey(Id);
        }

        public bool IsAcquired()
        {
            return Room.RoomState.AcquiredCollectables.Contains(Id);
        }

        public bool CanAcquire()
        {
            return CollectableExists() && !IsAcquired();
        }

        public bool Acquire()
        {
            if (CanAcquire())
                return Room.RoomState.AcquiredCollectables.Add(Id);
            return false;
        }
    }
}