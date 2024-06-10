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
        public const string PluginName = "mpewsey.maniamap";
        private const string MenuName = "Mania Map";
        private const string GraphEditorDockButtonName = "Graph Editor";

        public static ManiaMapPlugin Current { get; private set; }

        public RoomNode2DToolbar RoomNode2DToolbar { get; private set; }
        public RoomNode3DToolbar RoomNode3DToolbar { get; private set; }
        private LayoutGraphEditor GraphEditor { get; set; }
        private Button GraphEditorDockButton { get; set; }

        public static bool PluginIsValid()
        {
            return PluginIsEnabled() && IsInstanceValid(Current);
        }

        public static bool PluginIsEnabled()
        {
            return EditorInterface.Singleton.IsPluginEnabled(PluginName);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Engine.IsEditorHint() && PluginRequiresReset())
                ResetPlugin();
        }

        public static bool PluginRequiresReset()
        {
            return PluginIsEnabled() && !IsInstanceValid(Current);
        }

        public static void ResetPlugin()
        {
            if (PluginIsEnabled())
            {
                EditorInterface.Singleton.SetPluginEnabled(PluginName, false);
                EditorInterface.Singleton.SetPluginEnabled(PluginName, true);
                GD.Print("Reset ManiaMap plugin.");
            }
        }

        public override void _EnterTree()
        {
            Current = this;
            EditorInputs.AddInputActions();
            ManiaMapProjectSettings.CreateProjectSettings();
            AddToolMenu();
            AddGraphEditorDock();
            CreateRoomNode2DToolbar();
            CreateRoomNode3DToolbar();
        }

        public override void _ExitTree()
        {
            Current = null;
            EditorInputs.RemoveInputActions();
            RemoveToolMenuItem(MenuName);
            RemoveGraphEditorDock();
            RemoveRoomNode2DToolbar();
            RemoveRoomNode3DToolbar();
        }

        public override bool _Handles(GodotObject obj)
        {
            return obj is Node || obj is LayoutGraphResource;
        }

        public override void _Edit(GodotObject obj)
        {
            var room = FindContainingRoom(obj);

            if (room is RoomNode2D room2d)
            {
                RoomNode2DToolbar.SetTargetRoom(room2d);
                RoomNode2DToolbar.Visible = true;
                return;
            }

            if (room is RoomNode3D room3d)
            {
                RoomNode3DToolbar.SetTargetRoom(room3d);
                RoomNode3DToolbar.Visible = true;
                return;
            }

            RoomNode2DToolbar.Visible = false;
            RoomNode3DToolbar.Visible = false;

            if (obj is LayoutGraphResource graph)
            {
                GraphEditorDockButton.Visible = true;
                MakeBottomPanelItemVisible(GraphEditor);
                GraphEditor.SetEditorTarget(graph);
            }
        }

        private static Node FindContainingRoom(GodotObject obj)
        {
            if (IsInstanceValid(obj) && obj is Node node)
            {
                if (obj is RoomNode2D room2d)
                    return room2d;
                if (obj is RoomNode3D room3d)
                    return room3d;
                return FindContainingRoom(node.GetParent());
            }

            return null;
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
            var toolbar = scene.Instantiate<RoomNode2DToolbar>();
            RoomNode2DToolbar = toolbar;
            buttonContainer.AddChild(toolbar);
            toolbar.Visible = false;
        }

        private void CreateRoomNode3DToolbar()
        {
            var mainScreen3d = EditorInterface.Singleton.GetEditorMainScreen().GetChild(1);
            var hFlowContainers = mainScreen3d.FindChildren("*", nameof(HFlowContainer), true, false);
            var buttonContainer = (HFlowContainer)hFlowContainers[0];
            var scene = ResourceLoader.Load<PackedScene>(ManiaMapResources.Scenes.RoomNode3DToolbarScene);
            var toolbar = scene.Instantiate<RoomNode3DToolbar>();
            RoomNode3DToolbar = toolbar;
            buttonContainer.AddChild(toolbar);
            toolbar.Visible = false;
        }

        private void RemoveRoomNode2DToolbar()
        {
            if (IsInstanceValid(RoomNode2DToolbar))
                RoomNode2DToolbar.QueueFree();
        }

        private void RemoveRoomNode3DToolbar()
        {
            if (IsInstanceValid(RoomNode3DToolbar))
                RoomNode3DToolbar.QueueFree();
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
            var path = ManiaMapProjectSettings.GetBatchUpdateSearchPath();
            BatchUpdaterTool.BatchUpdateRoomTemplates(path);
        }
    }
}
#endif