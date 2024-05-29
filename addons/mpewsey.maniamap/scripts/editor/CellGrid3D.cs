#if TOOLS
using Godot;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class CellGrid3D : Node3D
    {
        [Export] public Material ActiveCellMaterial { get; set; }
        [Export] public Material InactiveCellMaterial { get; set; }
        [Export] public Material WireframeMaterial { get; set; }

        private RoomNode3D Room { get; set; }
        private List<MeshInstance3D> Cells { get; } = new List<MeshInstance3D>();
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

            // Back of cube
            tool.AddVertex(new Vector3(-1, -1, -1) * 0.5f);
            tool.AddVertex(new Vector3(1, -1, -1) * 0.5f);

            tool.AddVertex(new Vector3(1, -1, -1) * 0.5f);
            tool.AddVertex(new Vector3(1, 1, -1) * 0.5f);

            tool.AddVertex(new Vector3(1, 1, -1) * 0.5f);
            tool.AddVertex(new Vector3(-1, 1, -1) * 0.5f);

            tool.AddVertex(new Vector3(-1, 1, -1) * 0.5f);
            tool.AddVertex(new Vector3(-1, -1, -1) * 0.5f);

            tool.Commit(mesh);
            return mesh;
        }

        private void OnCellGridChanged()
        {
            if (IsInstanceValid(Room))
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
                    var cell = Cells[index++];
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

            while (Cells.Count > size)
            {
                var index = Cells.Count - 1;
                Cells[index].QueueFree();
                Cells.RemoveAt(index);
            }

            while (Cells.Count < size)
            {
                var cell = new MeshInstance3D() { Mesh = BoxMesh };
                var edges = new MeshInstance3D() { Mesh = BoxEdgeMesh, MaterialOverride = WireframeMaterial };
                cell.AddChild(edges);
                AddChild(cell);
                Cells.Add(cell);
            }
        }
    }
}
#endif