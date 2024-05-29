using Godot;
using MPewsey.Common.Collections;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Extension methods for IRoomNode.
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

        /// <summary>
        /// Returns the ManiaMap room template used for procedural generation.
        /// </summary>
        /// <param name="id">The unique template ID.</param>
        /// <param name="name">The template name.</param>
        public static RoomTemplate GetMMRoomTemplate(this IRoomNode room, int id, string name)
        {
            var roomNode = (Node)room;
            var cells = room.GetMMCells();
            room.AddMMDoors(cells);
            room.AddMMFeatures(cells);
            var spots = room.GetMMCollectableSpots();
            var template = new RoomTemplate(id, name, cells, spots);

            try
            {
                template.Validate();
                room.ValidateRoomFlags();
            }
            catch (Exception exception)
            {
                GD.PrintErr($"Room template failed validation: (Scene File Path = {roomNode.SceneFilePath}, Error = {exception.Message})");
                throw;
            }

            return template;
        }

        /// <summary>
        /// Returns an new array of ManiaMap cells for the room.
        /// </summary>
        private static Array2D<Cell> GetMMCells(this IRoomNode room)
        {
            room.SizeActiveCells();
            var rows = room.Rows;
            var columns = room.Columns;
            var activeCells = room.ActiveCells;
            var cells = new Array2D<Cell>(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                var row = activeCells[i];

                for (int j = 0; j < columns; j++)
                {
                    if (row[j])
                        cells[i, j] = Cell.New;
                }
            }

            return cells;
        }

        /// <summary>
        /// Returns a dictionary of ManiaMap collectable spots used by the procedural generator.
        /// </summary>
        private static HashMap<int, CollectableSpot> GetMMCollectableSpots(this IRoomNode room)
        {
            var roomNode = (Node)room;
            var nodes = roomNode.FindChildren("*", room.GetCollectableSpotClassName(), true, false);
            var result = new HashMap<int, CollectableSpot>();

            foreach (var node in nodes)
            {
                var spot = (ICollectableSpot)node;
                result.Add(spot.Id, spot.GetMMCollectableSpot());
            }

            return result;
        }

        /// <summary>
        /// Returns the cell collectable spot class name corresponding to the room type.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <exception cref="NotImplementedException">Raised if the room type is not handled.</exception>
        private static string GetCollectableSpotClassName(this IRoomNode room)
        {
            switch (room)
            {
                case RoomNode2D:
                    return nameof(CollectableSpot2D);
                case RoomNode3D:
                    return nameof(CollectableSpot3D);
                default:
                    throw new NotImplementedException($"Unhandled room type: {room.GetType()}");
            }
        }

        /// <summary>
        /// Adds ManiaMap features for the room to the specified cells array.
        /// </summary>
        /// <param name="cells">The cells array.</param>
        private static void AddMMFeatures(this IRoomNode room, Array2D<Cell> cells)
        {
            var roomNode = (Node)room;
            var nodes = roomNode.FindChildren("*", room.GetFeatureClassName(), true, false);

            foreach (var node in nodes)
            {
                var feature = (IFeature)node;
                var cell = cells[feature.Row, feature.Column];
                cell.AddFeature(feature.FeatureName);
            }
        }

        /// <summary>
        /// Returns the feature class name corresponding to the room type.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <exception cref="NotImplementedException">Raised if the room type is not handled.</exception>
        private static string GetFeatureClassName(this IRoomNode room)
        {
            switch (room)
            {
                case RoomNode2D:
                    return nameof(Feature2D);
                case RoomNode3D:
                    return nameof(Feature3D);
                default:
                    throw new NotImplementedException($"Unhandled room type: {room.GetType()}");
            }
        }

        /// <summary>
        /// Adds ManiaMap doors for the room to the specified cells array.
        /// </summary>
        /// <param name="cells">The cells array.</param>
        private static void AddMMDoors(this IRoomNode room, Array2D<Cell> cells)
        {
            var roomNode = (Node)room;
            var nodes = roomNode.FindChildren("*", room.GetDoorClassName(), true, false);

            foreach (var node in nodes)
            {
                var door = (IDoorNode)node;
                var cell = cells[door.Row, door.Column];
                cell.SetDoor(door.DoorDirection, door.GetMMDoor());
            }
        }

        /// <summary>
        /// Returns the door class name corresponding to the room type.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <exception cref="NotImplementedException">Raised if the room type is not handled.</exception>
        private static string GetDoorClassName(this IRoomNode room)
        {
            switch (room)
            {
                case RoomNode2D:
                    return nameof(DoorNode2D);
                case RoomNode3D:
                    return nameof(DoorNode3D);
                default:
                    throw new NotImplementedException($"Unhandled room type: {room.GetType()}");
            }
        }

        /// <summary>
        /// Validates that the room flags have unique ID's. Throws an exception if they do not.
        /// </summary>
        /// <exception cref="DuplicateIdException">Thrown if two room flags share the same ID.</exception>
        public static void ValidateRoomFlags(this IRoomNode room)
        {
            var roomNode = (Node)room;
            var nodes = roomNode.FindChildren("*", room.GetRoomFlagClassName(), true, false);
            var flags = new HashSet<int>(nodes.Count);

            foreach (var node in nodes)
            {
                var flag = (IRoomFlag)node;

                if (!flags.Add(flag.Id))
                    throw new DuplicateIdException($"Duplicate room flag ID: (ID = {flag.Id}, Path = {roomNode.GetPathTo(node)}.");
            }
        }

        /// <summary>
        /// Returns the room flag class name corresponding to the room type.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <exception cref="NotImplementedException">Raised if the room type is not handled.</exception>
        private static string GetRoomFlagClassName(this IRoomNode room)
        {
            switch (room)
            {
                case RoomNode2D:
                    return nameof(RoomFlag2D);
                case RoomNode3D:
                    return nameof(RoomFlag3D);
                default:
                    throw new NotImplementedException($"Unhandled room type: {room.GetType()}");
            }
        }

        /// <summary>
        /// Runs auto assign and updates the referenced RoomTemplateResource.
        /// Saves the room template resource to a separate file if it is not saved already.
        /// 
        /// This method returns true when successful and false otherwise.
        /// </summary>
        public static bool UpdateRoomTemplate(this IRoomNode room)
        {
            var roomNode = (Node)room;
            var sceneFilePath = roomNode.SceneFilePath;

            if (!SceneIsSavedToFile(sceneFilePath))
            {
                GD.PrintErr("Scene must be saved to file first.");
                return false;
            }

            room.AutoAssign();
            room.RoomTemplate.Initialize(room);

            if (!ResourceIsSavedToFile(room.RoomTemplate, sceneFilePath))
            {
                var path = roomNode.SceneFilePath.GetBaseName() + ".room_template.tres";
                ResourceSaver.Save(room.RoomTemplate, path);
                room.RoomTemplate = ResourceLoader.Load<RoomTemplateResource>(path);
                GD.Print($"Saved room template to: {path}");
            }

            if (Engine.IsEditorHint())
                EditorInterface.Singleton.MarkSceneAsUnsaved();

            GD.PrintRich("[color=#00ff00]Room template updated.[/color]");
            return true;
        }

        /// <summary>
        /// Returns true if the specified resource is saved in a separate file.
        /// </summary>
        /// <param name="resource">The resource.</param>
        private static bool ResourceIsSavedToFile(Resource resource, string sceneFilePath)
        {
            var path = resource.ResourcePath;
            return !string.IsNullOrEmpty(path) && !path.StartsWith(sceneFilePath);
        }

        /// <summary>
        /// Returns true if the room scene is saved to a file.
        /// </summary>
        private static bool SceneIsSavedToFile(string sceneFilePath)
        {
            return !string.IsNullOrEmpty(sceneFilePath);
        }

        /// <summary>
        /// Sets the room template if it doesn't already exist and runs auto assign on all descendent cell children.
        /// </summary>
        public static void AutoAssign(this IRoomNode room)
        {
            var roomNode = (Node)room;
            room.RoomTemplate ??= new RoomTemplateResource() { TemplateName = roomNode.Name };
            room.RoomTemplate.Id = Rand.AutoAssignId(room.RoomTemplate.Id);
            room.SizeActiveCells();
            var nodes = roomNode.FindChildren("*", room.GetCellChildClassName(), true, false);

            switch (room)
            {
                case RoomNode2D room2d:
                    AutoAssign2D(room2d, nodes);
                    break;
                case RoomNode3D room3d:
                    AutoAssign3D(room3d, nodes);
                    break;
                default:
                    throw new NotImplementedException($"Unhandled type: {room.GetType()}");
            }

            if (Engine.IsEditorHint())
                EditorInterface.Singleton.MarkSceneAsUnsaved();

            GD.PrintRich($"[color=#00ff00]Performed auto assign on {nodes.Count} cell children.[/color]");
        }

        /// <summary>
        /// Performs 2D auto assignment on the array of nodes.
        /// </summary>
        /// <param name="room">The ancestor room.</param>
        /// <param name="nodes">The array of nodes.</param>
        private static void AutoAssign2D(RoomNode2D room, Godot.Collections.Array<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var child = (CellChild2D)node;
                child.AutoAssign(room);
            }
        }

        /// <summary>
        /// Performs 3D auto assignment on the array of nodes.
        /// </summary>
        /// <param name="room">The ancestor room.</param>
        /// <param name="nodes">The array of nodes.</param>
        private static void AutoAssign3D(RoomNode3D room, Godot.Collections.Array<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var child = (CellChild3D)node;
                child.AutoAssign(room);
            }
        }

        /// <summary>
        /// Returns the cell child class name corresponding to the room type.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <exception cref="NotImplementedException">Raised if the room type is not handled.</exception>
        private static string GetCellChildClassName(this IRoomNode room)
        {
            switch (room)
            {
                case RoomNode2D:
                    return nameof(CellChild2D);
                case RoomNode3D:
                    return nameof(CellChild3D);
                default:
                    throw new NotImplementedException($"Unhandled room type: {room.GetType()}");
            }
        }
    }
}