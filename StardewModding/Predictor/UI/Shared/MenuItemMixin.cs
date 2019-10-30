using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Dannnno.StardewMods.Predictor.UI
{
    public class MenuItemMixin: OptionsElement
    {
        #region Constants
        protected const int SpriteSize = 64;
        protected const int MenuWidth = 880;
        protected const int FakeTilePositionTemporary = 123;
        #endregion

        #region Properties
        public Rectangle HoverTextBounds { get; internal set; }
        public string HoverText { get; internal set; }

        public IDictionary<string, string> Mappings { get; set; }
        #endregion

        //private int ParentSheetIndex { get; set; }
        //private SObject Item { get; set; }
        private string Name { get; set; }


        public MenuItemMixin() : base("")
        {
            Mappings = new Dictionary<string, string>();
            //Item = obj;
            //ParentSheetIndex = obj.ParentSheetIndex;
            Initialize();
        }

        private void Initialize()
        {
            //Name = Item.DisplayName;
            bounds = new Rectangle(8 * Game1.pixelZoom, 4 * Game1.pixelZoom, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
            //ItemDisplayInfo howToObtain = new ItemDisplayInfo(Item);
            //HoverText = howToObtain.GetItemDisplayInfo();
            HoverTextBounds = new Rectangle();
        }
        
        public virtual void draw(SpriteBatch spriteBatch, int slotX, int slotY, int xPositionOnScreen)
        {
            throw new NotImplementedException();

            int SpriteX = slotX + bounds.X;
            int SpriteY = slotY + bounds.Y;

            int NameX = slotX + bounds.X + (int)(Game1.tileSize * 1.5);
            int TextY = slotY + bounds.Y + Game1.pixelZoom * 3;

            spriteBatch.Draw(Game1.objectSpriteSheet, 
                             new Rectangle(SpriteX, SpriteY, SpriteSize, SpriteSize), 
                             new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, FakeTilePositionTemporary, 16, 16)), 
                             Color.White);

            //if (Item.Stack > 1)
            //{
            //    Utility.drawTinyDigits(Item.Stack, spriteBatch, new Vector2(SpriteX + SpriteSize - 12, SpriteY + SpriteSize - 12), 2f, 0.9f, Color.White);
            //}

            var linebuilder = new StringBuilder();
            foreach (var item in Mappings)
            {
                linebuilder.AppendLine($"{item.Key}: {item.Value}");
            }

            DrawHoverTextBox(spriteBatch, linebuilder.ToString());

            //SpriteText.drawString(spriteBatch, Name, NameX, TextY, 999, -1, 999, 1f, 0.1f, false, -1, "", -1);

            //if (HoverText != "")
            //{
            //    int InfoX = xPositionOnScreen + MenuWidth - 56 - SpriteText.getWidthOfString(StardewModdingAPI.Utilities.GetTranslation("TEXT_TO_HOVER_OVER_FOR_INFO"));

            //    HoverTextBounds.X = InfoX;
            //    HoverTextBounds.Y = TextY;
            //    HoverTextBounds.Width = SpriteText.getWidthOfString(Utilities.GetTranslation("TEXT_TO_HOVER_OVER_FOR_INFO"));
            //    HoverTextBounds.Height = SpriteText.getHeightOfString(Utilities.GetTranslation("TEXT_TO_HOVER_OVER_FOR_INFO"));

            //    SpriteText.drawString(spriteBatch, Utilities.GetTranslation("TEXT_TO_HOVER_OVER_FOR_INFO"), InfoX, TextY);
            //}
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

            position.X += 16;
            position.Y += 16 + 4;
            for (int i = 0; i < lines.Length; ++i)
            {
                b.DrawString(Game1.smallFont, lines[i], position, Game1.textColor);
                position.Y += lineHeight + spaceBetweenLines;
            }
        }

    }
}