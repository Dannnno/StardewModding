using Dannnno.StardewMods.Predictor.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace Dannnno.StardewMods.Predictor.UI
{
    public class ClickableMenuMixin : IClickableMenu
    {
        /// <summary>
        /// Get the game associated with the menu
        /// </summary>
        public IStardewGame Game { get; protected set; }

        /// <summary>
        /// Get or set the current hover text of the menu
        /// </summary>
        public string HoverText { get; set; }

        /// <summary>
        /// Get whether the menu is currently invisible
        /// </summary>
        public bool Invisible => false;

        /// <summary>
        /// Get whether the menu should be prevented from closing
        /// </summary>
        public bool ForcePreventClose => false;

        /// <summary>
        /// Initialize the menu
        /// </summary>
        /// <param name="game">The game the menu is for</param>
        public ClickableMenuMixin(IStardewGame game) : base(CalculateMenuXPosition(game),
                                                            CalculateMenuYPosition(game),
                                                            CalculateMenuWidth(),
                                                            CalculateMenuHeight(),
                                                            ShowCloseButton())
        {
            Game = game;
            HoverText = "";
        }
        
        /// <summary>
        /// Determine whether a value would go off the right side of the menu
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="width">The width of the menu</param>
        /// <returns>Whether the value is going off the right side</returns>
        protected bool IsGoingOutOfXRightView(int x, int width)
        {
            return x + width > Game1.viewport.Width;
        }

        /// <summary>
        /// Determine whether a value would go off the bottom of the menu
        /// </summary>
        /// <param name="y">The y position</param>
        /// <param name="height">The height of the menu</param>
        /// <returns>Whether the value is going off the bottom</returns>
        protected bool IsGoingOutOfYDownView(int y, int height)
        {
            return y + height > Game1.viewport.Height;
        }

        /// <summary>
        /// Determine whether a value would go off the top of the menu
        /// </summary>
        /// <param name="y">The y position</param>
        /// <param name="height">The height of the menu</param>
        /// <returns>Whether the value is going off the top</returns>
        protected bool IsGoingOutOfYUpView(int y, int height)
        {
            return y - height < 0;
        }

        /// <summary>
        /// Get the best Y value to place a hover box
        /// </summary>
        /// <param name="y">Current Y position</param>
        /// <param name="height">Height of the menu</param>
        /// <returns>Best y position of the hover box</returns>
        protected int GetBestY(int y, int height)
        {
            int overDown = Math.Abs(y + height - Game1.viewport.Height);
            int overUp = Math.Abs(y - (Game1.viewport.Height - height));

            return overDown > overUp ? Game1.getOldMouseY() - height : y;
        }

        /// <summary>
        /// Draws a text box for hover text
        /// </summary>
        /// <param name="b">The sprite batch to use</param>
        /// <param name="text">The text to put in the text box</param>
        /// <param name="spaceBetweenLines">How much space to put in between the lines</param>
        protected void DrawHoverTextBox(SpriteBatch b, string text = null, int spaceBetweenLines = 4)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = HoverText;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            string[] lines = text.Split('\n');
            int lineHeight = (int)Game1.smallFont.MeasureString("ABC").Y;
            Vector2 position = new Vector2(Game1.getOldMouseX() + 32, Game1.getOldMouseY() + 32);
            Vector2 boxDimensions = new Vector2(Game1.smallFont.MeasureString(text).X, lines.Length * lineHeight);

            boxDimensions.Y += 32 + (lines.Length - 1) * spaceBetweenLines + 4;
            boxDimensions.X += 32;

            if (IsGoingOutOfXRightView((int)position.X, (int)boxDimensions.X))
            {
                position.X = Game1.viewport.Width - boxDimensions.X;
            }
            if (IsGoingOutOfYDownView((int)position.Y, (int)boxDimensions.Y))
            {
                if (IsGoingOutOfYUpView((int)position.Y, (int)boxDimensions.Y))
                {
                    position.Y = GetBestY((int)position.Y, (int)boxDimensions.Y);
                }
                else
                {
                    position.Y = Game1.getOldMouseY() - boxDimensions.Y;
                }
            }

            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)position.X, (int)position.Y, (int)boxDimensions.X, (int)boxDimensions.Y, Color.White);

            position.X += 16;
            position.Y += 16 + 4;
            for (int i = 0; i < lines.Length; ++i)
            {
                b.DrawString(Game1.smallFont, lines[i], position, Game1.textColor);
                position.Y += lineHeight + spaceBetweenLines;
            }
        }

        /// <summary>
        /// Get whether to show the close button
        /// </summary>
        /// <returns>True</returns>
        protected static bool ShowCloseButton()
        {
            return true;
        }

        /// <summary>
        /// Calculate the width of the menu
        /// </summary>
        /// <returns>The width of the menu</returns>
        protected static int CalculateMenuWidth()
        {
            return 800 + borderWidth * 2;
        }

        /// <summary>
        /// Calculate the height of the menu
        /// </summary>
        /// <returns>The height of the menu</returns>
        protected static int CalculateMenuHeight()
        {
            return 600 + borderWidth * 2;
        }

        /// <summary>
        /// Calculate the upper left hand corner x position of the menu
        /// </summary>
        /// <param name="game">The game whose menu position we're calculating</param>
        /// <returns>The x position</returns>
        protected static int CalculateMenuXPosition(IStardewGame game)
        {
            return game.WindowWidth / 2 - CalculateMenuWidth() / 2;
        }

        /// <summary>
        /// Calculate the upper left hand corner y position of the menu
        /// </summary>
        /// <param name="game">The game whose menu position we're calculating</param>
        /// <returns>The y position</returns>
        protected static int CalculateMenuYPosition(IStardewGame game)
        {
            return game.WindowHeight / 2 - CalculateMenuHeight() / 2;
        }
    }
}
