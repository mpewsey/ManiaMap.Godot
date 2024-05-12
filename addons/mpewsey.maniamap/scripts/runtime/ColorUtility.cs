using Godot;
using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    public static class ColorUtility
    {
        public static Color4 ConvertColorToColor4(Color color)
        {
            return new Color4((byte)color.R8, (byte)color.G8, (byte)color.B8, (byte)color.A8);
        }

        public static Color ConvertColor4ToColor(Color4 color)
        {
            return Color.Color8(color.R, color.G, color.B, color.A);
        }
    }

}