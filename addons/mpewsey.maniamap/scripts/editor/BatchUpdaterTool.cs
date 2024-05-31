#if TOOLS
using Godot;
using System.IO;

namespace MPewsey.ManiaMapGodot.Editor
{
    public static class BatchUpdaterTool
    {
        private const string Room2DScriptReference = "[ext_resource type=\"Script\" path=\"res://addons/mpewsey.maniamap/scripts/runtime/RoomNode2D.cs\"";
        private const string Room3DScriptReference = "[ext_resource type=\"Script\" path=\"res://addons/mpewsey.maniamap/scripts/runtime/RoomNode3D.cs\"";

        public static void BatchUpdateRoomTemplates(string searchPath)
        {
            if (AnyScenesAreOpen())
            {
                GD.PrintErr("Cannot run batch update while there are open scenes.");
                return;
            }

            GD.Print("Starting room template batch update...");

            var count = 0;
            var paths = Directory.EnumerateFiles(ProjectSettings.GlobalizePath(searchPath), "*.tscn", SearchOption.AllDirectories);

            foreach (var path in paths)
            {
                if (UpdateRoomTemplate(path))
                    count++;
            }

            GD.PrintRich($"[color=#00ff00]Completed batch update for {count} rooms.[/color]");
        }

        private static bool AnyScenesAreOpen()
        {
            return EditorInterface.Singleton.GetOpenScenes().Length > 0;
        }

        private static bool UpdateRoomTemplate(string path)
        {
            if (!FileContainsRoom(path))
                return false;

            var scene = ResourceLoader.Load<PackedScene>(path);
            var node = scene.Instantiate<Node>(PackedScene.GenEditState.Instance);

            if (node is RoomNode2D room2d)
            {
                room2d.UpdateRoomTemplate();
                return SaveScene(scene, node);
            }

            if (node is RoomNode3D room3d)
            {
                // We have to add 3D scenes to the tree otherwise we get errors when accessing global positions.
                ManiaMapPlugin.Current.AddChild(node);
                node.QueueFree();
                room3d.UpdateRoomTemplate();
                var success = SaveScene(scene, node);
                return success;
            }

            GD.PrintErr($"Skipping unhandled room type: (Type = {node.GetType()}, ScenePath = {path})");
            return false;
        }

        private static bool SaveScene(PackedScene scene, Node node)
        {
            var packError = scene.Pack(node);

            if (packError != Error.Ok)
            {
                GD.PrintErr($"An error occured while packing scene: (Error = {packError}, ScenePath = {scene.ResourcePath})");
                return false;
            }

            var saveError = ResourceSaver.Save(scene);

            if (saveError != Error.Ok)
            {
                GD.PrintErr($"An error occured while saving scene: (Error = {saveError}, SavePath = {scene.ResourcePath})");
                return false;
            }

            return true;
        }

        private static bool FileContainsRoom(string path)
        {
            var lines = File.ReadLines(path);

            foreach (var line in lines)
            {
                if (line.StartsWith(Room2DScriptReference) || line.StartsWith(Room3DScriptReference))
                    return true;
            }

            return false;
        }
    }
}
#endif