using Godot;
using System;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Contains extension methods for IRoomNode.
    /// </summary>
    public static class IRoomNodeExtensions
    {
        /// <summary>
        /// Returns true if the specified cell index is within the bounds of the room.
        /// </summary>
        /// <param name="row">The cell row.</param>
        /// <param name="column">The cell column.</param>
        public static bool CellIndexExists(this IRoomNode room, int row, int column)
        {
            return (uint)row < (uint)room.Rows && (uint)column < (uint)room.Columns;
        }

        /// <summary>
        /// Returns the cell activity for the specified cell index.
        /// </summary>
        /// <param name="row">The cell row index.</param>
        /// <param name="column">The cell column index.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the specified index is out of range.</exception>
        public static bool GetCellActivity(this IRoomNode room, int row, int column)
        {
            if (!room.CellIndexExists(row, column))
                throw new IndexOutOfRangeException($"Cell index does not exist: ({row}, {column}).");

            return room.ActiveCells[row][column];
        }

        /// <summary>
        /// Returns true if the specified cell indexes both exist.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        public static bool CellIndexRangeExists(this IRoomNode room, Vector2I startIndex, Vector2I endIndex)
        {
            return room.CellIndexExists(startIndex.X, startIndex.Y) && room.CellIndexExists(endIndex.X, endIndex.Y);
        }

        /// <summary>
        /// Sets the cell activities for all cells in the specified index range.
        /// The order of the start and end indexes does not matter.
        /// If either the start or end indexes fall outside the cell grid, no action is taken
        /// and the method returns false.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="activity">The cell activity option.</param>
        public static bool SetCellActivities(this IRoomNode room, Vector2I startIndex, Vector2I endIndex, CellActivity activity)
        {
            if (activity == CellActivity.None || !room.CellIndexRangeExists(startIndex, endIndex))
                return false;

            var startRow = Mathf.Min(startIndex.X, endIndex.X);
            var endRow = Mathf.Max(startIndex.X, endIndex.X);
            var startColumn = Mathf.Min(startIndex.Y, endIndex.Y);
            var endColumn = Mathf.Max(startIndex.Y, endIndex.Y);

            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    room.SetCellActivity(i, j, activity);
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the activity for the specified cell index.
        /// </summary>
        /// <param name="row">The cell row index.</param>
        /// <param name="column">The cell column index.</param>
        /// <param name="activated">True to set the cell as active. False to set the cell as deactivated.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the specified index is out of range.</exception>
        public static void SetCellActivity(this IRoomNode room, int row, int column, bool activated)
        {
            if (!room.CellIndexExists(row, column))
                throw new IndexOutOfRangeException($"Cell index does not exist: ({row}, {column}).");

            room.ActiveCells[row][column] = activated;
        }

        /// <summary>
        /// Sets the activity for the specified cell index.
        /// </summary>
        /// <param name="row">The cell row index.</param>
        /// <param name="column">The cell column index.</param>
        /// <param name="activity">The cell activity option.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the specified index is out of range.</exception>
        /// <exception cref="NotImplementedException">Thrown for an unhandled cell activity.</exception>
        public static void SetCellActivity(this IRoomNode room, int row, int column, CellActivity activity)
        {
            if (!room.CellIndexExists(row, column))
                throw new IndexOutOfRangeException($"Cell index does not exist: ({row}, {column}).");

            switch (activity)
            {
                case CellActivity.None:
                    break;
                case CellActivity.Activate:
                    room.ActiveCells[row][column] = true;
                    break;
                case CellActivity.Deactivate:
                    room.ActiveCells[row][column] = false;
                    break;
                case CellActivity.Toggle:
                    room.ActiveCells[row][column] = !room.ActiveCells[row][column];
                    break;
                default:
                    throw new NotImplementedException($"Unhandled cell activity: {activity}.");
            }
        }

        /// <summary>
        /// Sizes the active cells array to match the current rows and columns properties.
        /// Any newly added cells are active by default.
        /// </summary>
        public static void SizeActiveCells(this IRoomNode room)
        {
            var rows = room.Rows;
            var columns = room.Columns;
            var activeCells = room.ActiveCells;

            while (activeCells.Count > rows)
            {
                activeCells.RemoveAt(activeCells.Count - 1);
            }

            foreach (var row in activeCells)
            {
                while (row.Count > columns)
                {
                    row.RemoveAt(row.Count - 1);
                }

                while (row.Count < columns)
                {
                    row.Add(true);
                }
            }

            while (activeCells.Count < rows)
            {
                activeCells.Add(new Godot.Collections.Array<bool>(Enumerable.Repeat(true, columns)));
            }
        }
    }
}