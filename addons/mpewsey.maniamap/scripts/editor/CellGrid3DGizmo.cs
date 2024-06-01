#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class CellGrid3DGizmo : Node3D
    {
        private static StringName AlbedoParameterName { get; } = "albedo";

        private RoomNode3D Room { get; set; }
        private BoxMesh BoxMesh { get; } = new BoxMesh();
        private ArrayMesh BoxEdgeMesh { get; } = BoxGizmo.CreateCubeEdgeMesh(1.002f);

        public static CellGrid3DGizmo CreateInstance(RoomNode3D room)
        {
            var gizmo = new CellGrid3DGizmo() { Room = room };
            room.OnCellGridChanged += gizmo.OnCellGridChanged;
            gizmo.PopulateCells();
            room.AddChild(gizmo);
            return gizmo;
        }

        private void OnCellGridChanged()
        {
            if (Engine.IsEditorHint() && IsInstanceValid(Room))
                PopulateCells();
        }

        private static bool DisplayCells()
        {
            return ManiaMapPlugin.PluginIsValid()
                && ManiaMapPlugin.Current.RoomNode3DToolbar.DisplayCells;
        }

        private void PopulateCells()
        {
            SizeCells();
            var index = 0;
            var cells = Room.ActiveCells;
            var cellSize = Room.CellSize;
            var displayCells = DisplayCells();
            var edgeColor = ManiaMapProjectSettings.GetRoom3DCellLineColor();
            var activeColor = ManiaMapProjectSettings.GetRoom3DActiveCellColor();
            var inactivecolor = ManiaMapProjectSettings.GetRoom3DInactiveCellColor();

            for (int i = 0; i < cells.Count; i++)
            {
                var row = cells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    var cell = GetChild<MeshInstance3D>(index++);
                    var edges = cell.GetChild<MeshInstance3D>(0);
                    edges.SetInstanceShaderParameter(AlbedoParameterName, edgeColor);
                    var color = row[j] ? activeColor : inactivecolor;
                    cell.SetInstanceShaderParameter(AlbedoParameterName, color);
                    cell.Position = Room.CellCenterLocalPosition(i, j);
                    cell.Scale = cellSize;
                    cell.Visible = displayCells;
                }
            }
        }

        private void SizeCells()
        {
            var size = Room.Rows * Room.Columns;
            var count = GetChildCount();

            for (int i = size; i < count; i++)
            {
                GetChild(i).QueueFree();
            }

            for (int i = count; i < size; i++)
            {
                var material = ManiaMapResources.Materials.AlbedoMaterial;
                var edges = new MeshInstance3D() { Mesh = BoxEdgeMesh, MaterialOverride = material };
                var cell = new MeshInstance3D() { Mesh = BoxMesh, MaterialOverride = material };
                cell.AddChild(edges);
                AddChild(cell);
            }
        }
    }
}
#endif