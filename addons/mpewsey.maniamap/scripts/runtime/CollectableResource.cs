using Godot;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A reference for a collectable with a unique ID.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class CollectableResource : Resource
    {
        private bool _editId;
        /// <summary>
        /// If true, the Id property becomes editable in the inspector.
        /// </summary>
        [Export] public bool EditId { get => _editId; set => SetValidatedField(ref _editId, value); }

        /// <summary>
        /// The unique ID associated with the collectable.
        /// </summary>
        [Export] public int Id { get; set; } = Rand.GetRandomId();

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
    }
}