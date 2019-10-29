using StardewModdingAPI;
using System;

namespace Dannnno.StardewMods.Predictor.Shared
{
    public interface IStardewGame
    {
        /// <summary>
        /// Get the number of geodes that have already been cracked by the player
        /// </summary>
        [StardewManagedProperty]
        public uint GeodeCount { get; set; }


        /// <summary>
        /// Get whether a menu can be opened
        /// </summary>
        public bool CanOpenMenu { get; }

        /// <summary>
        /// Get whether the game is ready for mods to interact
        /// </summary>
        public bool GameIsReady { get; }

        /// <summary>
        /// Get the width of the game window
        /// </summary>
        public int WindowWidth { get; }

        /// <summary>
        /// Get the height of the game window
        /// </summary>
        public int WindowHeight { get; }

        /// <summary>
        /// Get the tile size in the game window
        /// </summary>
        public int TileSize { get; }

        /// <summary>
        /// Create a context in which changes to game state are rolled back at the end of
        /// </summary>
        /// <param name="monitor">The monitor to which we should be logging messages, if that is desired</param>
        /// <returns>Disposable context manager</returns>
        public IDisposable WithTemporaryChanges(IMonitor monitor = null);
    }
}
