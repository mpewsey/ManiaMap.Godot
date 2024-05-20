using Godot;
using MPewsey.ManiaMap;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Contains methods for converting colors.
    /// </summary>
    public static class ColorUtility
    {
        /// <summary>
        /// Converts a Godot Color to a ManiaMap Color4.
        /// </summary>
        /// <param name="color">The Godot color.</param>
        public static Color4 ConvertColorToColor4(Color color)
        {
            return new Color4((byte)color.R8, (byte)color.G8, (byte)color.B8, (byte)color.A8);
        }

        /// <summary>
        /// Converts a ManiaMap Color4 to a Godot Color.
        /// </summary>
        /// <param name="color">The ManiaMap color.</param>
        public static Color ConvertColor4ToColor(Color4 color)
        {
            return Color.Color8(color.R, color.G, color.B, color.A);
        }
    }

}