using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dannnno.StardewMods.UI
{
    /// <summary>
    /// Generic tabbed menu class
    /// </summary>
    public class TabbedMenu : IClickableMenu
    {
        #region Constants
        private static readonly bool ShowCloseButton = true;
        private static readonly int MenuWidth = 800;
        private static readonly int MenuHeight = 600;
        private const int SelectedTabVerticalOffset = 8;
        private const float NoRotation = 0.0f;
        private const float TabBackgroundScale = 4f;
        private const float TabDepth = 0.0001f;
        private const float TabSpriteScale = 2.5f;
        private const float TabSpriteDepth = 0.9f;
        private const float CursorDepth = 1f;
        #endregion

        #region Properties
        // We don't need to recalculate these every time
        private static Lazy<Rectangle> LazyTabCursorRectangle => new Lazy<Rectangle>(() => new Rectangle(16, 368, 16, 16));
        private static Lazy<Vector2> LazyTabSpriteOrigin => new Lazy<Vector2>(() => new Vector2(4f, 4f));
        private Lazy<Rectangle> LazyMenuBounds => new Lazy<Rectangle>(() => new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height));

        private int TabXSpriteOffset => (int)(Graphics.TileSize / 3.25) + 3;
        private int TabYSpriteOffset => (int)(Graphics.TileSize / 2.25);

        private static Rectangle TabBackgroundRectangle => LazyTabCursorRectangle.Value;
        private static Vector2 TabSpriteOrigin => LazyTabSpriteOrigin.Value;

        private Rectangle MenuBounds => LazyMenuBounds.Value;

        private float CursorScale => Graphics.PixelZoom + Graphics.ButtonScale / 150f;

        private Rectangle CursorSource => Graphics.GetSpriteSourceRectangleForIconFromTileSheet(Graphics.CursorIcon, Graphics.MouseCursor);

        private readonly HoverTextBox HoverTextBox = new HoverTextBox();

        /// <summary>
        /// Get or set the tabs in the menu
        /// </summary>
        protected IList<object> Tabs { get; set; }

        private string hoverText;
        /// <summary>
        /// Get or set the hover text to display
        /// </summary>
        public string HoverText
        {
            get => hoverText;
            set
            {
                HoverTextBox.HoverText = value;
                hoverText = value;
            }
        }

        /// <summary>
        /// Get the current tab
        /// </summary>
        public int CurrentTabId { get; private set; } = 0;

        /// <summary>
        /// Get the graphics object
        /// </summary>
        public IStardewGraphics Graphics { get; }

        /// <summary>
        /// Get the base component id
        /// </summary>
        public int BaseComponentId { get; }
        #endregion

        /// <summary>
        /// Create a tabbed menu
        /// </summary>
        /// <param name="graphics">Graphics context to use to create it</param>
        /// <param name="baseComponentId">The base component id to use</param>
        public TabbedMenu(IStardewGraphics graphics, int baseComponentId) : base(CalculateMenuXPosition(),
                                                                                 CalculateMenuYPosition(),
                                                                                 CalculateMenuWidth(),
                                                                                 CalculateMenuHeight(),
                                                                                 ShowCloseButton)
        {
            Graphics = graphics;
            BaseComponentId = baseComponentId;
        }

        /// <summary>
        /// Draw the tabbed menu
        /// </summary>
        /// <param name="b">Batch to use</param>
        public override void draw(SpriteBatch b)
        {
            DrawBackground(b);
            DrawClickableTabs(b);
            DrawMouseAndHoverBox(b);
        }

        /// <summary>
        /// Draw the background (modal and menu)
        /// </summary>
        /// <param name="b">Batch to use</param>
        private void DrawBackground(SpriteBatch b)
        {
            // Whatever the current batch is works

            // Draw the modal backdrop to make the menu pop
            if (!Graphics.ShowMenuBackground)
            {
                b.Draw(
                    Graphics.FadeToBlackRectangle,
                    Graphics.Bounds,
                    Color.Black * 0.4f
                );
            }

            // Now draw out the menu itself 
            Graphics.DrawDialogueBox(MenuBounds);
            b.End();
        }

        /// <summary>
        /// Draw the individual tabs
        /// </summary>
        /// <param name="b">Batch to use</param>
        private void DrawClickableTabs(SpriteBatch b)
        {
            for (var i = 0; i < Tabs.Count; ++i)
            {
                b.Begin(SpriteSortMode.FrontToBack,
                        BlendState.NonPremultiplied,
                        SamplerState.PointClamp,
                        null,
                        null);

                var currentComponent = GetTabComponent(i);
                var iconId = GetTabIconId(i);

                // Draw the tab background
                b.Draw(Graphics.MouseCursor,
                       GetBackgroundBoundsFromComponent(i, currentComponent),
                       TabBackgroundRectangle,
                       Color.White,
                       NoRotation,
                       Vector2.Zero,
                       TabBackgroundScale,
                       SpriteEffects.None,
                       TabDepth);

                // Draw the sprite we want on the tab
                b.Draw(Graphics.SpriteSheet,
                       GetSpriteBoundsFromComponent(i, currentComponent),
                       Graphics.GetSpriteSourceRectangleForIconFromTileSheet(iconId, Graphics.SpriteSheet),
                       Color.White,
                       NoRotation,
                       TabSpriteOrigin,
                       TabSpriteScale,
                       SpriteEffects.None,
                       TabSpriteDepth);

                b.End();
            }
        }

        /// <summary>
        /// Get the icon id for the tab
        /// </summary>
        /// <param name="i">The tab</param>
        /// <returns>The Icon Id</returns>
        protected virtual int GetTabIconId(int i) => throw new NotImplementedException();

        /// <summary>
        /// Get the bounding box of the tab's background
        /// </summary>
        /// <param name="tabId">The tab we're looking for</param>
        /// <param name="component">That tab's component</param>
        /// <returns>The bounding box</returns>
        private Vector2 GetBackgroundBoundsFromComponent(int tabId, ClickableComponent component)
        {
            var selectedTabVerticalOffset = GetTabVerticalOffset(tabId);
            return new Vector2(component.bounds.X, component.bounds.Y + selectedTabVerticalOffset);
        }

        /// <summary>
        /// Get the bounding box of the tab's sprite icon
        /// </summary>
        /// <param name="tabId">The tab we're looking for</param>
        /// <param name="component">That tab's component</param>
        /// <returns>The bounding box</returns>
        private Vector2 GetSpriteBoundsFromComponent(int tabId, ClickableComponent component)
        {
            var componentBounds = GetBackgroundBoundsFromComponent(tabId, component);
            return new Vector2(componentBounds.X + TabXSpriteOffset, componentBounds.Y + TabYSpriteOffset);
        }

        /// <summary>
        /// If the given tab is the currently selected tab, then we will shift it down by `SelectedTabVerticalOffset`
        /// to indicate that it is selected
        /// </summary>
        /// <param name="tabId">The tab we're currently drawing</param>
        /// <returns>The vertical offset to apply</returns>
        private int GetTabVerticalOffset(int tabId)
        {
            Contract.Requires(tabId < Tabs.Count);
            return CurrentTabId == tabId ? SelectedTabVerticalOffset : 0;
        }

        /// <summary>
        /// We need to make our mouse show up in the right place, and if we have one we should draw the hover box
        /// </summary>
        /// <param name="b">Batch to use</param>
        private void DrawMouseAndHoverBox(SpriteBatch b)
        {
            b.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null);

            // Don't forget about inheritance; as good a time as any
            base.draw(b);

            // Now draw the hover text, because that should be in the same place as the mouse
            HoverTextBox.draw(b);

            // If we are using a hardware cursor, then we don't need to redraw it
            if (Graphics.UsingHardwareCursor)
            {
                return;
            }

            // Redraw the mouse, because it disappears
            b.Draw(Graphics.MouseCursor,
                   Graphics.PreviousMousePosition,
                   CursorSource,
                   Color.White,
                   NoRotation,
                   Vector2.Zero,
                   CursorScale,
                   SpriteEffects.None,
                   CursorDepth);
        }

        /// <summary>
        /// Get the clickable button for this tab
        /// </summary>
        /// <param name="tab">The tab to get</param>
        /// <returns>The button</returns>
        private ClickableComponent GetTabComponent(int tabId)
        {
            var rectangle = MakeTabRectangle(tabId + 1);
            var tabName = "test";

            var component = new ClickableComponent(rectangle, tabName)
            {
                myID = BaseComponentId + tabId,
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            };

            // Add the next neighbor if we aren't the last one
            if (tabId < Tabs.Count + 1)
            {
                component.rightNeighborID = BaseComponentId + 1;
                component.downNeighborID = tabId;
            }

            // Add the previous neighbor if we aren't the first one
            if (tabId != 0)
            {
                component.leftNeighborID = tabId - 1;
            }

            return component;
        }

        /// <summary>
        /// Create the bounds of the tab header
        /// </summary>
        /// <param name="tabPosistion">The tab to draw</param>
        /// <returns>Bounding box of the tab</returns>
        private Rectangle MakeTabRectangle(int tabPosistion)
        {
            return new Rectangle(xPositionOnScreen + Graphics.TileSize * tabPosistion,
                                 yPositionOnScreen + tabYPositionRelativeToMenuY + Graphics.TileSize,
                                 Graphics.TileSize,
                                 Graphics.TileSize);
        }

        /// <summary>
        /// Calculate the width of the menu
        /// </summary>
        /// <returns>The width of the menu</returns>
        private static int CalculateMenuWidth()
        {
            return MenuWidth + borderWidth * 2;
        }

        /// <summary>
        /// Calculate the height of the menu
        /// </summary>
        /// <returns>The height of the menu</returns>
        private static int CalculateMenuHeight()
        {
            return MenuHeight + borderWidth * 2;
        }

        /// <summary>
        /// Calculate the upper left hand corner x position of the menu
        /// </summary>
        /// <param name="game">The game whose menu position we're calculating</param>
        /// <returns>The x position</returns>
        private static int CalculateMenuXPosition()
        {
            return Game1.viewport.Width / 2 - CalculateMenuWidth() / 2;
        }

        /// <summary>
        /// Calculate the upper left hand corner y position of the menu
        /// </summary>
        /// <param name="game">The game whose menu position we're calculating</param>
        /// <returns>The y position</returns>
        private static int CalculateMenuYPosition()
        {
            return Game1.viewport.Height / 2 - CalculateMenuHeight() / 2;
        }

    }
}
