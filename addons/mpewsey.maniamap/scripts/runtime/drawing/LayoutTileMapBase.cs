using Godot;
using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Drawing
{
    public abstract partial class LayoutTileMapBase : Node2D
    {
        public const int LayerCount = 8;
        public const int BackgroundLayer = 0;
        public const int NorthLayer = 1;
        public const int EastLayer = 2;
        public const int SouthLayer = 3;
        public const int WestLayer = 4;
        public const int TopLayer = 5;
        public const int BottomLayer = 6;
        public const int FeatureLayer = 7;

        [Export] public Node Container { get; set; }
        [Export] public MapTileSet MapTileSet { get; set; }
        [Export] public Color RoomColor { get; set; } = Color.Color8(75, 75, 75);
        [Export] public DoorDrawMode DoorDrawMode { get; set; } = DoorDrawMode.AllDoors;

        public Layout Layout { get; protected set; }
        public LayoutState LayoutState { get; protected set; }
        protected Dictionary<int, List<Room>> RoomsByLayer { get; set; } = new Dictionary<int, List<Room>>();
        protected Dictionary<Uid, List<DoorPosition>> RoomDoors { get; set; } = new Dictionary<Uid, List<DoorPosition>>();

        public override void _Ready()
        {
            base._Ready();
            Container ??= this;
        }

        protected virtual void Initialize(Layout layout, LayoutState layoutState)
        {
            Layout = layout;
            LayoutState = layoutState;
            RoomDoors = layout.GetRoomDoors();
            RoomsByLayer = layout.GetRoomsByLayer();
        }

        protected void SetTiles(TileMap tileMap, int z)
        {
            tileMap.Clear();

            foreach (var room in RoomsByLayer[z])
            {
                var cells = room.Template.Cells;
                var roomState = LayoutState?.RoomStates[room.Id];

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

        protected void SetBackgroundTile(TileMap tileMap, Vector2I cellPosition, Color color)
        {
            var alternativeId = MapTileSet.GetBackgroundAlternativeId(color);
            tileMap.SetCell(BackgroundLayer, cellPosition, 0, MapTileSet.Background, alternativeId);
        }

        protected static void SetTile(TileMap tileMap, int layer, Vector2I cellPosition, Vector2I atlasPosition)
        {
            if (atlasPosition != new Vector2I(-1, -1))
                tileMap.SetCell(layer, cellPosition, 0, atlasPosition, 0);
        }

        protected Vector2I GetFeatureCoordinate(Cell cell)
        {
            foreach (var feature in cell.Features)
            {
                var atlasPosition = MapTileSet.GetTileCoordinate(feature);

                if (atlasPosition != new Vector2I(-1, -1))
                    return atlasPosition;
            }

            return new Vector2I(-1, -1);
        }

        protected Vector2I GetTileCoordinate(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTileSet.GetTileCoordinate(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTileSet.GetTileCoordinate(MapTileType.GetWallTileType(direction));

            return new Vector2I(-1, -1);
        }

        protected bool DoorExists(Room room, Vector2DInt position, DoorDirection direction)
        {
            if (RoomDoors.TryGetValue(room.Id, out var doors))
            {
                foreach (var door in doors)
                {
                    if (door.Matches(position, direction))
                        return true;
                }
            }

            return false;
        }

        protected TileMap CreateTileMap()
        {
            var tileMap = new TileMap() { TileSet = MapTileSet.TileSet };
            CreateTileMapLayers(tileMap);
            Container.AddChild(tileMap);
            return tileMap;
        }

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