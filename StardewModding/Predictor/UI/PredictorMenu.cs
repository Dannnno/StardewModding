using Dannnno.StardewMods.Predictor.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using static System.Linq.Enumerable;

namespace Dannnno.StardewMods.Predictor.UI
{
    #region Internal bits
    /// <summary>
    /// Constituent pieces of a menu item
    /// </summary>
    internal struct MenuTuple
    {
        /// <summary>
        /// Get or set the clickable component itself
        /// </summary>
        internal ClickableComponent Component { get; set; }

        /// <summary>
        /// Get or set the tab data
        /// </summary>
        internal PredictorMenuTab Tab { get; set; }

        /// <summary>
        /// Get or set the icon id
        /// </summary>
        public int IconId { get; set; }
    }

    /// <summary>
    /// The tabs that the predictor UI supports
    /// </summary>
    internal enum PredictorTabNameEnum : int
    {
        Geode = 0
    }

    /// <summary>
    /// Extension methods for convenience with the tab name enum
    /// </summary>
    internal static class PredictorTabNameEnumExtensions
    {
        #region Constants
        // cheating here for a bit now; at some point I'd like to have this pull from the data, but leaving this for prototyping
        public const int GeodeIconId = 535;

        // Guaranteed random by fair dice roll
        private const int StartingTabId = 26438;
        #endregion

        /// <summary>
        /// Get the position of the given tab
        /// </summary>
        /// <param name="tab">The tab in question</param>
        /// <returns>Numeric position of the tab</returns>
        public static int GetTabPosition(this PredictorTabNameEnum tab)
        {
            return (int)tab + 1;
        }

        /// <summary>
        /// Get a unique identifier for a tab
        /// </summary>
        /// <param name="tab">The tab</param>
        /// <returns>The tab ID</returns>
        public static int GetTabId(this PredictorTabNameEnum tab)
        {
            return StartingTabId + GetTabPosition(tab);
        }

        /// <summary>
        /// Get the position of our downstairs neighbor
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>The position of the neighbor below us</returns>
        public static int GetDownNeighbor(this PredictorTabNameEnum tab)
        {
            return tab.GetTabPosition() - 1;
        }

        /// <summary>
        /// Get the id of our right neighbor
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>The id of the tab to our right</returns>
        public static int GetRightNeighbor(this PredictorTabNameEnum tab)
        {
            return tab.GetTabId() + 1;
        }

        /// <summary>
        /// Get the id of our left neighbor
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>The id of the tab to our left</returns>
        public static int GetLeftNeighbor(this PredictorTabNameEnum tab)
        {
            return tab.GetTabId() - 1;
        }

        /// <summary>
        /// Get whether this is the last tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>Whether this is the last tab</returns>
        public static bool IsLastTab(this PredictorTabNameEnum tab)
        {
            return tab.GetTabPosition() == Enum.GetValues(typeof(PredictorTabNameEnum)).Length;
        }

        /// <summary>
        /// Get whether this is the first tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>Whether this is the first tab</returns>
        public static bool IsFirstTab(this PredictorTabNameEnum tab)
        {
            return tab.GetTabPosition() == 0;
        }

        /// <summary>
        /// Get the icon id for this tab
        /// </summary>
        /// <param name="tab">this tab</param>
        /// <returns>The icon ID</returns>
        public static int GetIconId(this PredictorTabNameEnum tab)
        {
            return tab switch
            {
                PredictorTabNameEnum.Geode => GeodeIconId,

                _ => 0,
            };
        }

        /// <summary>
        /// Determine if this is the current tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <param name="currentTab">The current tab</param>
        /// <returns>Whether they're the same tab</returns>
        public static bool IsCurrentTab(this PredictorTabNameEnum tab, int currentTab)
        {
            return (int)tab == currentTab;
        }

        public static Type GetMenuTabType(this PredictorTabNameEnum tab)
        {
            return tab switch
            {
                PredictorTabNameEnum.Geode => typeof(GeodeMenuTab),

                _ => typeof(PredictorMenuTab)
            };
        }
    }
    #endregion

    /// <summary>
    /// Menu class for the predictor app
    /// </summary>
    /// <remarks>
    /// Heavily borrowed from/adapted from https://github.com/LukeSeewald/PublicStardewValleyMods
    /// </remarks>
    public class PredictorMenu : IClickableMenu
    {
        #region fields
        private Lazy<int> LazyTabCount => new Lazy<int>(() => Enum.GetValues(typeof(PredictorTabNameEnum)).Length);
        private Lazy<IList<MenuTuple>> LazyTabs => new Lazy<IList<MenuTuple>>(() => Range(0, TabCount)
                                                                                        .Select(BuildMenuTab)
                                                                                        .ToList());
        #endregion

        #region properties
        /// <summary>
        /// Get the number of tabs that the menu has
        /// </summary>
        public int TabCount { get => LazyTabCount.Value; }

        /// <summary>
        /// Get the tabs associated with the menu
        /// </summary>
        internal IList<MenuTuple> Tabs => LazyTabs.Value;

        /// <summary>
        /// Get the game associated with the menu
        /// </summary>
        public IStardewGame Game { get; private set; }

        /// <summary>
        /// Get the currently selected tab
        /// </summary>
        public int CurrentTab { get; private set; }

        /// <summary>
        /// Get the current hover text
        /// </summary>
        public string HoverText { get; private set; }
        #endregion

        /// <summary>
        /// Create the predictor menu
        /// </summary>
        /// <param name="game">The game we're creating the menu for</param>
        public PredictorMenu(IStardewGame game) : base(CalculateMenuXPosition(game),
                                                       CalculateMenuYPosition(game),
                                                       CalculateMenuWidth(),
                                                       CalculateMenuHeight(),
                                                       ShowCloseButton())
        {
            Game = game;

            //invisible = false;
            //HoverText = "";
            //CurrentTab = (int)TabName.SpecificSeasonTab;
            //ForcePreventClose = false;

        }

        #region Drawing Overrides
        public override void draw(SpriteBatch spriteBatch)
        {
            if (!Game.ShowMenuBackground)
            {
                spriteBatch.Draw(Game.FadeToBlackRectangle,
                                 Game.Bounds,
                                 Color.Black * 0.4f);
            }

            var dialogBoxBounds = new Rectangle(xPositionOnScreen, yPositionOnScreen, Tabs[CurrentTab].Tab.width, Tabs[CurrentTab].Tab.height);
            Game.DrawDialogueBox(dialogBoxBounds);

            Tabs[CurrentTab].Tab.draw(spriteBatch);
            spriteBatch.End();
            foreach (var tab in Tabs)
            {
                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

                // TODO: Make this less magic
                var currentTabVerticalOffset = Tabs[CurrentTab].Component.Equals(tab.Component) ? 8 : 0;
                var spriteX = tab.Component.bounds.X + (int)(Game.TileSize / 3.25) + 3;
                var spriteY = tab.Component.bounds.Y + (int)(Game.TileSize / 2.25) + currentTabVerticalOffset;
                spriteBatch.Draw(Game1.mouseCursors,
                                 new Vector2(tab.Component.bounds.X, tab.Component.bounds.Y + currentTabVerticalOffset),
                                 new Rectangle?(new Rectangle(16, 368, 16, 16)),
                                 Color.White,
                                 0.0f,
                                 Vector2.Zero,
                                 4f,
                                 SpriteEffects.None,
                                 0.0001f);
                spriteBatch.Draw(Game1.objectSpriteSheet,
                                 new Vector2(spriteX, spriteY),
                                 new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tab.IconId, 16, 16)),
                                 Color.White,
                                 0f,
                                 new Vector2(4f, 4f),
                                 2.5f,
                                 SpriteEffects.None,
                                 0.9f);

                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            base.draw(spriteBatch);
            if (!string.IsNullOrWhiteSpace(HoverText))
            {
                DrawHoverTextBox(spriteBatch, HoverText, 4);
            }

            if (Game1.options.hardwareCursor)
            {
                return;
            }
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound)
                {
                    Game1.playSound("bigDeSelect");
                }
                Game1.exitActiveMenu();
                return;
            }

            for (int i = 0; i < TabCount; ++i)
            {
                var tab = Tabs[i].Component;
                if (tab.containsPoint(x, y) && CurrentTab != i)
                {
                    ChangeTab(i);
                    break;
                }
            }

            Tabs[CurrentTab].Tab.receiveLeftClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            Tabs[CurrentTab].Tab.receiveScrollWheelAction(direction);
        }

        public override void leftClickHeld(int x, int y)
        {
            Tabs[CurrentTab].Tab.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            Tabs[CurrentTab].Tab.releaseLeftClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            HoverText = "";
            Tabs[CurrentTab].Tab.performHoverAction(x, y);
            HoverText = Tabs[CurrentTab].Tab.HoverText;
            foreach (var tab in Tabs)
            {
                if (tab.Component.containsPoint(x, y))
                {
                    HoverText = tab.Component.label;
                    return;
                }
            }
            return;
        }
        #endregion

        #region Helper Drawing Methods

        private bool IsGoingOutOfXRightView(int x, int width)
        {
            return x + width > Game1.viewport.Width;
        }

        private bool IsGoingOutOfYDownView(int y, int height)
        {
            return y + height > Game1.viewport.Height;
        }

        private bool IsGoingOutOfYUpView(int y, int height)
        {
            return y - height < 0;
        }

        private int GetBestY(int y, int height)
        {
            int overDown = Math.Abs(y + height - Game1.viewport.Height);
            int overUp = Math.Abs(y - (Game1.viewport.Height - height));

            return overDown > overUp ? Game1.getOldMouseY() - height : y;
        }

        private void DrawHoverTextBox(SpriteBatch b, string text, int spaceBetweenLines)
        {
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
        private void ChangeTab(int tabIndex)
        {
            CurrentTab = tabIndex;
        }


        /// <summary>
        /// Build a menu tab
        /// </summary>
        /// <param name="tab">The tab to build for</param>
        /// <returns>The menu data</returns>
        private MenuTuple BuildMenuTab(int tab)
        {
            return BuildMenuTab((PredictorTabNameEnum)tab);
        }

        /// <summary>
        /// Build a menu tab
        /// </summary>
        /// <param name="tab">The tab to build for</param>
        /// <returns>The menu data</returns>
        private MenuTuple BuildMenuTab(PredictorTabNameEnum tab)
        {
            var tabType = tab.GetMenuTabType();
            var instance = Activator.CreateInstance(tabType, new object[] { xPositionOnScreen, yPositionOnScreen, width, height, tab });

            return new MenuTuple()
            {
                Component = GetTabComponent(tab),
                Tab = (PredictorMenuTab)Convert.ChangeType(instance, tabType),
                IconId = tab.GetIconId()
            };
        }

        /// <summary>
        /// Get the clickable button for this tab
        /// </summary>
        /// <param name="tab">The tab to get</param>
        /// <returns>The button</returns>
        private ClickableComponent GetTabComponent(PredictorTabNameEnum tab)
        {
            var rectangle = MakeTabRectangle(tab.GetTabPosition());
            var tabName = tab.ToString();

            var component = new ClickableComponent(rectangle, tabName)
            {
                myID = tab.GetTabId(),
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            };

            // Add the next neighbor if we aren't the last one
            if (!tab.IsLastTab())
            {
                component.rightNeighborID = tab.GetRightNeighbor();
                component.downNeighborID = tab.GetDownNeighbor();
            }

            // Add the previous neighbor if we aren't the first one
            if (!tab.IsFirstTab())
            {
                component.leftNeighborID = tab.GetLeftNeighbor();
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
            return new Rectangle(xPositionOnScreen + Game.TileSize * tabPosistion,
                                 yPositionOnScreen + tabYPositionRelativeToMenuY + Game.TileSize,
                                 Game.TileSize,
                                 Game.TileSize);
        }

        /// <summary>
        /// Get whether to show the close button
        /// </summary>
        /// <returns>True</returns>
        private static bool ShowCloseButton()
        {
            return true;
        }

        /// <summary>
        /// Calculate the width of the menu
        /// </summary>
        /// <returns>The width of the menu</returns>
        private static int CalculateMenuWidth()
        {
            return 800 + borderWidth * 2;
        }

        /// <summary>
        /// Calculate the height of the menu
        /// </summary>
        /// <returns>The height of the menu</returns>
        private static int CalculateMenuHeight()
        {
            return 600 + borderWidth * 2;
        }

        /// <summary>
        /// Calculate the upper left hand corner x position of the menu
        /// </summary>
        /// <param name="game">The game whose menu position we're calculating</param>
        /// <returns>The x position</returns>
        private static int CalculateMenuXPosition(IStardewGame game)
        {
            return game.WindowWidth / 2 - CalculateMenuWidth() / 2;
        }

        /// <summary>
        /// Calculate the upper left hand corner y position of the menu
        /// </summary>
        /// <param name="game">The game whose menu position we're calculating</param>
        /// <returns>The y position</returns>
        private static int CalculateMenuYPosition(IStardewGame game)
        {
            return game.WindowHeight / 2 - CalculateMenuHeight() / 2;
        }
        #endregion
    }
}
