using Dannnno.StardewMods.UI;
using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.UI
{
    public class PredictorMenuV2 : TabbedMenu
    {
        #region Constants
        private static readonly int PredictorBaseComponentId = 26438;
        private const int RottingPlantId = 747;
        private const int GeodeId = 535;
        #endregion

        public PredictorMenuV2(IStardewGraphics graphics) : base(graphics, PredictorBaseComponentId)
        {
            Tabs = new List<object>
            {
                1,
                2,
                3,
                4
            };
            HoverText = "test";
        }

        /// <summary>
        /// Get the icon id for the tab
        /// </summary>
        /// <param name="i">The tab</param>
        /// <returns>The Icon Id</returns>
        protected override int GetTabIconId(int i)
        {
            return i switch
            {
                // TODO: Be smarter about this, don't use constants
                0 => GeodeId,
                _ => RottingPlantId
            };
        }
    }
}
