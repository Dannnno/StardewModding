using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        /// Get the width of the game window
        /// </summary>
        public int WindowWidth => Game1.viewport.Width;

        /// <summary>
        /// Get the height of the game window
        /// </summary>
        public int WindowHeight => Game1.viewport.Height;

        /// <summary>
        /// Get the tile size in the game window
        /// </summary>
        public int TileSize => Game1.tileSize;

        /// <summary>
        /// Get whether to show the menu's background
        /// </summary>
        public bool ShowMenuBackground => Game1.options.showMenuBackground;

        /// <summary>
        /// Get the texture to use when fading to black
        /// </summary>
        public Texture2D FadeToBlackRectangle => Game1.fadeToBlackRect;

        /// <summary>
        /// Get the bounding box of the game window
        /// </summary>
        public Rectangle Bounds => Game1.graphics.GraphicsDevice.Viewport.Bounds;


        /// <summary>
        /// Create a context in which changes to game state are rolled back at the end of
        /// </summary>
        /// <param name="monitor">The monitor to which we should be logging messages, if that is desired</param>
        /// <returns>Disposable context manager</returns>
        public IDisposable WithTemporaryChanges(IMonitor monitor = null)
        {
            return new GameStateContextManager(this, monitor);
        }

        /// <summary>
        /// Draw a dialogue box
        /// </summary>
        /// <param name="x">X position of the box</param>
        /// <param name="y">Y position of the box</param>
        /// <param name="width">The width of the box</param>
        /// <param name="height">The height of the box</param>
        /// <param name="speaker">Whether to include the speaker</param>
        /// <param name="drawOnlyBox">Whether to only draw the box</param>
        /// <param name="message">The message</param>
        /// <param name="objectDialogueWithPortrait">Whether to include the pportrait</param>
        /// <param name="ignoreTitleSafe">Whether to ignore the title</param>
        public void DrawDialogueBox(int x, int y, int width, int height, bool speaker, bool drawOnlyBox, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = false)
        {
            Game1.drawDialogueBox(x, y, width, height, speaker, drawOnlyBox, message, objectDialogueWithPortrait, ignoreTitleSafe);
        }

        /// <summary>
        /// Draw a dialogue box
        /// </summary>
        /// <param name="r">Bounding box</param>
        /// <param name="speaker">Whether to include the speaker</param>
        /// <param name="drawOnlyBox">Whether to only draw the box</param>
        /// <param name="message">The message</param>
        /// <param name="objectDialogueWithPortrait">Whether to include the pportrait</param>
        /// <param name="ignoreTitleSafe">Whether to ignore the title</param>
        /// <summary>
        public void DrawDialogueBox(Rectangle r, bool speaker = false, bool drawOnlyBox = true, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = false)
        {
            DrawDialogueBox(r.X, r.Y, r.Width, r.Height, speaker, drawOnlyBox, message, objectDialogueWithPortrait, ignoreTitleSafe);
        }
    }
}
