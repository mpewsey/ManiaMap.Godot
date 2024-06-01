#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    public static class ManiaMapProjectSettings
    {
        // Batch Update
        private const string BatchUpdateSearchPathSetting = "mania_map/settings/batch_update/search_path";
        private const string BatchUpdateSearchPathDefaultValue = "res://";

        // Room 2D
        private const string Room2DActiveCellColorSetting = "mania_map/2d_gizmos/room_2d/active_cell_color";
        private static Color Room2DActiveCellColorDefaultValue { get; } = new Color(0, 0, 1, 0.1f);

        private const string Room2DInactiveCellColorSetting = "mania_map/2d_gizmos/room_2d/inactive_cell_color";
        private static Color Room2DInactiveCellColorDefaultValue { get; } = new Color(1, 0, 0, 0.1f);

        private const string Room2DCellLineColorSetting = "mania_map/2d_gizmos/room_2d/cell_line_color";
        private static Color Room2DCellLineColorDefaultValue { get; } = new Color(0.5f, 0.5f, 0.5f);

        // Room 3D
        private const string Room3DActiveCellColorSetting = "mania_map/3d_gizmos/room_3d/active_cell_color";
        private static Color Room3DActiveCellColorDefaultValue { get; } = new Color(0, 0, 1, 0.1f);

        private const string Room3DInactiveCellColorSetting = "mania_map/3d_gizmos/room_3d/inactive_cell_color";
        private static Color Room3DInactiveCellColorDefaultValue { get; } = new Color(1, 0, 0, 0.1f);

        private const string Room3DCellLineColorSetting = "mania_map/3d_gizmos/room_3d/cell_line_color";
        private static Color Room3DCellLineColorDefaultValue { get; } = new Color(0, 0, 0);

        // Door Threshold 2D
        private const string DoorThreshold2DFillColorSetting = "mania_map/2d_gizmos/door_threshold_2d/fill_color";
        private static Color DoorThreshold2DFillColorDefaultValue { get; } = new Color(1, 1, 0, 0.1f);

        private const string DoorThreshold2DLineColorSetting = "mania_map/2d_gizmos/door_threshold_2d/line_color";
        private static Color DoorThreshold2DLineColorDefaultValue { get; } = new Color(1, 1, 0);

        private static Godot.Collections.Dictionary[] GetPropertyInfos()
        {
            return new Godot.Collections.Dictionary[]
            {
                // Batch Update
                new Godot.Collections.Dictionary()
                {
                    { "name",  BatchUpdateSearchPathSetting },
                    { "type", (int)Variant.Type.String },
                    { "hint", (int)PropertyHint.Dir },
                    { "hint_string", string.Empty },
                    { "default_value", BatchUpdateSearchPathDefaultValue },
                },
                // Room 2D
                new Godot.Collections.Dictionary()
                {
                    { "name",  Room2DActiveCellColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", Room2DActiveCellColorDefaultValue },
                },
                new Godot.Collections.Dictionary()
                {
                    { "name",  Room2DInactiveCellColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", Room2DInactiveCellColorDefaultValue },
                },
                new Godot.Collections.Dictionary()
                {
                    { "name",  Room2DCellLineColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", Room2DCellLineColorDefaultValue },
                },
                // Room 3D
                new Godot.Collections.Dictionary()
                {
                    { "name",  Room3DActiveCellColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", Room3DActiveCellColorDefaultValue },
                },
                new Godot.Collections.Dictionary()
                {
                    { "name",  Room3DInactiveCellColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", Room3DInactiveCellColorDefaultValue },
                },
                new Godot.Collections.Dictionary()
                {
                    { "name",  Room3DCellLineColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", Room3DCellLineColorDefaultValue },
                },
                // Door Threshold 2D
                new Godot.Collections.Dictionary()
                {
                    { "name",  DoorThreshold2DFillColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", DoorThreshold2DFillColorDefaultValue },
                },
                new Godot.Collections.Dictionary()
                {
                    { "name",  DoorThreshold2DLineColorSetting },
                    { "type", (int)Variant.Type.Color },
                    { "hint", (int)PropertyHint.None },
                    { "hint_string", string.Empty },
                    { "default_value", DoorThreshold2DLineColorDefaultValue },
                },
            };
        }

        public static string GetBatchUpdateSearchPath()
        {
            return ProjectSettings.GetSetting(BatchUpdateSearchPathSetting, BatchUpdateSearchPathDefaultValue).AsString();
        }

        public static Color GetRoom2DActiveCellColor()
        {
            return ProjectSettings.GetSetting(Room2DActiveCellColorSetting, Room2DActiveCellColorDefaultValue).AsColor();
        }

        public static Color GetRoom2DInactiveCellColor()
        {
            return ProjectSettings.GetSetting(Room2DInactiveCellColorSetting, Room2DInactiveCellColorDefaultValue).AsColor();
        }

        public static Color GetRoom2DCellLineColor()
        {
            return ProjectSettings.GetSetting(Room2DCellLineColorSetting, Room2DCellLineColorDefaultValue).AsColor();
        }

        public static Color GetRoom3DActiveCellColor()
        {
            return ProjectSettings.GetSetting(Room3DActiveCellColorSetting, Room3DActiveCellColorDefaultValue).AsColor();
        }

        public static Color GetRoom3DInactiveCellColor()
        {
            return ProjectSettings.GetSetting(Room3DInactiveCellColorSetting, Room3DInactiveCellColorDefaultValue).AsColor();
        }

        public static Color GetRoom3DCellLineColor()
        {
            return ProjectSettings.GetSetting(Room3DCellLineColorSetting, Room3DCellLineColorDefaultValue).AsColor();
        }

        public static Color GetDoorThreshold2DFillColor()
        {
            return ProjectSettings.GetSetting(DoorThreshold2DFillColorSetting, DoorThreshold2DFillColorDefaultValue).AsColor();
        }

        public static Color GetDoorThreshold2DLineColor()
        {
            return ProjectSettings.GetSetting(DoorThreshold2DLineColorSetting, DoorThreshold2DLineColorDefaultValue).AsColor();
        }

        public static void CreateProjectSettings()
        {
            foreach (var propertyInfo in GetPropertyInfos())
            {
                var name = propertyInfo["name"].AsString();
                var defaultValue = propertyInfo["default_value"];

                if (!ProjectSettings.HasSetting(name))
                    ProjectSettings.SetSetting(name, defaultValue);

                ProjectSettings.SetInitialValue(name, defaultValue);
                ProjectSettings.AddPropertyInfo(propertyInfo);
            }
        }
    }
}
#endif
