using Godot;

namespace MPewsey.ManiaMapGodot
{
    [Tool]
    [GlobalClass]
    public partial class TemplateGroupEntry : Resource
    {
        [Export] public RoomTemplateResource RoomTemplate { get; set; }
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int MinQuantity { get; set; }
        [Export(PropertyHint.Range, "0,100,1,or_greater")] public int MaxQuantity { get; set; } = int.MaxValue;
    }
}