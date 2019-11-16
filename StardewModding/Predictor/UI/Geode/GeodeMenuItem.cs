using Dannnno.StardewMods.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.UI
{
    internal class GeodeMenuItem: MenuTabItem
    {
        internal IList<GeodeResultModel> Data { get; set; }

        public GeodeMenuItem(IList<GeodeResultModel> data) : base()
        {
            Data = data;
        }

        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, int xPositionOnScreen)
        {
            int spriteX = slotX + bounds.X;
            int spriteY = slotY + bounds.Y;

            int NameX = slotX + bounds.X + (int)(Game1.tileSize * 1.5);
            int TextY = slotY + bounds.Y + Game1.pixelZoom * 3;

            spriteBatch.Draw(Game1.objectSpriteSheet, new Rectangle(spriteX, spriteY, SpriteSize, SpriteSize), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Data[0].TreasureId, 16, 16)), Color.White);
            
            SpriteText.drawString(spriteBatch, Data[0].DisplayName, NameX, TextY, 999, -1, 999, 1f, 0.1f, false, -1, "", -1);

            var hoverHelpText = "Hover";
            if (!string.IsNullOrWhiteSpace(Data[0].HoverText))
            {
                int InfoX = xPositionOnScreen + MenuWidth - 56 - SpriteText.getWidthOfString(hoverHelpText);

                HoverTextBounds = new Rectangle(InfoX, TextY, SpriteText.getWidthOfString(hoverHelpText), SpriteText.getHeightOfString(hoverHelpText));

                SpriteText.drawString(spriteBatch, hoverHelpText, InfoX, TextY);
            }
        }
    }
}