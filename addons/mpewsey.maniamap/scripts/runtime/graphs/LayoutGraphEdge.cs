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

        private TemplateGroup _templateGroup;
        [Export] public TemplateGroup TemplateGroup { get => _templateGroup; set => SetField(ref _templateGroup, value); }

        private EdgeDirection _edgeDirection;
        [Export] public EdgeDirection EdgeDirection { get => _edgeDirection; set => SetField(ref _edgeDirection, value); }

        private Color _color;
        [Export] public Color Color { get => _color; set => SetField(ref _color, value); }

        private int _z;
        [Export] public int Z { get => _z; set => SetField(ref _z, value); }

        private bool _requireRoom;
        [Export] public bool RequireRoom { get => _requireRoom; set => SetField(ref _requireRoom, value); }

        private float _roomChance;
        [Export] public float RoomChance { get => _roomChance; set => SetField(ref _roomChance, value); }

        private string[] _tags = Array.Empty<string>();
        [Export] public string[] Tags { get => _tags; set => SetField(ref _tags, value); }

        private int _code;
        [ExportGroup("Code")]
        [Export(PropertyHint.Flags, ManiaMapResources.DoorCodeFlags)] public int Code { get => _code; set => SetField(ref _code, value); }

        private void SetField<T>(ref T field, T value)
        {
            field = value;
            EmitChanged();
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
    }
}