#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class CellGrid3D : Node3D
    {
        [Export] public Material ActiveCellMaterial { get; set; }
        [Export] public Material InactiveCellMaterial { get; set; }
        [Export] public Material WireframeMaterial { get; set; }

        private RoomNode3D Room { get; set; }
        private Mesh BoxMesh { get; set; } = new BoxMesh();
        private Mesh BoxEdgeMesh { get; set; } = CreateBoxEdgeMesh();

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

        private static Mesh CreateBoxEdgeMesh()
        {
            var mesh = new ArrayMesh();
            var tool = new SurfaceTool();
            tool.Begin(Mesh.PrimitiveType.Lines);
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
            if (IsInstanceValid(Room) && ManiaMapPlugin.PluginIsValid())
                PopulateCells();
        }

        private void PopulateCells()
        {
            SizeCells();
            var index = 0;
            var cells = Room.ActiveCells;
            var cellSize = Room.CellSize;
            var displayCells = ManiaMapPlugin.Current.RoomNode3DToolbar.DisplayCells;

            for (int i = 0; i < cells.Count; i++)
            {
                var row = cells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    var cell = GetChild<MeshInstance3D>(index++);
                    cell.MaterialOverride = row[j] ? ActiveCellMaterial : InactiveCellMaterial;
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
                var edges = new MeshInstance3D() { Mesh = BoxEdgeMesh, MaterialOverride = WireframeMaterial };
                var cell = new MeshInstance3D() { Mesh = BoxMesh };
                cell.AddChild(edges);
                AddChild(cell);
            }
        }
    }
}
#endif