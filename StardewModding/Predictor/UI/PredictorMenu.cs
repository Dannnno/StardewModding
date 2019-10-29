using Dannnno.StardewMods.Predictor.Geodes;
using Dannnno.StardewMods.Predictor.Shared;

namespace Dannnno.StardewMods.Predictor.UI
{
    #region Internal bits

    /// <summary>
    /// The tabs that the predictor UI supports
    /// </summary>
    public enum PredictorTabNameEnum : int
    {
        Geode = 0
    }

    #endregion

    /// <summary>
    /// Menu class for the predictor app
    /// </summary>
    /// <remarks>
    /// Heavily borrowed from/adapted from https://github.com/LukeSeewald/PublicStardewValleyMods
    /// </remarks>
    public class PredictorMenu : TabbedMenuMixin<PredictorTabNameEnum>
    {
        private static readonly int StartingMenuId = 26438; // Randomly chosen, guaranteed fair

        /// <summary>
        /// Create the predictor menu
        /// </summary>
        /// <param name="game">The game we're creating the menu for</param>
        public PredictorMenu(IStardewGame game, GeodePredictor geodePredictor) : base(game, StartingMenuId)
        {
            InitializeSubMenus(geodePredictor);
            CurrentTab = (int)PredictorTabNameEnum.Geode;
        }

        protected override int GetIconId(PredictorTabNameEnum tab)
        {
            return tab switch
            {
                // TODO: Make these not constants
                PredictorTabNameEnum.Geode => 535, // Geode
                _ => 747 // Rotting plant
            };
        }


        private void InitializeSubMenus(GeodePredictor geodePredictor)
        {
            // Geode
            Tabs.Add(new MenuTuple()
            {
                Component = GetTabComponent(PredictorTabNameEnum.Geode),
                Menu = new GeodeMenuTab(Game, geodePredictor),
                IconId = GetIconId(PredictorTabNameEnum.Geode)
            });
        }
    }
}
