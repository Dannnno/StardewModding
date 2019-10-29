using Dannnno.StardewMods.Predictor.Geodes;
using Dannnno.StardewMods.Predictor.Shared;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dannnno.StardewMods.Predictor.UI
{
    /// <summary>
    /// Menu class for individual tabs
    /// </summary>
    /// <remarks>
    /// Heavily borrowed from/adapted from https://github.com/LukeSeewald/PublicStardewValleyMods
    /// </remarks>
    internal class GeodeMenuTab : TabbedMenuItemMixin<GeodeMenuItem>
    {
        #region Constants
        private const int DefaultSearchDistance = 20;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set cached predictions
        /// </summary>
        private IDictionary<StardewValley.Object, IDictionary<uint, StardewValley.Object>> CachedPredictions { get; set; }

        /// <summary>
        /// Get the current geode count
        /// </summary>
        private uint GeodeCount => Game.GeodeCount;

        private uint distance;
        /// <summary>
        /// Get or set the distance to search ahead
        /// </summary>
        public uint Distance
        {
            get => distance;
            set
            {
                // When it changes, start filling our cache
                PopulateItems();
                distance = value;
            }
        }

        private GeodePredictor predictor;
        /// <summary>
        /// The predictor we're going to use
        /// </summary>
        public GeodePredictor Predictor
        {
            get => predictor;
            set
            {
                // Changing the predictor invalidates the existing cache
                CachedPredictions = new Dictionary<StardewValley.Object, IDictionary<uint, StardewValley.Object>>();
                predictor = value;
            }
        }
        #endregion

        public GeodeMenuTab(IStardewGame game, GeodePredictor geodePredictor, uint distance = DefaultSearchDistance) : base("Geodes", game)
        {
            CachedPredictions = new Dictionary<StardewValley.Object, IDictionary<uint, StardewValley.Object>>();
            Predictor = geodePredictor;
            Distance = distance;
        }

        protected override void PopulateItems()
        {
            CachedPredictions ??= new Dictionary<StardewValley.Object, IDictionary<uint, StardewValley.Object>>();

            var firstMissing = FirstMissingKeyInRange(Distance);

            // If we're missing some records, get them
            if (firstMissing.HasValue)
            {
                uint i = GeodeCount + Math.Max(firstMissing.Value, DefaultSearchDistance);
                foreach (var searchResults in Predictor.PredictTreasureFromGeode(i))
                {
                    foreach (var geodeKind in searchResults.Keys)
                    {
                        if (!CachedPredictions.ContainsKey(geodeKind))
                        {
                            CachedPredictions[geodeKind] = new Dictionary<uint, StardewValley.Object>();
                        }
                        CachedPredictions[geodeKind][i] = searchResults[geodeKind];
                    }
                }
            }

            // Now that we have records, create our actual menu items
            foreach (var geodeKind in CachedPredictions)
            {
                foreach (var prediction in geodeKind.Value)
                {
                    Items.Add(new GeodeMenuItem(geodeKind.Key.ParentSheetIndex.ToString(), 
                                                new[] { $"{prediction.Key}: {prediction.Value.ParentSheetIndex}" }.ToList()));
                }
            }
        }

        /// <summary>
        /// Determine if a given item is in the cache
        /// </summary>
        /// <param name="searchDistance">How far ahead to look</param>
        /// <returns>Whether the item at that search distance is in the cache</returns>
        private bool KeyIsInCache(uint searchDistance)
        {
            // If we don't have enough geode kinds in the cache, then we don't have it
            if (CachedPredictions.Count != Predictor.GeodeList.Count || CachedPredictions.Count == 0)
            {
                return false;
            }

            // We actually want to start from the current count, so we are able to leverage old predictions
            var modifiedKey = searchDistance + GeodeCount;
            foreach (var geodeKind in CachedPredictions.Keys)
            {
                if (!CachedPredictions[geodeKind].ContainsKey(modifiedKey))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Find the first item we do not have cached within a certain range
        /// </summary>
        /// <param name="searchDistance">How far ahead to look</param>
        /// <returns>The first distance we don't have</returns>
        private uint? FirstMissingKeyInRange(uint searchDistance)
        {
            for (var start = 0; start < searchDistance; ++start)
            {
                if (!KeyIsInCache(searchDistance))
                {
                    return searchDistance;
                }
            }

            return null;
        }
    }
}