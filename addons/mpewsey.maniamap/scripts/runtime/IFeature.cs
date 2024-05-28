namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// A feature to associate with a cell.
    /// </summary>
    public interface IFeature : ICellChild
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        public string FeatureName { get; }
    }
}