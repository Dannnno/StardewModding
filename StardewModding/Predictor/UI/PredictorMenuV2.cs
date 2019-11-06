using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Dannnno.StardewMods.Predictor.UI.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Dannnno.StardewMods.Predictor.UI
{
    public class PredictorMenuV2 : IClickableMenu
    {
        #region Constants
        private static readonly bool ShowCloseButton = true;
        private static readonly int MenuWidth = 800;
        private static readonly int MenuHeight = 600;
        private const int SelectedTabVerticalOffset = 8;
        private const float NoRotation = 0.0f;
        private const float CursorScale = 4f;
        private const float CursorDepth = 0.0001f;
        private const int TileSheetSpriteSize = 16;
        private const float TabSpriteScale = 2.5f;
        private const float TabSpriteDepth = 0.9f;
        private const int BaseComponentId = 26438;
        private const int RottingPlantId = 747;

        // We don't need to recalculate this every time
        private static Lazy<Rectangle> LazyTabCursorRectangle => new Lazy<Rectangle>(() => new Rectangle(16, 368, 16, 16));
        private static Lazy<Vector2> LazyTabSpriteOrigin => new Lazy<Vector2>(() => new Vector2(4f, 4f));

        private static int TabXSpriteOffset => (int)(Game1.tileSize / 3.25) + 3;
        private static int TabYSpriteOffset => (int)(Game1.tileSize / 2.25);

        private static Rectangle TabCursorRectangle => LazyTabCursorRectangle.Value;
        private static Vector2 TabSpriteOrigin => LazyTabSpriteOrigin.Value;
        #endregion

        #region Properties
        private IList<object> Tabs { get; set; }

        private string hoverText;
        public string HoverText
        {
            get => hoverText;
            set
            {
                HoverTextBox.HoverText = value;
                hoverText = value;
            }
        }

        private readonly HoverTextBox HoverTextBox = new HoverTextBox();

        public int CurrentTabId { get; private set; }
        #endregion

        public PredictorMenuV2() : base(CalculateMenuXPosition(),
                                        CalculateMenuYPosition(),
                                        CalculateMenuWidth(),
                                        CalculateMenuHeight(),
                                        ShowCloseButton)
        {
            Tabs = new List<object>
            {
                1,
                2,
                3,
                4
            };
            CurrentTabId = 0;

            HoverText = "test";
        }

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
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(
                    Game1.fadeToBlackRect,
                    Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    Color.Black * 0.4f
                );
            }

            // Now draw out the menu itself 
            Game1.drawDialogueBox(
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                speaker: false,
                drawOnlyBox: true);

            b.End();
        }

        /// <summary>
        /// Draw the individual tabs
        /// </summary>
        /// <param name="b"></param>
        private void DrawClickableTabs(SpriteBatch b)
        {
            for (int i = 0; i < Tabs.Count; ++i)
            {
                b.Begin(SpriteSortMode.FrontToBack, 
                        BlendState.NonPremultiplied, 
                        SamplerState.PointClamp, 
                        null,
                        null);

                ClickableComponent currentComponent = GetTabComponent(i);
                int iconId = GetTabIconId(i);

                // Draw the tab background
                b.Draw(Game1.mouseCursors,
                       GetBackgroundBoundsFromComponent(i, currentComponent),
                       TabCursorRectangle,
                       Color.White,
                       NoRotation,
                       Vector2.Zero,
                       CursorScale,
                       SpriteEffects.None,
                       CursorDepth);

                // Draw the sprite we want on the tab
                b.Draw(Game1.objectSpriteSheet,
                       GetSpriteBoundsFromComponent(i, currentComponent),
                       Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, iconId, TileSheetSpriteSize, TileSheetSpriteSize),
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
        private int GetTabIconId(int i)
        {
            // Dumb thing that doesn't give me intellisense warnings and always returns the same thing
            return i == i + 1 ? RottingPlantId : RottingPlantId;
        }

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
            if (Game1.options.hardwareCursor)
            {
                return;
            }

            // Redraw the mouse, because it disappears
            b.Draw(Game1.mouseCursors,
                   new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                   new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)),
                   Color.White,
                   0.0f,
                   Vector2.Zero,
                   Game1.pixelZoom + Game1.dialogueButtonScale / 150f,
                   SpriteEffects.None,
                   1f);
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
            return new Rectangle(xPositionOnScreen + Game1.tileSize * tabPosistion,
                                 yPositionOnScreen + tabYPositionRelativeToMenuY + Game1.tileSize,
                                 Game1.tileSize,
                                 Game1.tileSize);
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
