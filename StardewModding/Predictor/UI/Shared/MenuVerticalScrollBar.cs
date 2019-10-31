using Dannnno.StardewMods.Predictor.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace Dannnno.StardewMods.Predictor.UI.Shared
{
    public class MenuVerticalScrollBar : IClickableMenu
    {
        #region Lazy Fields
        private Lazy<ClickableTextureComponent> LazyUpArrow => new Lazy<ClickableTextureComponent>(() => new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game.TileSize / 4,
                                                                                                                                                     yPositionOnScreen + Game.TileSize,
                                                                                                                                                     11 * Game1.pixelZoom,
                                                                                                                                                     12 * Game1.pixelZoom),
                                                                                                                                       Game1.mouseCursors,
                                                                                                                                       new Rectangle(421, 459, 11, 12),
                                                                                                                                       Game1.pixelZoom,
                                                                                                                                       false));
        private Lazy<ClickableTextureComponent> LazyDownArrow => new Lazy<ClickableTextureComponent>(() => new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game.TileSize / 4,
                                                                                                                                                       yPositionOnScreen + height - Game.TileSize,
                                                                                                                                                       11 * Game1.pixelZoom,
                                                                                                                                                       12 * Game1.pixelZoom),
                                                                                                                                         Game1.mouseCursors,
                                                                                                                                         new Rectangle(421, 472, 11, 12),
                                                                                                                                         Game1.pixelZoom,
                                                                                                                                         false));
        private Lazy<ClickableTextureComponent> LazyScrollBar => new Lazy<ClickableTextureComponent>(() => new ClickableTextureComponent(new Rectangle(UpArrow.bounds.X + Game1.pixelZoom * 3,
                                                                                                                                                       UpArrow.bounds.Y + UpArrow.bounds.Height + Game1.pixelZoom,
                                                                                                                                                       6 * Game1.pixelZoom,
                                                                                                                                                       10 * Game1.pixelZoom),
                                                                                                                                         Game1.mouseCursors,
                                                                                                                                         new Rectangle(435, 463, 6, 10),
                                                                                                                                         Game1.pixelZoom,
                                                                                                                                         false));
        private Lazy<Rectangle> LazyScrollBarRunner => new Lazy<Rectangle>(() => new Rectangle(ScrollBar.bounds.X,
                                                                                               UpArrow.bounds.Y + UpArrow.bounds.Height + Game1.pixelZoom,
                                                                                               ScrollBar.bounds.Width,
                                                                                               height - Game.TileSize * 2 - UpArrow.bounds.Height - Game1.pixelZoom * 2));
        #endregion

        #region Properties
        protected ClickableTextureComponent UpArrow => LazyUpArrow.Value;
        protected ClickableTextureComponent DownArrow => LazyDownArrow.Value;
        protected ClickableTextureComponent ScrollBar => LazyScrollBar.Value;
        protected Rectangle ScrollBarRunner => LazyScrollBarRunner.Value;
        public IStardewGame Game { get; set; }
        #endregion

        public MenuVerticalScrollBar(IStardewGame game)
        {
            Game = game;
        }


        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            UpArrow.draw(spriteBatch);
            DownArrow.draw(spriteBatch);
            drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarRunner.X, ScrollBarRunner.Y, ScrollBarRunner.Width, ScrollBarRunner.Height, Color.White, Game1.pixelZoom, false);
            ScrollBar.draw(spriteBatch);
        }
    }
}
