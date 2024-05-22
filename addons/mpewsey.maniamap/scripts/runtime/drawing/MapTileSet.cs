using Godot;
using MPewsey.ManiaMap;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Drawing
{
    /// <summary>
    /// Contains references to the map tiles used to draw a `Layout`.
    /// </summary>
    [GlobalClass]
    public partial class MapTileSet : Resource
    {
        /// <summary>
        /// The tile set.
        /// </summary>
        [Export] public TileSet TileSet { get; set; }

        private Vector2I _background = new Vector2I(0, 0);
        /// <summary>
        /// The atlas coordinate of the background tile.
        /// </summary>
        [ExportGroup("Tile Coordinates")]
        [Export] public Vector2I Background { get => _background; set => SetField(ref _background, value); }

        private Vector2I _northWall = new Vector2I(1, 0);
        /// <summary>
        /// The atlas coordinate of the north wall tile.
        /// </summary>
        [Export] public Vector2I NorthWall { get => _northWall; set => SetField(ref _northWall, value); }

        private Vector2I _eastWall = new Vector2I(2, 0);
        /// <summary>
        /// The atlas coordinate of the east wall tile.
        /// </summary>
        [Export] public Vector2I EastWall { get => _eastWall; set => SetField(ref _eastWall, value); }

        private Vector2I _southWall = new Vector2I(3, 0);
        /// <summary>
        /// The atlas coordinate of the south wall tile.
        /// </summary>
        [Export] public Vector2I SouthWall { get => _southWall; set => SetField(ref _southWall, value); }

        private Vector2I _westWall = new Vector2I(4, 0);
        /// <summary>
        /// The atlas coordinate of the west wall tile.
        /// </summary>
        [Export] public Vector2I WestWall { get => _westWall; set => SetField(ref _westWall, value); }

        private Vector2I _northDoor = new Vector2I(5, 0);
        /// <summary>
        /// The atlas coordinate of the north door tile.
        /// </summary>
        [Export] public Vector2I NorthDoor { get => _northDoor; set => SetField(ref _northDoor, value); }

        private Vector2I _eastDoor = new Vector2I(6, 0);
        /// <summary>
        /// The atlas coordinate of the east door tile.
        /// </summary>
        [Export] public Vector2I EastDoor { get => _eastDoor; set => SetField(ref _eastDoor, value); }

        private Vector2I _southDoor = new Vector2I(7, 0);
        /// <summary>
        /// The atlas coordinate of the south door tile.
        /// </summary>
        [Export] public Vector2I SouthDoor { get => _southDoor; set => SetField(ref _southDoor, value); }

        private Vector2I _westDoor = new Vector2I(0, 1);
        /// <summary>
        /// The atlas coordinate of the west door tile.
        /// </summary>
        [Export] public Vector2I WestDoor { get => _westDoor; set => SetField(ref _westDoor, value); }

        private Vector2I _topDoor = new Vector2I(1, 1);
        /// <summary>
        /// The atlas coordinate of the top door tile.
        /// </summary>
        [Export] public Vector2I TopDoor { get => _topDoor; set => SetField(ref _topDoor, value); }

        private Vector2I _bottomDoor = new Vector2I(2, 1);
        /// <summary>
        /// The atlas coordinate of the bottom door tile.
        /// </summary>
        [Export] public Vector2I BottomDoor { get => _bottomDoor; set => SetField(ref _bottomDoor, value); }

        private FeatureAtlasCoordinate[] _features = Array.Empty<FeatureAtlasCoordinate>();
        /// <summary>
        /// An array of feature names and their tile atlas coordinates.
        /// </summary>
        [Export] public FeatureAtlasCoordinate[] Features { get => _features; set => SetField(ref _features, value); }

        /// <summary>
        /// A dictionary of tile atlas coordinates by name.
        /// </summary>
        private Dictionary<string, Vector2I> TileCoordinates { get; } = new Dictionary<string, Vector2I>();

        /// <summary>
        /// A dictionary of background tile alternative ID's by background color.
        /// </summary>
        private Dictionary<Color, int> BackgroundAlternativeTileIds { get; } = new Dictionary<Color, int>();

        /// <summary>
        /// True if the map tiles are dirty and require internal update.
        /// </summary>
        public bool IsDirty { get; private set; } = true;

        /// <summary>
        /// Returns the read-only tile coordinate dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, Vector2I> GetTileCoordinates()
        {
            PopulateIfDirty();
            return TileCoordinates;
        }

        /// <summary>
        /// Returns the read-only background alternative tile ID dictionary.
        /// </summary>
        public IReadOnlyDictionary<Color, int> GetBackgroundAlternativeTileIds()
        {
            PopulateIfDirty();
            return BackgroundAlternativeTileIds;
        }

        private void SetField<T>(ref T field, T value)
        {
            field = value;
            SetDirty();
        }

        /// <summary>
        /// Marks the object as dirty and requiring internal update.
        /// </summary>
        public void SetDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Populates the lookup dictionaries if the object is dirty.
        /// </summary>
        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateTileCoordinates();
                BackgroundAlternativeTileIds.Clear();
                IsDirty = false;
            }
        }

        /// <summary>
        /// Populates the tile coordinates dictionary.
        /// </summary>
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

        /// <summary>
        /// Returns the tile coordinate corresponding to the specified name.
        /// If the name does not exist, returns Vector2I(-1, -1).
        /// </summary>
        /// <param name="name">The tile name.</param>
        public Vector2I GetTileCoordinate(string name)
        {
            PopulateIfDirty();

            if (string.IsNullOrWhiteSpace(name))
                return new Vector2I(-1, -1);

            if (TileCoordinates.TryGetValue(name, out var coordinate))
                return coordinate;

            return new Vector2I(-1, -1);
        }

        /// <summary>
        /// Returns the background alternative tile ID for the specified background color.
        /// If the alternative tile does not already exist, creates it.
        /// </summary>
        /// <param name="color">The background color.</param>
        public int GetBackgroundAlternativeId(Color color)
        {
            PopulateIfDirty();

            if (BackgroundAlternativeTileIds.TryGetValue(color, out var id))
                return id;

            return CreateBackgroundAlternativeId(color);
        }

        /// <summary>
        /// Creates a background alternative tile ID with the specified color and returns the ID.
        /// </summary>
        /// <param name="color">The background color.</param>
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