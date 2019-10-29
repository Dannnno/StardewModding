using Dannnno.StardewMods.Predictor.Geodes;
using Dannnno.StardewMods.Predictor.Shared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace Dannnno.StardewMods.Predictor
{
    /// <summary>
    /// Mod entry point for predictions
    /// </summary>
    public class ModEntry : Mod
    {
        private GeodePredictor predictor;
        private IStardewGame wrapper;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Initialize our tools
            wrapper = new StardewGameWrapper();
            predictor = new GeodePredictor(
                new StardewGeodeService<IStardewObjectProvider>(),
                new StardewDataObjectInfoProvider(helper),
                wrapper,
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
            if (!wrapper.GameIsReady)
                return;

            // P for Predictor
            if (e.Button == SButton.P)
            {
                if (wrapper.CanOpenMenu)
                {
                    OpenMenu();
                }
            }

            predictor.PredictTreasureFromGeode(20);
        }

        private void OpenMenu()
        {
            Game1.activeClickableMenu = new CollectionsPage(50, 50, 100, 100);
        }
    }
}