using StardewModdingAPI;
using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.Shared
{
    /// <summary>
    /// Standard data provider that pulls form the Data/ObjectInformation file
    /// </summary>
    public class StardewDataObjectInfoProvider : IStardewObjectProvider
    {
        /// <summary>
        /// Get the SMAPI helper that can retrieve the xnb content
        /// </summary>
        public IModHelper Helper { get; set; }

        /// <summary>
        /// Create the data provider
        /// </summary>
        /// <param name="helper">With a little help from our friends at SMAPI</param>
        public StardewDataObjectInfoProvider(IModHelper helper)
        {
            Helper = helper;
        }

        /// <summary>
        /// Get the stardew objects from the Data/ObjectInformation.xnb file
        /// </summary>
        /// <returns>The list of objects</returns>
        public IEnumerable<StardewValley.Object> GetStardewObjects()
        {
            foreach (var rawObject in GetStardewRawObjects())
            {
                yield return new StardewValley.Object(rawObject.Key, initialStack: 0, isRecipe: false, price: -1, quality: 0);
            }
        }

        /// <summary>
        /// Gets raw stardew objects from the Data/ObjectInformation file
        /// </summary>
        /// <returns>Raw stardew objects</returns>
        public IDictionary<int, string> GetStardewRawObjects()
        {
            return Helper.Content.Load<Dictionary<int, string>>("Data/ObjectInformation", ContentSource.GameContent);
        }
    }
}
