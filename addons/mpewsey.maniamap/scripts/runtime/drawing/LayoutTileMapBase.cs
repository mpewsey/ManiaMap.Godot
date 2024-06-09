using Godot;
using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System;

namespace MPewsey.ManiaMapGodot.Drawing
{
    /// <summary>
    /// The base class for drawing a `Layout` to tile maps.
    /// </summary>
    [GlobalClass]
    public abstract partial class LayoutTileMapBase : Node2D
    {
        /// <summary>
        /// The total number of tile map layers.
        /// </summary>
        public const int LayerCount = 8;

        /// <summary>
        /// The tile map layer used for the room fill background.
        /// </summary>
        public const int BackgroundLayer = 0;

        /// <summary>
        /// The tile map layer used for north walls and doors.
        /// </summary>
        public const int NorthLayer = 1;

        /// <summary>
        /// The tile map layer used for east walls and doors.
        /// </summary>
        public const int EastLayer = 2;

        /// <summary>
        /// The tile map layer used for south walls and doors.
        /// </summary>
        public const int SouthLayer = 3;

        /// <summary>
        /// The tile map layer used for west walls and doors.
        /// </summary>
        public const int WestLayer = 4;

        /// <summary>
        /// The tile map layer used for top walls and doors.
        /// </summary>
        public const int TopLayer = 5;

        /// <summary>
        /// The tile map layer used for bottom walls and doors.
        /// </summary>
        public const int BottomLayer = 6;

        /// <summary>
        /// The tile map layer used for features.
        /// </summary>
        public const int FeatureLayer = 7;

        /// <summary>
        /// The container to which tile maps will be added as children. If null, this node will be used.
        /// </summary>
        [Export] public Node Container { get; set; }

        /// <summary>
        /// The map tile set.
        /// </summary>
        [Export] public MapTileSet MapTileSet { get; set; }

        /// <summary>
        /// The color used if the room is set as visible.
        /// </summary>
        [Export] public Color RoomColor { get; set; } = Color.Color8(75, 75, 75);

        /// <summary>
        /// The mode used for drawing doors.
        /// </summary>
        [Export] public DoorDrawMode DoorDrawMode { get; set; } = DoorDrawMode.AllDoors;

        /// <summary>
        /// The currently drawn layout pack.
        /// </summary>
        public LayoutPack LayoutPack { get; protected set; }

        public override void _Ready()
        {
            base._Ready();
            Container ??= this;
        }

        /// <summary>
        /// Returns a new layout state for the given layout with all cells visible.
        /// </summary>
        /// <param name="layout">The layout.</param>
        protected static LayoutState CreateFullyVisibleLayoutState(Layout layout)
        {
            var layoutState = new LayoutState(layout);

            foreach (var roomState in layoutState.RoomStates.Values)
            {
                Array.Fill(roomState.VisibleCells.Array, ~0);
            }

            return layoutState;
        }

        /// <summary>
        /// Sets the tiles for the specified layer (z) coordinate to the provided tile map.
        /// </summary>
        /// <param name="tileMap">The tile map.</param>
        /// <param name="z">The layer coordinate.</param>
        protected void SetTiles(TileMap tileMap, int z)
        {
            tileMap.Clear();

            foreach (var room in LayoutPack.GetRoomsInLayer(z))
            {
                var cells = room.Template.Cells;
                var roomState = LayoutPack.LayoutState?.RoomStates[room.Id];

                for (int i = 0; i < cells.Rows; i++)
                {
                    for (int j = 0; j < cells.Columns; j++)
                    {
                        var cell = cells[i, j];

                        // If cell is empty, go to next cell.
                        if (cell == null)
                            continue;

                        var position = new Vector2DInt(i, j);
                        var isCompletelyVisible = roomState == null || roomState.CellIsVisible(position);

                        // If room state is defined and is not visible, go to next cell.
                        if (!isCompletelyVisible && !roomState.IsVisible)
                            continue;

                        var cellPosition = new Vector2I(room.Position.Y + j, room.Position.X + i);

                        // Get adjacent cells
                        var north = cells.GetOrDefault(i - 1, j);
                        var south = cells.GetOrDefault(i + 1, j);
                        var west = cells.GetOrDefault(i, j - 1);
                        var east = cells.GetOrDefault(i, j + 1);

                        // Get the wall or door tiles
                        var topTile = GetTileCoordinate(room, cell, null, position, DoorDirection.Top);
                        var bottomTile = GetTileCoordinate(room, cell, null, position, DoorDirection.Bottom);
                        var northTile = GetTileCoordinate(room, cell, north, position, DoorDirection.North);
                        var southTile = GetTileCoordinate(room, cell, south, position, DoorDirection.South);
                        var westTile = GetTileCoordinate(room, cell, west, position, DoorDirection.West);
                        var eastTile = GetTileCoordinate(room, cell, east, position, DoorDirection.East);

                        // Add cell background tile
                        var backgroundColor = isCompletelyVisible ? ColorUtility.ConvertColor4ToColor(room.Color) : RoomColor;
                        SetBackgroundTile(tileMap, cellPosition, backgroundColor);

                        // Draw tiles
                        SetTile(tileMap, TopLayer, cellPosition, topTile);
                        SetTile(tileMap, BottomLayer, cellPosition, bottomTile);
                        SetTile(tileMap, NorthLayer, cellPosition, northTile);
                        SetTile(tileMap, SouthLayer, cellPosition, southTile);
                        SetTile(tileMap, EastLayer, cellPosition, eastTile);
                        SetTile(tileMap, WestLayer, cellPosition, westTile);

                        // Draw features only if the cell is completely visible
                        if (isCompletelyVisible)
                            SetTile(tileMap, FeatureLayer, cellPosition, GetFeatureCoordinate(cell));
                    }
                }
            }
        }

        /// <summary>
        /// Sets the background tile to the specified tile map coordinate.
        /// </summary>
        /// <param name="tileMap">The tile map.</param>
        /// <param name="cellPosition">The cell coordinate in the tile map.</param>
        /// <param name="color">The background color.</param>
        protected void SetBackgroundTile(TileMap tileMap, Vector2I cellPosition, Color color)
        {
            var alternativeId = MapTileSet.GetBackgroundAlternativeId(color);
            tileMap.SetCell(BackgroundLayer, cellPosition, 0, MapTileSet.Background, alternativeId);
        }

        /// <summary>
        /// Sets the tile to the specified tile map doordinate. Does nothing if an element of the atlas coordinate is negative.
        /// </summary>
        /// <param name="tileMap">The tile map.</param>
        /// <param name="layer">The tile map layer to which the tile will be set.</param>
        /// <param name="cellPosition">The cell coordinate in the tile map.</param>
        /// <param name="atlasPosition">The tile atlas coordinate.</param>
        protected static void SetTile(TileMap tileMap, int layer, Vector2I cellPosition, Vector2I atlasPosition)
        {
            if (atlasPosition.X >= 0 && atlasPosition.Y >= 0)
                tileMap.SetCell(layer, cellPosition, 0, atlasPosition, 0);
        }

        /// <summary>
        /// Finds the first feature with an existing tile coordinate and returns it.
        /// If such a feature does not exist, returns Vector2I(-1, -1).
        /// </summary>
        /// <param name="cell">The cell for which features will be queried.</param>
        protected Vector2I GetFeatureCoordinate(Cell cell)
        {
            foreach (var feature in cell.Features)
            {
                var atlasPosition = MapTileSet.GetTileCoordinate(feature);

                if (atlasPosition.X >= 0 && atlasPosition.Y >= 0)
                    return atlasPosition;
            }

            return new Vector2I(-1, -1);
        }

        /// <summary>
        /// Returns the atlas coordinate for the wall or door tile corresponding to the specified door direction.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbor">The neighboring cell in the door direction.</param>
        /// <param name="position">The cell index in the room.</param>
        /// <param name="direction">The door direction.</param>
        protected Vector2I GetTileCoordinate(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && LayoutPack.DoorExists(room.Id, position, direction))
                return MapTileSet.GetTileCoordinate(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTileSet.GetTileCoordinate(MapTileType.GetWallTileType(direction));

            return new Vector2I(-1, -1);
        }

        /// <summary>
        /// Creates a new tile map in the tile map container and returns it.
        /// </summary>
        protected TileMap CreateTileMap()
        {
            var tileMap = new TileMap() { TileSet = MapTileSet.TileSet };
            CreateTileMapLayers(tileMap);
            Container.AddChild(tileMap);
            return tileMap;
        }

        /// <summary>
        /// Creates the required tile map layers in the tile map and configures their settings.
        /// </summary>
        /// <param name="tileMap">The tile map.</param>
        protected static void CreateTileMapLayers(TileMap tileMap)
        {
            var count = tileMap.GetLayersCount();

            for (int i = count; i < LayerCount; i++)
            {
                tileMap.AddLayer(-1);
            }

            for (int i = 0; i < LayerCount; i++)
            {
                tileMap.SetLayerNavigationEnabled(i, false);
            }
        }
    }
}