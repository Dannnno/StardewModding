using Dannnno.StardewMods.Predictor.Shared;
using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.Geodes
{
    /// <summary>
    /// Implementation of a stardew geode service that uses the string "Geode" to identify geodes
    /// </summary>
    public class StardewGeodeService<ProviderType> : IGeodeService<ProviderType> where ProviderType : IStardewObjectProvider
    {
        /// <summary>
        /// Get the set of geode objects from game data
        /// </summary>
        /// <param name="provider">The data provider to get objects from</param>
        /// <returns>Enumerable over the geode objects in game data</returns>
        public IEnumerable<StardewValley.Object> RetrieveGeodes(ProviderType provider)
        {
            foreach (var stardewObject in provider.GetStardewRawObjects())
            {
                if (IsObjectInfoAGeode(stardewObject.Value))
                {
                    yield return new StardewValley.Object(stardewObject.Key, 0); // Stack count doesn't matter
                }
            }
        }

        /// <summary>
        /// Determine whether a given record from Data/ObjectInformation is a geode
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="objectInformation">The string literal from the object information</param>
        /// <returns>Whether the object is a geode</returns>
        /// <remarks>This is currently really stupid, and just does a check for the strings "Geode" and "geode"</remarks>
        private bool IsObjectInfoAGeode(string objectInformation)
        {
            return objectInformation.Contains("Geode") || objectInformation.Contains("geode");
        }
    }
}
