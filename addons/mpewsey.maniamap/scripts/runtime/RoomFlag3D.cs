using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A room flag that can be set or toggled to alter the `LayoutState`.
    /// 
    /// See IRoomFlagExtensions for additional methods usable by this class.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.RoomFlag3DIcon)]
    public partial class RoomFlag3D : CellChild3D, IRoomFlag
    {
        private bool _editId;
        /// <summary>
        /// If true, the Id property becomes editable in the inspector.
        /// </summary>
        [Export] public bool EditId { get => _editId; set => SetValidatedField(ref _editId, value); }

        /// <inheritdoc/>
        [Export] public int Id { get; set; } = -1;

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