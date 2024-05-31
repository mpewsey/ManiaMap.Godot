using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A possible collectable location within a RoomNode3D.
    /// 
    /// See ICollectableSpotExtensions for additional methods usable by this class.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.CollectableSpot3Dicon)]
    public partial class CollectableSpot3D : CellChild3D, ICollectableSpot
    {
        private bool _editId;
        /// <summary>
        /// If true, the Id property becomes editable in the inspector.
        /// </summary>
        [Export] public bool EditId { get => _editId; set => SetValidatedField(ref _editId, value); }

        /// <inheritdoc/>
        [Export] public int Id { get; set; } = -1;

        /// <inheritdoc/>
        [Export] public CollectableGroup CollectableGroup { get; set; }

        /// <inheritdoc/>
        [Export(PropertyHint.Range, "0,2,0.1,or_greater")] public float Weight { get; set; } = 1;

        private void SetValidatedField<T>(ref T field, T value)
        {
            field = value;
            NotifyPropertyListChanged();
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (name == PropertyName.Id)
            {
                var flag = EditId ? PropertyUsageFlags.None : PropertyUsageFlags.ReadOnly;
                property["usage"] = (int)(PropertyUsageFlags.Default | flag);
            }
        }

        /// <inheritdoc/>
        public override void AutoAssign(RoomNode3D room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }
    }
}