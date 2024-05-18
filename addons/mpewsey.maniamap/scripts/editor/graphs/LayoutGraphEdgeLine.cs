#if TOOLS
using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Graphs.Editor
{
    [Tool]
    public partial class LayoutGraphEdgeLine : Node2D
    {
        [Export] public Line2D Line { get; set; }
        [Export] public TextureRect Quarter1Sprite { get; set; }
        [Export] public TextureRect Quarter3Sprite { get; set; }
        [Export] public Texture2D FixedArrowTexture { get; set; }
        [Export] public Texture2D FlexibleArrowTexture { get; set; }
        private float LineWidth { get; set; }

        public override void _Ready()
        {
            base._Ready();
            LineWidth = Line.Width;

            if (Line.Points.Length != 2)
                Line.Points = new Vector2[2];
        }

        public void Populate(LayoutGraphEdgeElement edgeElement, Dictionary<int, LayoutGraphNodeElement> nodeElements, float zoom)
        {
            var edge = edgeElement.EdgeResource;
            var fromFound = nodeElements.TryGetValue(edge.FromNode, out var fromNode);
            var toFound = nodeElements.TryGetValue(edge.ToNode, out var toNode);

            if (!fromFound || !toFound)
            {
                Visible = false;
                return;
            }

            Visible = true;
            var fromPosition = fromNode.Position + 0.5f * zoom * fromNode.Size;
            var toPosition = toNode.Position + 0.5f * zoom * toNode.Size;

            Line.Width = Mathf.Max(LineWidth * zoom, 1);
            Line.DefaultColor = edge.Color;
            Line.SetPointPosition(0, fromPosition);
            Line.SetPointPosition(1, toPosition);

            var delta = toPosition - fromPosition;
            var angle = Mathf.Atan2(delta.Y, delta.X) + EdgeDirectionAngleOffset(edge.Direction);

            Quarter1Sprite.Rotation = Quarter3Sprite.Rotation = angle;
            Quarter1Sprite.SelfModulate = Quarter3Sprite.SelfModulate = edge.Color;
            Quarter1Sprite.Texture = Quarter3Sprite.Texture = GetQuarterSpriteIcon(edge.Direction);
            Quarter1Sprite.Scale = Quarter3Sprite.Scale = new Vector2(zoom, zoom);

            Quarter1Sprite.Position = fromPosition.Lerp(toPosition, 0.25f) - 0.5f * Quarter1Sprite.Size;
            Quarter3Sprite.Position = fromPosition.Lerp(toPosition, 0.75f) - 0.5f * Quarter3Sprite.Size;
        }

        private static float EdgeDirectionAngleOffset(EdgeDirection direction)
        {
            if (direction == EdgeDirection.ReverseFixed || direction == EdgeDirection.ReverseFlexible)
                return Mathf.Pi;
            return 0;
        }

        private Texture2D GetQuarterSpriteIcon(EdgeDirection direction)
        {
            switch (direction)
            {
                case EdgeDirection.Both:
                    return null;
                case EdgeDirection.ForwardFlexible:
                case EdgeDirection.ReverseFlexible:
                    return FlexibleArrowTexture;
                case EdgeDirection.ForwardFixed:
                case EdgeDirection.ReverseFixed:
                    return FixedArrowTexture;
                default:
                    throw new NotImplementedException($"Unhandled edge direction: {direction}.");
            }
        }
    }
}
#endif