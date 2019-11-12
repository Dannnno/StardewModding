using System;
using Dannnno.StardewMods.Abstraction;
using Dannnno.StardewMods.Predictor.Geodes;
using Dannnno.StardewMods.Predictor.UI;
using Dannnno.StardewMods.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Dannnno.StardewMods.Predictor
{
    /// <summary>
    /// Mod entry point for predictions
    /// </summary>
    public class ModEntry : Mod
    {
        // P for Predictor
        // TODO: Make this configurable
        private const SButton PredictorMenuButton = SButton.P;

        public bool MenuIsOpen { get; private set; }

        /// <summary>
        /// Get or set the geode predictor
        /// </summary>
        private GeodePredictor GeodePredictor { get; set; }

        /// <summary>
        /// Get or set the game metadata wrapper
        /// </summary>
        private IStardewGame GameWrapper { get; set; }

        /// <summary>
        /// Get or set the graphics wrapper
        /// </summary>
        private IStardewGraphics GraphicsWrapper { get; set; }

        /// <summary>
        /// Get or set the menu to trigger
        /// </summary>
        private PredictorMenuV2 Menu { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Initialize our tools
            GameWrapper = new StardewGameWrapper();
            GraphicsWrapper = new Game1Graphics();
            Menu = new PredictorMenuV2(GraphicsWrapper);
            GeodePredictor = new GeodePredictor(
                new StardewGeodeService<IStardewObjectProvider>(),
                new StardewDataObjectInfoProvider(helper),
                GameWrapper,
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
            if (!GraphicsWrapper.GameIsReady)
            {
                return;
            }

            if (e.Button == PredictorMenuButton)
            {
                GraphicsWrapper.ToggleActiveMenu(GameWrapper, Menu);
            }
        }
    }
}