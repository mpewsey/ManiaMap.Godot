#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    public static class EditorInputs
    {
        public static StringName DeleteAction { get; } = "mania_map/delete";
        public static StringName SelectAllAction { get; } = "mania_map/select_all";
        public static StringName SelectAllNodesAction { get; } = "mania_map/select_all_nodes";
        public static StringName SelectAllEdgesAction { get; } = "mania_map/select_all_edges";

        public static void AddInputActions()
        {
            RemoveInputActions();

            InputMap.AddAction(DeleteAction);
            InputMap.ActionAddEvent(DeleteAction, new InputEventKey { Keycode = Key.Delete });

            InputMap.AddAction(SelectAllAction);
            InputMap.ActionAddEvent(SelectAllAction, new InputEventKey { Keycode = Key.A, CtrlPressed = true });

            InputMap.AddAction(SelectAllNodesAction);
            InputMap.ActionAddEvent(SelectAllNodesAction, new InputEventKey { Keycode = Key.N, CtrlPressed = true });

            InputMap.AddAction(SelectAllEdgesAction);
            InputMap.ActionAddEvent(SelectAllEdgesAction, new InputEventKey { Keycode = Key.E, CtrlPressed = true });
        }

        public static void RemoveInputActions()
        {
            RemoveAction(DeleteAction);
            RemoveAction(SelectAllAction);
            RemoveAction(SelectAllNodesAction);
            RemoveAction(SelectAllNodesAction);
        }

        private static void RemoveAction(StringName name)
        {
            if (InputMap.HasAction(name))
                InputMap.EraseAction(name);
        }
    }
}
#endif