#if TOOLS
using Godot;
using System;
using System.IO;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class ManiaMapPlugin : EditorPlugin
    {
        private const string MenuName = "Mania Map";
        private const string BatchUpdateSearchPathSetting = "mania_map/settings/batch_update_search_path";
        private const string Room2DScriptReference = "[ext_resource type=\"Script\" path=\"res://addons/mpewsey.maniamap/scripts/runtime/RoomNode2D.cs\"";

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
            if (AnyScenesAreOpen())
            {
                GD.PrintErr("Cannot run batch update while there are open scenes.");
                return;
            }

            GD.Print("Starting room template batch update...");

            var count = 0;
            var searchPath = ProjectSettings.GetSetting(BatchUpdateSearchPathSetting, "res://").AsString();
            var paths = Directory.EnumerateFiles(ProjectSettings.GlobalizePath(searchPath), "*.tscn", SearchOption.AllDirectories);

            foreach (var path in paths)
            {
                if (FileContainsRoom(path) && UpdateRoomTemplate(path))
                    count++;
            }

            GD.Print($"Completed batch update for {count} rooms.");
        }

        private static bool AnyScenesAreOpen()
        {
            return EditorInterface.Singleton.GetOpenScenes().Length > 0;
        }

        private static bool UpdateRoomTemplate(string path)
        {
            path = ProjectSettings.LocalizePath(path);
            var scene = ResourceLoader.Load<PackedScene>(path);
            var node = scene.Instantiate<Node>(PackedScene.GenEditState.Instance);

            switch (node)
            {
                case RoomNode2D room2d:
                    room2d.UpdateRoomTemplateResource();
                    return SaveScene(room2d, path);
                default:
                    GD.PrintErr($"Skipping unhandled room type: (Type = {node.GetType()}, ScenePath = {path})");
                    return false;
            }
        }

        private static bool SaveScene(Node node, string path)
        {
            var scene = new PackedScene();
            var packError = scene.Pack(node);

            if (packError != Error.Ok)
            {
                GD.PrintErr($"An error occured while packing scene: (Error = {packError}, ScenePath = {path})");
                return false;
            }

            var saveError = ResourceSaver.Save(scene, path);

            if (saveError != Error.Ok)
            {
                GD.PrintErr($"An error occured while saving scene: (Error = {packError}, SavePath = {path})");
                return false;
            }

            return true;
        }

        private static bool FileContainsRoom(string path)
        {
            var lines = File.ReadLines(path);

            foreach (var line in lines)
            {
                if (line.StartsWith(Room2DScriptReference))
                    return true;
            }

            return false;
        }
    }
}
#endif