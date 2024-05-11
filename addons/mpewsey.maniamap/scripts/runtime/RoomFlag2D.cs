using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomFlag2DIcon)]
    public partial class RoomFlag2D : CellChild2D
    {
        [Export] public int Id { get; set; } = Rand.GetRandomId();

        public override void AutoAssign(RoomNode2D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }

        public bool FlagIsSet()
        {
            return Room.RoomState.Flags.Contains(Id);
        }

        public bool SetFlag()
        {
            return Room.RoomState.Flags.Add(Id);
        }

        public bool RemoveFlag()
        {
            return Room.RoomState.Flags.Remove(Id);
        }

        public bool ToggleFlag()
        {
            if (Room.RoomState.Flags.Add(Id))
                return true;

            Room.RoomState.Flags.Remove(Id);
            return false;
        }
    }
}