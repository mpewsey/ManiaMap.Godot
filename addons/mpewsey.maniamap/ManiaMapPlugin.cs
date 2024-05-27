#if TOOLS
using Godot;
using MPewsey.ManiaMapGodot.Graphs;
using MPewsey.ManiaMapGodot.Graphs.Editor;
using System;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class ManiaMapPlugin : EditorPlugin
    {
        private const string MenuName = "Mania Map";
        private const string GraphEditorDockButtonName = "Graph Editor";
        private const string BatchUpdateSearchPathSetting = "mania_map/settings/batch_update_search_path";

        public static ManiaMapPlugin Current { get; private set; }

        public RoomNode2DToolbar RoomNode2DToolbar { get; private set; }
        private LayoutGraphEditor GraphEditor { get; set; }
        private Button GraphEditorDockButton { get; set; }

        public override void _EnterTree()
        {
            Current = this;
            EditorInputs.AddInputActions();
            CreateProjectSettings();
            AddToolMenu();
            AddGraphEditorDock();
            CreateRoomNode2DToolbar();
        }

        public override void _ExitTree()
        {
            Current = null;
            EditorInputs.RemoveInputActions();
            RemoveToolMenuItem(MenuName);
            RemoveGraphEditorDock();
            RemoveRoomNode2DToolbar();
        }

        public override bool _Handles(GodotObject obj)
        {
            return obj is LayoutGraphResource || obj is RoomNode2D;
        }

        public override void _Edit(GodotObject obj)
        {
            if (obj is RoomNode2D room2d)
            {
                RoomNode2DToolbar.SetTargetRoom(room2d);
                RoomNode2DToolbar.MoveToFront();
                RoomNode2DToolbar.Visible = true;
                return;
            }

            RoomNode2DToolbar.Visible = false;

            if (obj is LayoutGraphResource graph)
            {
                GraphEditorDockButton.Visible = true;
                MakeBottomPanelItemVisible(GraphEditor);
                GraphEditor.SetEditorTarget(graph);
            }
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

        private void AddGraphEditorDock()
        {
            var scene = ResourceLoader.Load<PackedScene>(ManiaMapResources.Scenes.LayoutGraphEditorScene);
            GraphEditor = scene.Instantiate<LayoutGraphEditor>();
            GraphEditorDockButton = AddControlToBottomPanel(GraphEditor, GraphEditorDockButtonName);
            GraphEditorDockButton.Visible = false;
        }

        private void RemoveGraphEditorDock()
        {
            if (IsInstanceValid(GraphEditor))
            {
                RemoveControlFromBottomPanel(GraphEditor);
                GraphEditor.QueueFree();
            }

            if (IsInstanceValid(GraphEditorDockButton))
                GraphEditorDockButton.QueueFree();
        }

        public void HideGraphEditorDock()
        {
            if (IsInstanceValid(GraphEditorDockButton))
                GraphEditorDockButton.Visible = false;

            HideBottomPanel();
        }

        private void CreateRoomNode2DToolbar()
        {
            var mainScreen2d = EditorInterface.Singleton.GetEditorMainScreen().GetChild(0);
            var hFlowContainers = mainScreen2d.FindChildren("*", nameof(HFlowContainer), true, false);
            var buttonContainer = (HFlowContainer)hFlowContainers[0];
            var scene = ResourceLoader.Load<PackedScene>(ManiaMapResources.Scenes.RoomNode2DToolbarScene);
            RoomNode2DToolbar = scene.Instantiate<RoomNode2DToolbar>();
            buttonContainer.AddChild(RoomNode2DToolbar);
            RoomNode2DToolbar.Visible = false;
        }

        private void RemoveRoomNode2DToolbar()
        {
            if (IsInstanceValid(RoomNode2DToolbar))
                RoomNode2DToolbar.QueueFree();
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