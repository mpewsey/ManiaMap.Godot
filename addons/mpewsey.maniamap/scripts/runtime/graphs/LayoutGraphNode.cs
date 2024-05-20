using Godot;
using MPewsey.ManiaMap.Graphs;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Graphs
{
    /// <summary>
    /// A `LayoutGraphResource` node.
    /// </summary>
    [Tool]
    public partial class LayoutGraphNode : Resource
    {
        private int _id;
        /// <summary>
        /// The unique node ID.
        /// </summary>
        [Export] public int Id { get => _id; set => SetField(ref _id, value); }

        private string _name;
        /// <summary>
        /// The node and room name.
        /// </summary>
        [Export] public string Name { get => _name; set => SetField(ref _name, value); }

        private string _variationGroup;
        /// <summary>
        /// The variation group to which this node belongs. Leave empty if none.
        /// 
        /// Nodes belonging to the same group are randomly swapped by the LayoutGraphRandomizerStep.
        /// </summary>
        [Export] public string VariationGroup { get => _variationGroup; set => SetField(ref _variationGroup, value); }

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
        /// The layer coordinate.
        /// </summary>
        [Export] public int Z { get => _z; set => SetField(ref _z, value); }

        private string[] _tags = Array.Empty<string>();
        /// <summary>
        /// An array of tags.
        /// </summary>
        [Export] public string[] Tags { get => _tags; set => SetField(ref _tags, value); }

        private Vector2 _position;
        /// <summary>
        /// The node position within the graph.
        /// </summary>
        [Export] public Vector2 Position { get => _position; set => SetField(ref _position, value); }

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

            if (name == PropertyName.Id)
                property["usage"] = (int)(usage | PropertyUsageFlags.ReadOnly);
            else if (name == PropertyName.Position)
                property["usage"] = (int)(usage & ~PropertyUsageFlags.Editor);
        }

        public LayoutGraphNode()
        {

        }

        /// <summary>
        /// Initializes a new node.
        /// </summary>
        /// <param name="id">The node ID.</param>
        /// <param name="position">The position within the graph.</param>
        public LayoutGraphNode(int id, Vector2 position)
        {
            Id = id;
            Name = $"Node {id}";
            Position = position;
        }

        /// <summary>
        /// Adds the ManiaMap layout node to the specified layout graph used by the procedural generator.
        /// </summary>
        public void AddMMLayoutNode(LayoutGraph graph)
        {
            var node = graph.AddNode(Id);
            node.Name = Name;
            node.Color = ColorUtility.ConvertColorToColor4(Color);
            node.TemplateGroup = TemplateGroup.Name;
            node.Z = Z;
            node.Tags = new List<string>(Tags);

            if (!string.IsNullOrWhiteSpace(VariationGroup))
                graph.AddNodeVariation(VariationGroup, node.Id);
        }
    }
}