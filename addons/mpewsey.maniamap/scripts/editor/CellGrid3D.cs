#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class CellGrid3D : Node3D
    {
        private static StringName AlbedoParameterName { get; } = "albedo";

        [Export] public Material CellMaterial { get; set; }

        private RoomNode3D Room { get; set; }

        public static CellGrid3D CreateInstance(RoomNode3D room)
        {
            var scene = ResourceLoader.Load<PackedScene>(ManiaMapResources.Scenes.CellGrid3DScene);
            var grid = scene.Instantiate<CellGrid3D>();
            grid.Room = room;
            room.OnCellGridChanged += grid.OnCellGridChanged;
            grid.PopulateCells();
            room.AddChild(grid);
            return grid;
        }

        private ArrayMesh CreateBoxEdgeMesh()
        {
            var mesh = new ArrayMesh();
            var tool = new SurfaceTool();
            tool.Begin(Mesh.PrimitiveType.Lines);
            tool.SetMaterial(CellMaterial);
            var scale = 0.5001f;

            var a = new Vector3(-1, -1, -1) * scale;
            var b = new Vector3(1, -1, -1) * scale;
            var c = new Vector3(1, 1, -1) * scale;
            var d = new Vector3(-1, 1, -1) * scale;

            var e = new Vector3(-1, -1, 1) * scale;
            var f = new Vector3(1, -1, 1) * scale;
            var g = new Vector3(1, 1, 1) * scale;
            var h = new Vector3(-1, 1, 1) * scale;

            // Back side
            tool.AddVertex(a);
            tool.AddVertex(b);

            tool.AddVertex(b);
            tool.AddVertex(c);

            tool.AddVertex(c);
            tool.AddVertex(d);

            tool.AddVertex(d);
            tool.AddVertex(a);

            // Front side
            tool.AddVertex(e);
            tool.AddVertex(f);

            tool.AddVertex(f);
            tool.AddVertex(g);

            tool.AddVertex(g);
            tool.AddVertex(h);

            tool.AddVertex(h);
            tool.AddVertex(e);

            // Left side
            tool.AddVertex(a);
            tool.AddVertex(e);

            tool.AddVertex(d);
            tool.AddVertex(h);

            // Right side
            tool.AddVertex(b);
            tool.AddVertex(f);

            tool.AddVertex(c);
            tool.AddVertex(g);

            tool.Commit(mesh);
            return mesh;
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
                var edges = new MeshInstance3D() { Mesh = CreateBoxEdgeMesh() };
                var boxMesh = new BoxMesh() { Material = CellMaterial };
                var cell = new MeshInstance3D() { Mesh = boxMesh };
                cell.AddChild(edges);
                AddChild(cell);
            }
        }
    }
}
#endif