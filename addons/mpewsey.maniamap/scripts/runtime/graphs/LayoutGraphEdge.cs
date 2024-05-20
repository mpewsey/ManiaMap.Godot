using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Graphs;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Graphs
{
    /// <summary>
    /// An edge of a `LayoutGraphResource`.
    /// </summary>
    [Tool]
    public partial class LayoutGraphEdge : Resource
    {
        private int _fromNode;
        /// <summary>
        /// The from node ID.
        /// </summary>
        [Export] public int FromNode { get => _fromNode; set => SetField(ref _fromNode, value); }

        private int _toNode;
        /// <summary>
        /// The to node ID.
        /// </summary>
        [Export] public int ToNode { get => _toNode; set => SetField(ref _toNode, value); }

        private string _name;
        /// <summary>
        /// The edge and room name.
        /// </summary>
        [Export] public string Name { get => _name; set => SetField(ref _name, value); }

        private EdgeDirection _direction;
        /// <summary>
        /// The edge direction.
        /// </summary>
        [Export] public EdgeDirection Direction { get => _direction; set => SetField(ref _direction, value); }

        private TemplateGroup _templateGroup;
        /// <summary>
        /// The template group.
        /// </summary>
        [Export] public TemplateGroup TemplateGroup { get => _templateGroup; set => SetField(ref _templateGroup, value); }

        private Color _color = new Color(1, 1, 1);
        /// <summary>
        /// The room color.
        /// </summary>
        [Export] public Color Color { get => _color; set => SetField(ref _color, value); }

        private int _z;
        /// <summary>
        /// The room layer coordinate.
        /// </summary>
        [Export] public int Z { get => _z; set => SetField(ref _z, value); }

        private bool _requireRoom;
        /// <summary>
        /// If true, in order to form a valid layout, a room must be added for this edge if the room chance is satisfied.
        /// Otherwise, the room may be skipped if adding an edge for the room fails.
        /// </summary>
        [Export] public bool RequireRoom { get => _requireRoom; set => SetField(ref _requireRoom, value); }

        private float _roomChance;
        /// <summary>
        /// The chance that a room will be created from the edge. The value should be between 0 and 1.
        /// </summary>
        [Export(PropertyHint.Range, "0,1,")] public float RoomChance { get => _roomChance; set => SetField(ref _roomChance, value); }

        private string[] _tags = Array.Empty<string>();
        /// <summary>
        /// An array of room tags.
        /// </summary>
        [Export] public string[] Tags { get => _tags; set => SetField(ref _tags, value); }

        private int _doorCode;
        /// <summary>
        /// The matching door code.
        /// </summary>
        [ExportGroup("Door Code")]
        [Export(PropertyHint.Flags, ManiaMapResources.Enums.DoorCodeFlags)] public int DoorCode { get => _doorCode; set => SetField(ref _doorCode, value); }

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

        /// <summary>
        /// Initializes a new edge.
        /// </summary>
        /// <param name="fromNode">The from node ID.</param>
        /// <param name="toNode">The to node ID.</param>
        public LayoutGraphEdge(int fromNode, int toNode)
        {
            FromNode = fromNode;
            ToNode = toNode;
            Name = $"Edge ({fromNode}, {toNode})";
        }

        /// <summary>
        /// Returns true if the from node or to node matches the specified ID.
        /// </summary>
        /// <param name="nodeId">The node ID.</param>
        public bool ContainsNode(int nodeId)
        {
            return FromNode == nodeId || ToNode == nodeId;
        }

        /// <summary>
        /// Adds the ManiaMap layout edge to the specified layout graph used by the procedural generator.
        /// </summary>
        public void AddMMLayoutEdge(LayoutGraph graph)
        {
            var edge = graph.AddEdge(FromNode, ToNode);
            edge.Name = Name;
            edge.Direction = Direction;
            edge.TemplateGroup = TemplateGroup?.Name;
            edge.Color = ColorUtility.ConvertColorToColor4(Color);
            edge.Z = Z;
            edge.RequireRoom = RequireRoom;
            edge.RoomChance = RoomChance;
            edge.Tags = new List<string>(Tags);
            edge.DoorCode = (DoorCode)DoorCode;
        }
    }
}