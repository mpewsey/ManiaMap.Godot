using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Drawing
{
    [GlobalClass]
    public partial class MapTileSet : Resource
    {
        [Export] public TileSet TileSet { get; set; }

        [ExportGroup("Tile Coordinates")]
        [Export] public Vector2I Background { get; set; } = new Vector2I(0, 0);
        [Export] public Vector2I NorthWall { get; set; } = new Vector2I(1, 0);
        [Export] public Vector2I EastWall { get; set; } = new Vector2I(2, 0);
        [Export] public Vector2I SouthWall { get; set; } = new Vector2I(3, 0);
        [Export] public Vector2I WestWall { get; set; } = new Vector2I(4, 0);
        [Export] public Vector2I NorthDoor { get; set; } = new Vector2I(5, 0);
        [Export] public Vector2I EastDoor { get; set; } = new Vector2I(6, 0);
        [Export] public Vector2I SouthDoor { get; set; } = new Vector2I(7, 0);
        [Export] public Vector2I WestDoor { get; set; } = new Vector2I(0, 1);
        [Export] public Vector2I TopDoor { get; set; } = new Vector2I(1, 1);
        [Export] public Vector2I BottomDoor { get; set; } = new Vector2I(2, 1);
        [Export] public FeatureCoordinate[] Features { get; set; } = Array.Empty<FeatureCoordinate>();

        private Dictionary<string, Vector2I> TileCoordinates { get; } = new Dictionary<string, Vector2I>();
        private Dictionary<Color, int> BackgroundAlternativeTileIds { get; } = new Dictionary<Color, int>();
        private bool IsDirty { get; set; } = true;

        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateTileCoordinates();
                BackgroundAlternativeTileIds.Clear();
                IsDirty = false;
            }
        }

        private void PopulateTileCoordinates()
        {
            TileCoordinates.Clear();

            TileCoordinates.Add(MapTileType.NorthWall, NorthWall);
            TileCoordinates.Add(MapTileType.EastWall, EastWall);
            TileCoordinates.Add(MapTileType.SouthWall, SouthWall);
            TileCoordinates.Add(MapTileType.WestWall, WestWall);

            TileCoordinates.Add(MapTileType.NorthDoor, NorthDoor);
            TileCoordinates.Add(MapTileType.EastDoor, EastDoor);
            TileCoordinates.Add(MapTileType.SouthDoor, SouthDoor);
            TileCoordinates.Add(MapTileType.WestDoor, WestDoor);
            TileCoordinates.Add(MapTileType.TopDoor, TopDoor);
            TileCoordinates.Add(MapTileType.BottomDoor, BottomDoor);

            foreach (var feature in Features)
            {
                TileCoordinates.Add(feature.FeatureName, feature.AtlasCoordinate);
            }
        }

        public Vector2I GetTileCoordinate(string name)
        {
            PopulateIfDirty();

            if (string.IsNullOrWhiteSpace(name))
                return new Vector2I(-1, -1);

            if (TileCoordinates.TryGetValue(name, out var coordinate))
                return coordinate;

            return new Vector2I(-1, -1);
        }

        public int GetBackgroundAlternativeId(Color color)
        {
            PopulateIfDirty();

            if (BackgroundAlternativeTileIds.TryGetValue(color, out var id))
                return id;

            return CreateBackgroundAlternativeId(color);
        }

        private int CreateBackgroundAlternativeId(Color color)
        {
            var source = (TileSetAtlasSource)TileSet.GetSource(0);
            var id = source.CreateAlternativeTile(Background);
            var tileData = source.GetTileData(Background, id);
            tileData.Modulate = color;
            BackgroundAlternativeTileIds.Add(color, id);
            return id;
        }
    }
}