#if TOOLS
using Godot;

namespace MPewsey.ManiaMapGodot.Editor
{
    [Tool]
    public partial class CellGrid2DGizmo : Node2D
    {
        private RoomNode2D Room { get; set; }

        public static CellGrid2DGizmo CreateInstance(RoomNode2D room)
        {
            var gizmo = new CellGrid2DGizmo() { Room = room, ZIndex = (int)RenderingServer.CanvasItemZMax };
            room.OnCellGridChanged += gizmo.OnCellGridChanged;
            room.AddChild(gizmo);
            return gizmo;
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

            if (Engine.IsEditorHint() && IsInstanceValid(Room))
                DrawCells();
        }

        private static bool DisplayCells()
        {
            return ManiaMapPlugin.PluginIsValid()
                && ManiaMapPlugin.Current.RoomNode2DToolbar.DisplayCells;
        }

        private void DrawCells()
        {
            if (DisplayCells())
            {
                var activeFillColor = ManiaMapProjectSettings.GetRoom2DActiveCellColor();
                var inactiveFillColor = ManiaMapProjectSettings.GetRoom2DInactiveCellColor();
                var lineColor = ManiaMapProjectSettings.GetRoom2DCellLineColor();
                DrawCellRects(inactiveFillColor, lineColor, false);
                DrawCellXs(lineColor, false);
                DrawCellRects(activeFillColor, lineColor, true);
            }
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
                        var rect = new Rect2(Room.CellCenterLocalPosition(i, j) - 0.5f * cellSize, cellSize);
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
                        var topLeft = Room.CellCenterLocalPosition(i, j) - 0.5f * cellSize;
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