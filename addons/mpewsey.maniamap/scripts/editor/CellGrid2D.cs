#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    public partial class CellGrid2D : Node2D
    {
        private RoomNode2D Room { get; set; }

        public static CellGrid2D CreateInstance(RoomNode2D room)
        {
            var grid = new CellGrid2D() { Room = room, ZIndex = (int)RenderingServer.CanvasItemZMax };
            room.OnCellGridChanged += grid.OnCellGridChanged;
            room.AddChild(grid);
            return grid;
        }

        private void OnCellGridChanged()
        {
            QueueRedraw();
        }

        public override void _Ready()
        {
            base._Ready();

            if (Engine.IsEditorHint())
                QueueRedraw();
        }

        public override void _Draw()
        {
            base._Draw();

            if (Engine.IsEditorHint())
            {
                if (ManiaMapPlugin.Current.RoomNode2DToolbar.DisplayCells)
                    DrawCells();
            }
        }

        private void DrawCells()
        {
            var activeFillColor = new Color(0, 0, 1, 0.1f);
            var inactiveFillColor = new Color(1, 0, 0, 0.1f);
            var activeLineColor = new Color(0.5f, 0.5f, 0.5f);
            var inactiveLineColor = new Color(0.5f, 0.5f, 0.5f);
            DrawCellRects(inactiveFillColor, inactiveLineColor, false);
            DrawCellXs(inactiveLineColor, false);
            DrawCellRects(activeFillColor, activeLineColor, true);
        }

        private void DrawCellRects(Color fillColor, Color lineColor, bool active)
        {
            var cells = Room.ActiveCells;
            var cellSize = Room.CellSize;

            for (int i = 0; i < cells.Count; i++)
            {
                var row = cells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == active)
                    {
                        var rect = new Rect2(Room.CellCenterGlobalPosition(i, j) - 0.5f * cellSize, cellSize);
                        DrawRect(rect, fillColor);
                        DrawRect(rect, lineColor, false);
                    }
                }
            }
        }

        private void DrawCellXs(Color lineColor, bool active)
        {
            var cells = Room.ActiveCells;
            var cellSize = Room.CellSize;

            for (int i = 0; i < cells.Count; i++)
            {
                var row = cells[i];

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == active)
                    {
                        var topLeft = Room.CellCenterGlobalPosition(i, j) - 0.5f * cellSize;
                        var bottomRight = topLeft + cellSize;
                        DrawLine(topLeft, bottomRight, lineColor);
                        DrawLine(new Vector2(topLeft.X, bottomRight.Y), new Vector2(bottomRight.X, topLeft.Y), lineColor);
                    }
                }
            }
        }
    }
}
#endif