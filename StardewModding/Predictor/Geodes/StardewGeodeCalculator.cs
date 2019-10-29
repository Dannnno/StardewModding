using StardewValley;

namespace Dannnno.StardewMods.Predictor.Geodes
{
    /// <summary>
    /// Standard method of calculating treasures
    /// </summary>
    public class StardewGeodeCalculator : IGeodeTreasureCalculator
    {
        /// <summary>
        /// Get the treasure from the geode
        /// </summary>
        /// <param name="geode">The geode</param>
        /// <returns>The treasure</returns>
        public Object GetTreasureFromGeode(Object geode)
        {
            return Utility.getTreasureFromGeode(geode); // Why copy the method when we can just use the game's?
        }
    }
}
