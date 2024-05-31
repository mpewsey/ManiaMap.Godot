#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class RoomNode3DToolbar : Control
    {
        private const string EditModeDisabledTooltip = "Cannot Edit Cells (Go To Top View)";
        private const string EditModeEnabledTooltip = "Can Edit Cells (In Top View)";

        [Export] public Button DisplayCellsButton { get; set; }
        [Export] public Button NoneEditModeButton { get; set; }
        [Export] public Button ActivateEditModeButton { get; set; }
        [Export] public Button DeactivateEditModeButton { get; set; }
        [Export] public Button ToggleEditModeButton { get; set; }
        [Export] public Button AutoAssignButton { get; set; }
        [Export] public Button UpdateRoomTemplateButton { get; set; }

        [Export] public TextureRect EditModeEnabledIcon { get; set; }
        [Export] public Texture2D EditModeEnabledTexture { get; set; }
        [Export] public Texture2D EditModeDisabledTexture { get; set; }

        public CellActivity CellEditMode { get; private set; } = CellActivity.None;
        public bool DisplayCells { get; private set; } = true;
        public bool EditViewEnabled { get; private set; }
        private RoomNode3D Room { get; set; }

        public override void _Ready()
        {
            base._Ready();
            DisplayCellsButton.Toggled += OnDisplayCellsButtonToggled;
            NoneEditModeButton.Toggled += OnNoneEditModeButtonToggled;
            ActivateEditModeButton.Toggled += OnActivateEditModeButtonToggled;
            DeactivateEditModeButton.Toggled += OnDeactivateEditModeButtonToggled;
            ToggleEditModeButton.Toggled += OnToggleEditModeButtonToggled;
            AutoAssignButton.Pressed += OnAutoAssignButtonPressed;
            UpdateRoomTemplateButton.Pressed += OnUpdateRoomTemplateButtonPressed;
            ToggleButton(DisplayCellsButton, true);
            ToggleButton(NoneEditModeButton, true);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Engine.IsEditorHint())
                PopulateEditModeEnabledIcon();
        }

        private void PopulateEditModeEnabledIcon()
        {
            if (CameraIsLookingDown())
            {
                EditModeEnabledIcon.Texture = EditModeEnabledTexture;
                EditModeEnabledIcon.TooltipText = EditModeEnabledTooltip;
                return;
            }

            EditModeEnabledIcon.Texture = EditModeDisabledTexture;
            EditModeEnabledIcon.TooltipText = EditModeDisabledTooltip;
        }

        private static bool CameraIsLookingDown()
        {
            var camera = EditorInterface.Singleton.GetEditorViewport3D().GetCamera3D();
            var cameraDirection = camera.GlobalRotationDegrees;
            return cameraDirection.IsEqualApprox(new Vector3(-90, 0, 0));
        }

        private static void ToggleButton(Button button, bool toggled)
        {
            button.SetPressedNoSignal(toggled);
            button.Flat = !toggled;
        }

        public void SetTargetRoom(RoomNode3D room)
        {
            if (!IsInstanceValid(Room) || room != Room)
            {
                Room = room;
                room.TreeExited += OnRoomExitedTree;
            }
        }

        private void OnRoomExitedTree()
        {
            if (IsInstanceValid(Room))
                Room.TreeExited -= OnRoomExitedTree;

            Room = null;
        }

        private void OnAutoAssignButtonPressed()
        {
            if (IsInstanceValid(Room))
                Room.AutoAssign();
        }

        private void OnUpdateRoomTemplateButtonPressed()
        {
            if (IsInstanceValid(Room))
                Room.UpdateRoomTemplate();
        }

        private void OnDisplayCellsButtonToggled(bool toggled)
        {
            DisplayCells = toggled;
            ToggleButton(DisplayCellsButton, toggled);

            if (IsInstanceValid(Room))
                Room.EmitOnCellGridChanged();
        }

        private void OnNoneEditModeButtonToggled(bool toggled)
        {
            ToggleButton(NoneEditModeButton, toggled);

            if (toggled)
                CellEditMode = CellActivity.None;
        }

        private void OnActivateEditModeButtonToggled(bool toggled)
        {
            ToggleButton(ActivateEditModeButton, toggled);

            if (toggled)
                CellEditMode = CellActivity.Activate;
        }

        private void OnDeactivateEditModeButtonToggled(bool toggled)
        {
            ToggleButton(DeactivateEditModeButton, toggled);

            if (toggled)
                CellEditMode = CellActivity.Deactivate;
        }

        private void OnToggleEditModeButtonToggled(bool toggled)
        {
            ToggleButton(ToggleEditModeButton, toggled);

            if (toggled)
                CellEditMode = CellActivity.Toggle;
        }
    }
}
#endif