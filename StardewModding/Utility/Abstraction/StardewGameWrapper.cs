using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace Dannnno.StardewMods.Abstraction
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
