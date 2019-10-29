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
        /// Get the current geode count
        /// </summary>
        private uint GeodeCount => Game.GeodeCount;

        /// <summary>
        /// Get or set the distance to search ahead
        /// </summary>
        public uint Distance { get; set; }

        /// <summary>
        /// The predictor we're going to use
        /// </summary>
        public GeodePredictor Predictor { get; set; }
        #endregion

        public GeodeMenuTab(IStardewGame game, GeodePredictor geodePredictor, uint distance = DefaultSearchDistance) : base("Geodes", game)
        {
            Predictor = geodePredictor;
            Distance = distance;
        }

        protected override void PopulateItems()
        {
            uint start = GeodeCount < Distance ? 0 : GeodeCount - Distance;
            uint end = GeodeCount + Distance;

            // Now that we have records, create our actual menu items
            foreach (var predictionIndex in Predictor.PredictTreasureFromGeodesInRange(start, end))
            {
                foreach (var geodeKind in predictionIndex)
                {
                    Items.Add(new GeodeMenuItem(geodeKind.Key.ParentSheetIndex.ToString(),
                                                $"{start}: {geodeKind.Value.ParentSheetIndex}"));
                }
                ++start;
            }
        }

    }
}