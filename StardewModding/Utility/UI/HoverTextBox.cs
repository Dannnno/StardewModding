using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace Dannnno.StardewMods.UI.Shared
{
    /// <summary>
    /// Class that represents some hover text
    /// </summary>
    public class HoverTextBox : IClickableMenu
    {
        private const int BoxOffsetFromCursor = 32;

        private Lazy<int> LazyLineHeight => new Lazy<int>(() => (int)Game1.smallFont.MeasureString("ABC").Y);
        private Lazy<Vector2> LazyTextBoxDimensions { get; set; }

        private int LineHeight => LazyLineHeight.Value;

        private string hoverText;
        /// <summary>
        /// Get or set the hover text
        /// </summary>
        public string HoverText
        {
            get => hoverText;
            set
            {
                if (hoverText != value)
                {
                    LazyTextBoxDimensions = new Lazy<Vector2>(getTextBoxDimensions);
                    hoverText = value;
                }
            }
        }

        /// <summary>
        /// Get the individual lines of hover text
        /// </summary>
        public string[] HoverTextLines => HoverText?.Split('\n');

        /// <summary>
        /// Get or set the amount of space to go between the lines
        /// </summary>
        public int SpaceBetweenLines { get; set; }

        public Vector2 TextBoxDimensions => LazyTextBoxDimensions.Value;

        /// <summary>
        /// Create a hovering text box
        /// </summary>
        /// <param name="text">The text in the box</param>
        /// <param name="spaceBetweenLines">How much space between lines</param>
        public HoverTextBox(string text = null, int spaceBetweenLines = 4)
        {
            HoverText = text;
            SpaceBetweenLines = spaceBetweenLines;
        }

        /// <summary>
        /// Draw the hover box
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            // If we don't have hover text, then it doesn't matter
            if (string.IsNullOrWhiteSpace(HoverText))
            {
                return;
            }

            // The box goes slightly down and to the right of the cursor by default
            var position = new Vector2(Game1.getOldMouseX() + BoxOffsetFromCursor, Game1.getOldMouseY() + BoxOffsetFromCursor);
            
            // If we would go off the screen, move the text box so it stays on screen
            if (IsGoingOutOfXRightView((int)position.X, (int)TextBoxDimensions.X))
            {
                position.X = Game1.viewport.Width - TextBoxDimensions.X;
            }
            if (IsGoingOutOfYDownView((int)position.Y, (int)TextBoxDimensions.Y))
            {
                if (IsGoingOutOfYUpView((int)position.Y, (int)TextBoxDimensions.Y))
                {
                    position.Y = GetBestY((int)position.Y, (int)TextBoxDimensions.Y);
                }
                else
                {
                    position.Y = Game1.getOldMouseY() - TextBoxDimensions.Y;
                }
            }

            // Draw the box
            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)position.X, (int)position.Y, (int)TextBoxDimensions.X, (int)TextBoxDimensions.Y, Color.White);

            // The contents of the box should be slightly offset
            position.X += 16;
            position.Y += 16 + 4;

            // Draw each line of the string
            for (var i = 0; i < HoverTextLines.Length; ++i)
            {
                b.DrawString(Game1.smallFont, HoverTextLines[i], position, Game1.textColor);
                position.Y += LineHeight + SpaceBetweenLines;
            }
        }

        /// <summary>
        /// Get the dimensions of the text box
        /// </summary>
        /// <returns>The dimensions of the text box</returns>
        private Vector2 getTextBoxDimensions()
        {
            return new Vector2(
                Game1.smallFont.MeasureString(HoverText).X // The size of the string
                + BoxOffsetFromCursor // Plus our offset
                + (HoverTextLines.Length - 1) * SpaceBetweenLines // Plus padding between the lines
                + 4, // Plus a little magic
                HoverTextLines.Length * LineHeight // Number of rows times height per row
                + BoxOffsetFromCursor // Plus our offset
            );
        }

        /// <summary>
        /// Determine whether a value would go off the right side of the screen
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="width">The width of the menu</param>
        /// <returns>Whether the value is going off the right side</returns>
        private bool IsGoingOutOfXRightView(int x, int width)
        {
            return x + width > Game1.viewport.Width;
        }

        /// <summary>
        /// Determine whether a value would go off the bottom of the screen
        /// </summary>
        /// <param name="y">The y position</param>
        /// <param name="height">The height of the menu</param>
        /// <returns>Whether the value is going off the bottom</returns>
        private bool IsGoingOutOfYDownView(int y, int height)
        {
            return y + height > Game1.viewport.Height;
        }

        /// <summary>
        /// Determine whether a value would go off the top of the menu
        /// </summary>
        /// <param name="y">The y position</param>
        /// <param name="height">The height of the menu</param>
        /// <returns>Whether the value is going off the top</returns>
        private bool IsGoingOutOfYUpView(int y, int height)
        {
            return y - height < 0;
        }

        /// <summary>
        /// Get the best Y value to place a hover box
        /// </summary>
        /// <param name="y">Current Y position</param>
        /// <param name="height">Height of the menu</param>
        /// <returns>Best y position of the hover box</returns>
        private int GetBestY(int y, int height)
        {
            int overDown = Math.Abs(y + height - Game1.viewport.Height);
            int overUp = Math.Abs(y - (Game1.viewport.Height - height));

            return overDown > overUp ? Game1.getOldMouseY() - height : y;
        }
    }
}
