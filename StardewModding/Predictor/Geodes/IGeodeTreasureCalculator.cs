namespace Dannnno.StardewMods.Predictor.Geodes
{
    /// <summary>
    /// Standard interface for calculating the geode's contents
    /// </summary>
    public interface IGeodeTreasureCalculator
    {
        /// <summary>
        /// Get the treasure from a geode
        /// </summary>
        /// <param name="geode">The geode to get the treasure from</param>
        /// <returns>Buried gold</returns>
        public StardewValley.Object GetTreasureFromGeode(StardewValley.Object geode);
    }
}
