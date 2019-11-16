using Dannnno.StardewMods.Abstraction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Dannnno.StardewMods.UI
{
    public interface IStardewGraphics
    {
        /// <summary>
        /// Get the size of a sprite's tile
        /// </summary>
        public int TileSheetSpriteSize { get; }

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
        /// Get whether to show the menu's background
        /// </summary>
        public bool ShowMenuBackground { get; }

        /// <summary>
        /// Get the texture to use when fading to black
        /// </summary>
        public Texture2D FadeToBlackRectangle { get; }

        /// <summary>
        /// Get the cursor texture
        /// </summary>
        public Texture2D MouseCursor { get; }

        /// <summary>
        /// Get the bounding box of the game window
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Get the position of the mouse before this drawing event
        /// </summary>
        public Vector2 PreviousMousePosition { get; }

        /// <summary>
        /// Get the game's primary sprite sheet
        /// </summary>
        public Texture2D SpriteSheet { get; }

        /// <summary>
        /// Get whether the game is using a hardware cursor
        /// </summary>
        public bool UsingHardwareCursor { get; }

        /// <summary>
        /// Get the cursor icon
        /// </summary>
        public int CursorIcon { get; }

        /// <summary>
        /// Get the pixel zoom
        /// </summary>
        public int PixelZoom { get; }

        /// <summary>
        /// Get the button scale
        /// </summary>
        public float ButtonScale { get; }

        #region Methods

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
        public void DrawDialogueBox(int x, int y, int width, int height, bool speaker, bool drawOnlyBox, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = false);

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
        public void DrawDialogueBox(Rectangle r, bool speaker = false, bool drawOnlyBox = true, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = false);

        /// <summary>
        /// Toggles the active menu
        /// </summary>
        /// <typeparam name="T">The type of the menu</typeparam>
        /// <param name="game">The game we're working in</param>
        /// <param name="menuToToggle">The menu to toggle</param>
        public void ToggleActiveMenu<T>(IStardewGame game, T menuToToggle) where T : IClickableMenu;

        /// <summary>
        /// Get the source rectangle for a given sprite
        /// </summary>
        /// <param name="iconId">The id of the sprite</param>
        /// <param name="texture">The tile sheet to pull from</param>
        /// <returns>The source rectangle</returns>
        public Rectangle GetSpriteSourceRectangleForIconFromTileSheet(int iconId, Texture2D texture);
        #endregion
    }
}
