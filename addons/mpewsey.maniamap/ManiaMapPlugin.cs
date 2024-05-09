#if TOOLS
using Godot;
using System;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class ManiaMapPlugin : EditorPlugin
    {
        private const string MenuName = "Mania Map";
        private const string BatchUpdateSearchPathSetting = "mania_map/settings/batch_update_search_path";

        public override void _EnterTree()
        {
            CreateProjectSettings();
            AddToolMenu();
        }

        public override void _ExitTree()
        {
            RemoveToolMenuItem(MenuName);
        }

        private static void CreateProjectSettings()
        {
            CreateSetting(BatchUpdateSearchPathSetting, "res://");
        }

        private static void CreateSetting(string name, Variant defaultValue)
        {
            if (!ProjectSettings.HasSetting(name))
                ProjectSettings.SetSetting(name, defaultValue);
            ProjectSettings.SetInitialValue(name, defaultValue);
        }

        private void AddToolMenu()
        {
            var menu = new PopupMenu();
            menu.IndexPressed += OnMenuIndexPressed;
            menu.AddItem("Batch Update Room Templates");
            AddToolSubmenuItem(MenuName, menu);
        }

        private void OnMenuIndexPressed(long index)
        {
            switch (index)
            {
                case 0:
                    BatchUpdateRoomTemplates();
                    break;
                default:
                    throw new NotImplementedException($"Unhandled menu index: {index}.");
            }
        }

        private static void BatchUpdateRoomTemplates()
        {
            var path = ProjectSettings.GetSetting(BatchUpdateSearchPathSetting, "res://").AsString();
            BatchUpdaterTool.BatchUpdateRoomTemplates(path);
        }
    }
}
#endif