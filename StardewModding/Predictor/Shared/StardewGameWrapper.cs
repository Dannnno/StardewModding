using StardewModdingAPI;
using StardewValley;
using System;

namespace Dannnno.StardewMods.Predictor.Shared
{
    /// <summary>
    /// Encapsulates StardewValley's Game1
    /// </summary>
    public class StardewGameWrapper : IStardewGame
    {
        /// <summary>
        /// Get the number of geodes that have already been cracked by the player
        /// </summary>
        [StardewManagedProperty]
        public uint GeodeCount
        {
            get { return Game1.stats.GeodesCracked; }
            set { Game1.stats.GeodesCracked = value; }
        }

        /// <summary>
        /// Get whether a menu can be opened
        /// </summary>
        public bool CanOpenMenu => Game1.activeClickableMenu == null;

        /// <summary>
        /// Get whether the game is ready for mods to interact
        /// </summary>
        public bool GameIsReady => Context.IsWorldReady;

        /// <summary>
        /// Create a context in which changes to game state are rolled back at the end of
        /// </summary>
        /// <param name="monitor">The monitor to which we should be logging messages, if that is desired</param>
        /// <returns>Disposable context manager</returns>
        public IDisposable WithTemporaryChanges(IMonitor monitor = null)
        {
            return new GameStateContextManager(this, monitor);
        }
    }
}
