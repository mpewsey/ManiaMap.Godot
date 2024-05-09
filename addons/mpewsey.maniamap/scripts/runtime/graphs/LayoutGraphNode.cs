using Godot;
using System;

namespace MPewsey.ManiaMapGodot.Graphs
{
    [Tool]
    public partial class LayoutGraphNode : Resource
    {
        private int _id;
        [Export] public int Id { get => _id; set => SetField(ref _id, value); }

        private string _name;
        [Export] public string Name { get => _name; set => SetField(ref _name, value); }

        private string _variationGroup;
        [Export] public string VariationGroup { get => _variationGroup; set => SetField(ref _variationGroup, value); }

        private TemplateGroup _templateGroup;
        [Export] public TemplateGroup TemplateGroup { get => _templateGroup; set => SetField(ref _templateGroup, value); }

        private Color _color = new Color(1, 1, 1);
        [Export] public Color Color { get => _color; set => SetField(ref _color, value); }

        private int _z;
        [Export] public int Z { get => _z; set => SetField(ref _z, value); }

        private string[] _tags = Array.Empty<string>();
        [Export] public string[] Tags { get => _tags; set => SetField(ref _tags, value); }

        private Vector2 _position;
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

        public LayoutGraphNode(int id, Vector2 position)
        {
            Id = id;
            Name = $"Node {id}";
            Position = position;
        }
    }
}