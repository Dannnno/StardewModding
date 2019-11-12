using Dannnno.StardewMods.Abstraction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Dannnno.StardewMods.UI
{
    public class Game1Graphics : IStardewGraphics
    {
        /// <summary>
        /// Get the size of a sprite's tile
        /// </summary>
        public int TileSheetSpriteSize => 16;

        /// <summary>
        /// Get the tile size in the game window
        /// </summary>
        public int TileSize => Game1.tileSize;

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
        /// Get whether to show the menu's background
        /// </summary>
        public bool ShowMenuBackground => Game1.options.showMenuBackground;

        /// <summary>
        /// Get the texture to use when fading to black
        /// </summary>
        public Texture2D FadeToBlackRectangle => Game1.fadeToBlackRect;

        /// <summary>
        /// Get the cursor texture
        /// </summary>
        public Texture2D MouseCursor => Game1.mouseCursors;

        /// <summary>
        /// Get the bounding box of the game window
        /// </summary>
        public Rectangle Bounds => Game1.graphics.GraphicsDevice.Viewport.Bounds;

        /// <summary>
        /// Get the position of the mouse before this drawing event
        /// </summary>
        public Vector2 PreviousMousePosition => new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY());

        /// <summary>
        /// Get the game's primary sprite sheet
        /// </summary>
        public Texture2D SpriteSheet => Game1.objectSpriteSheet;

        /// <summary>
        /// Get whether the game is using a hardware cursor
        /// </summary>
        public bool UsingHardwareCursor => Game1.options.hardwareCursor;

        /// <summary>
        /// Get the cursor icon
        /// </summary>
        public int CursorIcon => Game1.options.gamepadControls ? 44 : 0;

        /// <summary>
        /// Get the pixel zoom
        /// </summary>
        public int PixelZoom => Game1.pixelZoom;

        /// <summary>
        /// Get the button scale
        /// </summary>
        public float ButtonScale => Game1.dialogueButtonScale;


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

        /// <summary>
        /// Toggles the active menu
        /// </summary>
        /// <typeparam name="T">The type of the menu</typeparam>
        /// <param name="game">The game we're working in</param>
        /// <param name="menuToToggle">The menu to toggle</param>
        public void ToggleActiveMenu<T>(IStardewGame game, T menuToToggle) where T : IClickableMenu
        {
            if (CanOpenMenu)
            {
                Game1.activeClickableMenu = menuToToggle;
            }
            else if (Game1.activeClickableMenu is T)
            {
                Game1.exitActiveMenu();
            }
        }

        /// <summary>
        /// Get the source rectangle for a given sprite
        /// </summary>
        /// <param name="iconId">The id of the sprite</param>
        /// <param name="texture">The tile sheet to pull from</param>
        /// <returns>The source rectangle</returns>
        public Rectangle GetSpriteSourceRectangleForIconFromTileSheet(int iconId, Texture2D texture) => Game1.getSourceRectForStandardTileSheet(texture ?? SpriteSheet, iconId, TileSheetSpriteSize, TileSheetSpriteSize);
    }
}
