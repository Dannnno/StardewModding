using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.Shared
{
    /// <summary>
    /// Standard interface by which to get stardew valley objects
    /// </summary>
    public interface IStardewObjectProvider
    {
        /// <summary>
        /// Get the raw stardew objects, from key to delimited string
        /// </summary>
        /// <returns>Mapping from stardew object IDs to raw strings</returns>
        public IDictionary<int, string> GetStardewRawObjects();

        /// <summary>
        /// Get stardew objects in stardew format
        /// </summary>
        /// <returns>Collection of stardew objects</returns>
        public IEnumerable<StardewValley.Object> GetStardewObjects();
    }
}
