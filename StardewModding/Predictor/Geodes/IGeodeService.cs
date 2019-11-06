using Dannnno.StardewMods.Predictor.Shared;
using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.Geodes
{
    /// <summary>
    /// Service that returns geode objects
    /// </summary>
    /// <typeparam name="IProviderType">The provider that can yield the data</typeparam>
    public interface IGeodeService<IProviderType> where IProviderType : IStardewObjectProvider
    {
        /// <summary>
        /// Get the geode objects this service is aware of
        /// </summary>
        /// <param name="provider">The provider that will yield stardew objects to select geodes from</param>
        /// <returns>Enumeration of the known geode kinds</returns>
        public IEnumerable<StardewValley.Object> RetrieveGeodes(IProviderType provider);
    }
}
