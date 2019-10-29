using Dannnno.StardewMods.Predictor.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.UI
{
    public class TabbedMenuItemMixin<TItem> : ClickableMenuMixin where TItem: MenuItemMixin
    {
        #region Constants
        protected const int NumViewablePerPage = 6;
        protected const int ItemBufferSpace = 6;
        #endregion

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
        private Lazy<Point> LazyHeaderBounds => new Lazy<Point>(() => new Point(xPositionOnScreen + Game.TileSize / 4,
                                                                                yPositionOnScreen + Game.TileSize * 5 / 4 + Game1.pixelZoom));
        #endregion

        #region Properties
        protected ClickableTextureComponent UpArrow => LazyUpArrow.Value;
        protected ClickableTextureComponent DownArrow => LazyDownArrow.Value;
        protected ClickableTextureComponent ScrollBar => LazyScrollBar.Value;
        protected Rectangle ScrollBarRunner => LazyScrollBarRunner.Value;
        protected Point HeaderBounds => LazyHeaderBounds.Value;
        protected string TabName { get; private set; }
        protected OptionsElement Header { get; set; }
        protected IList<ClickableComponent> ViewableItems { get; set; }
        public bool Scrolling { get; private set; }
        public int CurrentItemIndex { get; private set; }
        public IList<TItem> Items { get; set; }
        #endregion

        /// <summary>
        /// Create a new tabbed menu item
        /// </summary>
        /// <param name="game"></param>
        public TabbedMenuItemMixin(string tabName, IStardewGame game) : base(game)
        {
            TabName = tabName;
            CurrentItemIndex = 0;
            Items = new List<TItem>();
            ViewableItems = new List<ClickableComponent>();
            Header = new OptionsElement(TabNameToString());

            Initialize();
        }

        /// <summary>
        /// Populate the Items list
        /// </summary>
        protected virtual void PopulateItems()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize the menu page
        /// </summary>
        protected virtual void Initialize()
        {
            var xCoord = xPositionOnScreen + Game.TileSize / 4;
            for (int i = 0; i < NumViewablePerPage; ++i)
            {
                var yCoord = yPositionOnScreen + (Game.TileSize + ItemBufferSpace) * (i + 2);
                var bounds = new Rectangle(xCoord, yCoord, width - Game.TileSize / 2, (height - Game.TileSize * 2) / (NumViewablePerPage + 1) + Game1.pixelZoom);
                var component = new ClickableComponent(bounds, i.ToString())
                {
                    myID = i,
                    upNeighborID = i > 0 ? i + 1 : -7777,
                    downNeighborID = i < NumViewablePerPage - 1 ? i - 1 : -7777,
                    fullyImmutable = true
                };
                ViewableItems.Add(component);
            }
        }

        /// <summary>
        /// Draw the individual menu items
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            PopulateItems();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            Header.draw(spriteBatch, HeaderBounds.X, HeaderBounds.Y);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            for (int i = 0; i < NumViewablePerPage && CurrentItemIndex + i < Items.Count; ++i)
            {
                Items[CurrentItemIndex + i].draw(spriteBatch, ViewableItems[i].bounds.X, ViewableItems[i].bounds.Y, xPositionOnScreen);
            }
            if (!GameMenu.forcePreventClose)
            {
                UpArrow.draw(spriteBatch);
                DownArrow.draw(spriteBatch);
                if (Items.Count > NumViewablePerPage)
                {
                    drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarRunner.X, ScrollBarRunner.Y, ScrollBarRunner.Width, ScrollBarRunner.Height, Color.White, Game1.pixelZoom, false);
                    ScrollBar.draw(spriteBatch);
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (UpArrow.containsPoint(x, y) && CurrentItemIndex > 0)
            {
                UpArrow.scale = UpArrow.baseScale;
                TryToMoveUpOneItem();
                Game1.playSound("shwip");
            }
            else if (DownArrow.containsPoint(x, y) && CurrentItemIndex < Items.Count - NumViewablePerPage)
            {
                DownArrow.scale = DownArrow.baseScale;
                TryMoveDownOneItem();
                Game1.playSound("shwip");
            }
            else if (ScrollBar.containsPoint(x, y))
            {
                Scrolling = true;
            }
            else if (ScrollBarRunner.Contains(x, y))
            {
                Scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            Scrolling = false;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (Scrolling)
            {
                int initialY = ScrollBar.bounds.Y;
                int howManyItemsToMove = Math.Abs(initialY - y) / GetScrollBarDivisionSize();
                if (howManyItemsToMove > 0)
                {
                    if (initialY - y > 0)
                    {
                        //Its moving up to the start of the list
                        TryToMoveUpXItems(howManyItemsToMove);
                    }
                    else
                    {
                        TryToMoveDownXItems(howManyItemsToMove);
                    }
                }
            }
        }

        private void TryToMoveUpXItems(int x)
        {
            if (CurrentItemIndex > x - 1)
            {
                CurrentItemIndex -= x;
            }
            else
            {
                //The user moved so fast that it thinks there's nothing left to scroll
                //e.g trying to move up 4 items when theres only 3 left
                CurrentItemIndex = 0;
            }
            SetScrollBarToCurrentItem();
        }

        private void TryToMoveDownXItems(int x)
        {
            if (CurrentItemIndex < Items.Count - NumViewablePerPage - x + 1)
            {
                CurrentItemIndex += x;
            }
            else
            {
                CurrentItemIndex = Items.Count - NumViewablePerPage;
            }
            SetScrollBarToCurrentItem();
        }

        private void TryToMoveUpOneItem()
        {
            if (CurrentItemIndex > 0)
            {
                CurrentItemIndex -= 1;
                SetScrollBarToCurrentItem();
            }
        }

        private void TryMoveDownOneItem()
        {
            if (CurrentItemIndex < Items.Count - NumViewablePerPage)
            {
                CurrentItemIndex += 1;
                SetScrollBarToCurrentItem();
            }
        }

        private void SetScrollBarToCurrentItem()
        {
            if (CurrentItemIndex == Items.Count - NumViewablePerPage)
            {
                ScrollBar.bounds.Y = DownArrow.bounds.Y - ScrollBar.bounds.Height - Game1.pixelZoom;
            }
            else
            {
                ScrollBar.bounds.Y = GetScrollBarDivisionSize() * CurrentItemIndex + UpArrow.bounds.Bottom + Game1.pixelZoom;
            }
        }

        private int GetScrollBarDivisionSize()
        {
            return (ScrollBarRunner.Height - ScrollBar.bounds.Height) / Math.Max(1, Items.Count - NumViewablePerPage);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0)
            {
                TryToMoveUpOneItem();
            }
            else if (direction < 0)
            {
                TryMoveDownOneItem();
            }
            Game1.playSound("shiny4");
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            HoverText = "";
            for (int i = 0; i < NumViewablePerPage; ++i)
            {
                if (CurrentItemIndex >= 0 && CurrentItemIndex + i < Items.Count && Items[CurrentItemIndex + i].bounds.Contains(x - ViewableItems[i].bounds.X, y - ViewableItems[i].bounds.Y))
                {
                    Game1.SetFreeCursorDrag();
                    break;
                }

            }

            if (ScrollBarRunner.Contains(x, y))
                Game1.SetFreeCursorDrag();
            if (GameMenu.forcePreventClose)
                return;
            UpArrow.tryHover(x, y, 0.1f);
            DownArrow.tryHover(x, y, 0.1f);
            ScrollBar.tryHover(x, y, 0.1f);
            for (int i = 0; i < NumViewablePerPage && CurrentItemIndex + i < Items.Count; ++i)
            {
                if (Items[CurrentItemIndex + i].HoverTextBounds.Contains(x, y))
                {
                    HoverText = Items[CurrentItemIndex + i].HoverText;
                    return;
                }
            }
        }

        private string TabNameToString()
        {
            // TODO: Use Translation APIs
            return TabName;
        }
    }
}