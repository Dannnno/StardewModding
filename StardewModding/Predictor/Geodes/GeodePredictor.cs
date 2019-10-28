using Dannnno.StardewMods.Predictor.Shared;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dannnno.StardewMods.Predictor.Geodes
{
    using GeodeServiceType = IGeodeService<IStardewObjectProvider>;
    using StardewObject = StardewValley.Object;
    using StardewList = IList<StardewValley.Object>;
    using LazyStardewList = Lazy<IList<StardewValley.Object>>;

    /// <summary>
    /// Predicts what new geode objects will be found
    /// </summary>
    public class GeodePredictor
    {
        #region fields
        private GeodeServiceType geodeService;
        private IStardewObjectProvider objectProvider;
        private LazyStardewList geodeList;
        #endregion

        #region properties
        /// <summary>
        /// Get or set the service used to retrieve the geodes
        /// </summary>
        public GeodeServiceType GeodeService
        {
            get => geodeService;
            set
            {
                geodeList = getNewLazyGeodeList();
                geodeService = value;
            }
        }

        /// <summary>
        /// Get the list of geodes that this predictor can work on
        /// </summary>
        public StardewList GeodeList { get => geodeList.Value; }

        /// <summary>
        /// Get or set the provider of geode objects
        /// </summary>
        public IStardewObjectProvider ObjectProvider
        {
            get => objectProvider;
            set
            {
                geodeList = getNewLazyGeodeList();
                objectProvider = value;
            }
        }

        /// <summary>
        /// Get or set the associated game
        /// </summary>
        public IStardewGame Game { get; set; }

        /// <summary>
        /// Get or set the associated monitor
        /// </summary>
        public IMonitor Monitor { get; set; }
        #endregion

        /// <summary>
        /// Create a new predictor
        /// </summary>
        /// <param name="service">The service to use to predict geodes</param>
        /// <param name="provider">The provider to retrieve objects from</param>
        /// <param name="game">The game this predictor is associated with</param>
        /// <param name="monitor">The monitor to log to</param>
        public GeodePredictor(GeodeServiceType service, IStardewObjectProvider provider, IStardewGame game, IMonitor monitor = null)
        {
            GeodeService = service;
            ObjectProvider = provider;
            Game = game;
            Monitor = monitor;
        }

        /// <summary>
        /// If the service or provider changes then we need to get the value again, but we still want it to be lazy
        /// </summary>
        /// <returns>Lazy-initialized list of geodes</returns>
        private LazyStardewList getNewLazyGeodeList()
        {
            return new LazyStardewList(() => GeodeService.RetrieveGeodes(ObjectProvider).ToList());
        }

        /// <summary>
        /// Predict the treasures that a geode `distance` items away will return
        /// </summary>
        /// <param name="distance">How far ahead to look</param>
        /// <returns>For each kind of geode we can predict, the associated mapping geode</returns>
        public IEnumerable<IDictionary<StardewObject, StardewObject>> PredictTreasureFromGeode(uint distance = 1)
        {
            // We're going to modify state, make sure we reset it correctly at the end
            using (Game.WithTemporaryChanges(Monitor))
            {
                // How far ahead to calculate
                uint endingValue = Game.GeodeCount + distance;

                for (uint startingValue = Game.GeodeCount; startingValue < endingValue; ++Game.GeodeCount)
                {
                    var results = new Dictionary<StardewObject, StardewObject>();
                    foreach (var geodeKind in GeodeList)
                    {
                        var nextTreasure = StardewValley.Utility.getTreasureFromGeode(geodeKind); // Why copy the method when we can just use the game's?
                        results[geodeKind] = nextTreasure;
                    }
                    yield return results;
                }
            }
        }
    }
}
