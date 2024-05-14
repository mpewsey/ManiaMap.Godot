#if TOOLS
using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot.Graphs.Editor
{
    public partial class LayoutGraphMultiEditor : Resource
    {
        private bool _editName;
        [Export] public bool EditName { get => _editName; set => SetEditField(ref _editName, value); }

        private string _name;
        [Export] public string Name { get => _name; set => SetElementField(ref _name, value, PropertyName.Name); }

        private bool _editVariationGroup;
        [Export] public bool EditVariationGroup { get => _editVariationGroup; set => SetEditField(ref _editVariationGroup, value); }

        private string _variationGroup;
        [Export] public string VariationGroup { get => _variationGroup; set => SetElementField(ref _variationGroup, value, PropertyName.VariationGroup); }

        private bool _editEdgeDirection;
        [Export] public bool EditEdgeDirection { get => _editEdgeDirection; set => SetEditField(ref _editEdgeDirection, value); }

        private EdgeDirection _edgeDirection;
        [Export] public EdgeDirection EdgeDirection { get => _edgeDirection; set => SetElementField(ref _edgeDirection, value, PropertyName.EdgeDirection); }

        private bool _editTempateGroup;
        [Export] public bool EditTempateGroup { get => _editTempateGroup; set => SetEditField(ref _editTempateGroup, value); }

        private TemplateGroup _templateGroup;
        [Export] public TemplateGroup TemplateGroup { get => _templateGroup; set => SetElementField(ref _templateGroup, value, PropertyName.TemplateGroup); }

        private bool _editColor;
        [Export] public bool EditColor { get => _editColor; set => SetEditField(ref _editColor, value); }

        private Color _color = new Color(1, 1, 1);
        [Export] public Color Color { get => _color; set => SetElementField(ref _color, value, PropertyName.Color); }

        private bool _editZ;
        [Export] public bool EditZ { get => _editZ; set => SetEditField(ref _editZ, value); }

        private int _z;
        [Export] public int Z { get => _z; set => SetElementField(ref _z, value, PropertyName.Z); }

        private bool _editRequireRoom;
        [Export] public bool EditRequireRoom { get => _editRequireRoom; set => SetEditField(ref _editRequireRoom, value); }

        private bool _requireRoom;
        [Export] public bool RequireRoom { get => _requireRoom; set => SetElementField(ref _requireRoom, value, PropertyName.RequireRoom); }

        private bool _editRoomChance;
        [Export] public bool EditRoomChance { get => _editRoomChance; set => SetEditField(ref _editRoomChance, value); }

        private float _roomChance;
        [Export] public float RoomChance { get => _roomChance; set => SetElementField(ref _roomChance, value, PropertyName.RoomChance); }

        private bool _editDoorCode;
        [Export] public bool EditDoorCode { get => _editDoorCode; set => SetEditField(ref _editDoorCode, value); }

        private int _doorCode;
        [Export(PropertyHint.Flags, ManiaMapResources.Enums.DoorCodeFlags)] public int DoorCode { get => _doorCode; set => SetElementField(ref _doorCode, value, PropertyName.DoorCode); }

        private Resource[] Targets { get; } = Array.Empty<Resource>();
        private LayoutGraphElementFlags TargetFlags { get; }

        public LayoutGraphMultiEditor()
        {

        }

        public LayoutGraphMultiEditor(IEnumerable<Resource> targets)
        {
            Targets = targets.ToArray();
            TargetFlags = GetTargetFlags(Targets);
        }

        private void SetEditField<T>(ref T field, T value)
        {
            field = value;
            NotifyPropertyListChanged();
        }

        private void SetElementField<[MustBeVariant] T>(ref T field, T value, StringName name)
        {
            field = value;
            SetTargetFields(name, Variant.From(value));
        }

        private void SetTargetFields(StringName name, Variant value)
        {
            if (IsNodeOnlyProperty(name))
            {
                foreach (var target in Targets)
                {
                    if (target is LayoutGraphNode)
                        target.Set(name, value);
                }
            }
            else if (IsEdgeOnlyProperty(name))
            {
                foreach (var target in Targets)
                {
                    if (target is LayoutGraphEdge)
                        target.Set(name, value);
                }
            }
            else
            {
                foreach (var target in Targets)
                {
                    target.Set(name, value);
                }
            }
        }

        private static LayoutGraphElementFlags GetTargetFlags(Resource[] targets)
        {
            LayoutGraphElementFlags flags = 0;

            foreach (var target in targets)
            {
                switch (target)
                {
                    case LayoutGraphNode:
                        flags |= LayoutGraphElementFlags.Node;
                        break;
                    case LayoutGraphEdge:
                        flags |= LayoutGraphElementFlags.Edge;
                        break;
                    default:
                        throw new NotImplementedException($"Unhandled target type: {target.GetType()}");
                }

                if (flags == LayoutGraphElementFlags.All)
                    break;
            }

            return flags;
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();

            if (IsNodeOnlyEditProperty(name))
                property["usage"] = (int)GetEditPropertyUsageFlags(LayoutGraphElementFlags.Node);
            else if (IsEdgeOnlyEditProperty(name))
                property["usage"] = (int)GetEditPropertyUsageFlags(LayoutGraphElementFlags.Edge);
            else if (name == PropertyName.Name)
                property["usage"] = (int)GetPropertyUsageFlags(EditName, LayoutGraphElementFlags.All);
            else if (name == PropertyName.VariationGroup)
                property["usage"] = (int)GetPropertyUsageFlags(EditVariationGroup, LayoutGraphElementFlags.Node);
            else if (name == PropertyName.EdgeDirection)
                property["usage"] = (int)GetPropertyUsageFlags(EditEdgeDirection, LayoutGraphElementFlags.Edge);
            else if (name == PropertyName.TemplateGroup)
                property["usage"] = (int)GetPropertyUsageFlags(EditTempateGroup, LayoutGraphElementFlags.All);
            else if (name == PropertyName.Color)
                property["usage"] = (int)GetPropertyUsageFlags(EditColor, LayoutGraphElementFlags.All);
            else if (name == PropertyName.Z)
                property["usage"] = (int)GetPropertyUsageFlags(EditZ, LayoutGraphElementFlags.All);
            else if (name == PropertyName.RequireRoom)
                property["usage"] = (int)GetPropertyUsageFlags(EditRequireRoom, LayoutGraphElementFlags.Edge);
            else if (name == PropertyName.RoomChance)
                property["usage"] = (int)GetPropertyUsageFlags(EditRoomChance, LayoutGraphElementFlags.Edge);
            else if (name == PropertyName.DoorCode)
                property["usage"] = (int)GetPropertyUsageFlags(EditDoorCode, LayoutGraphElementFlags.Edge);
        }

        private static bool IsNodeOnlyProperty(StringName name)
        {
            return name == PropertyName.VariationGroup;
        }

        private static bool IsEdgeOnlyProperty(StringName name)
        {
            return name == PropertyName.EdgeDirection
                || name == PropertyName.RequireRoom
                || name == PropertyName.RoomChance
                || name == PropertyName.DoorCode;
        }

        private static bool IsNodeOnlyEditProperty(StringName name)
        {
            return name == PropertyName.EditVariationGroup;
        }

        private static bool IsEdgeOnlyEditProperty(StringName name)
        {
            return name == PropertyName.EditEdgeDirection
                || name == PropertyName.EditRequireRoom
                || name == PropertyName.EditRoomChance
                || name == PropertyName.EditDoorCode;
        }

        private bool PropertyIsVisible(LayoutGraphElementFlags flags)
        {
            return (TargetFlags & flags) != 0
                && TargetFlags != LayoutGraphElementFlags.All;
        }

        private PropertyUsageFlags GetEditPropertyUsageFlags(LayoutGraphElementFlags flags)
        {
            if (PropertyIsVisible(flags))
                return PropertyUsageFlags.Default;

            return PropertyUsageFlags.Default & ~PropertyUsageFlags.Editor;
        }

        private PropertyUsageFlags GetPropertyUsageFlags(bool isEdited, LayoutGraphElementFlags flags)
        {
            if (isEdited && PropertyIsVisible(flags))
                return PropertyUsageFlags.Default;

            return PropertyUsageFlags.Default & ~PropertyUsageFlags.Editor;
        }
    }
}
#endif