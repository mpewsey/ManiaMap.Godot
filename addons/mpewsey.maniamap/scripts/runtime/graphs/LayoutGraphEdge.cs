using Godot;
using MPewsey.ManiaMap;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphEdge : Resource
    {
        private int _fromNode;
        [Export] public int FromNode { get => _fromNode; set => SetField(ref _fromNode, value); }

        private int _toNode;
        [Export] public int ToNode { get => _toNode; set => SetField(ref _toNode, value); }

        private string _name;
        [Export] public string Name { get => _name; set => SetField(ref _name, value); }

        private EdgeDirection _direction;
        [Export] public EdgeDirection Direction { get => _direction; set => SetField(ref _direction, value); }

        private TemplateGroup _templateGroup;
        [Export] public TemplateGroup TemplateGroup { get => _templateGroup; set => SetField(ref _templateGroup, value); }

        private Color _color = new Color(1, 1, 1);
        [Export] public Color Color { get => _color; set => SetField(ref _color, value); }

        private int _z;
        [Export] public int Z { get => _z; set => SetField(ref _z, value); }

        private bool _requireRoom;
        [Export] public bool RequireRoom { get => _requireRoom; set => SetField(ref _requireRoom, value); }

        private float _roomChance;
        [Export(PropertyHint.Range, "0,1,")] public float RoomChance { get => _roomChance; set => SetField(ref _roomChance, value); }

        private string[] _tags = Array.Empty<string>();
        [Export] public string[] Tags { get => _tags; set => SetField(ref _tags, value); }

        private int _doorCode;
        [ExportGroup("Door Code")]
        [Export(PropertyHint.Flags, ManiaMapResources.DoorCodeFlags)] public int DoorCode { get => _doorCode; set => SetField(ref _doorCode, value); }

        private void SetField<T>(ref T field, T value)
        {
            field = value;
            EmitChanged();
        }

        public override void _ValidateProperty(Godot.Collections.Dictionary property)
        {
            base._ValidateProperty(property);
            var name = property["name"].AsStringName();
            var usage = property["usage"].As<PropertyUsageFlags>();

            if (name == PropertyName.FromNode || name == PropertyName.ToNode)
                property["usage"] = (int)(usage | PropertyUsageFlags.ReadOnly);
        }

        public LayoutGraphEdge()
        {

        }

        public LayoutGraphEdge(int fromNode, int toNode)
        {
            FromNode = fromNode;
            ToNode = toNode;
            Name = $"Edge ({fromNode}, {toNode})";
        }

        public bool ContainsNode(int nodeId)
        {
            return FromNode == nodeId || ToNode == nodeId;
        }
    }
}