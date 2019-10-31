using Dannnno.StardewMods.Predictor.Geodes;
using Dannnno.StardewMods.Predictor.Shared;
using Dannnno.StardewMods.Predictor.UI;
using Dannnno.StardewMods.Predictor.UI.Shared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Dannnno.StardewMods.Predictor
{
    /// <summary>
    /// Mod entry point for predictions
    /// </summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// Get or set the geode predictor
        /// </summary>
        private GeodePredictor GeodePredictor { get; set; }

        /// <summary>
        /// Get or set the game metadata wrapper
        /// </summary>
        private IStardewGame Wrapper { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Initialize our tools
            Wrapper = new StardewGameWrapper();
            GeodePredictor = new GeodePredictor(
                new StardewGeodeService<IStardewObjectProvider>(),
                new StardewDataObjectInfoProvider(helper),
                Wrapper,
                new StardewGeodeCalculator(),
                Monitor
            );

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Wrapper.GameIsReady)
                return;

            // P for Predictor
            // TODO: Make this configurable
            if (e.Button == SButton.P)
            {
                if (Wrapper.CanOpenMenu)
                {
                    OpenMenu();
                }
            }
        }

        /// <summary>
        /// Open the prediction menu
        /// </summary>
        private void OpenMenu()
        {
            Game1.activeClickableMenu = new PredictorMenuV2();
                //new PredictorMenu(Wrapper, GeodePredictor);
        }
    }
}