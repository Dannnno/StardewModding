using Dannnno.StardewMods.Predictor.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dannnno.StardewMods.Predictor.UI
{
    /// <summary>
    /// Constituent pieces of a menu item
    /// </summary>
    public struct MenuTuple
    {
        /// <summary>
        /// Get or set the clickable component itself
        /// </summary>
        internal ClickableComponent Component { get; set; }

        /// <summary>
        /// Get or set the menu data
        /// </summary>
        internal ClickableMenuMixin Menu { get; set; }

        /// <summary>
        /// Get or set the icon id
        /// </summary>
        public int IconId { get; set; }
    }

    public class TabbedMenuMixin<TEnum> : ClickableMenuMixin where TEnum : Enum
    {
        #region Fields
        private Lazy<int> LazyEnumMemberCount => new Lazy<int>(() => Enum.GetValues(typeof(TEnum)).Length);
        #endregion

        #region Properties
        /// <summary>
        /// Get the number of tabs that the menu has
        /// </summary>
        public int TabCount => Tabs.Count;

        /// <summary>
        /// Get the default background color
        /// </summary>
        public Color DefaultBackgroundColor => Color.Black * 0.4f;

        /// <summary>
        /// Get the number of members in the enum tab basis
        /// </summary>
        public int EnumMemberCount => LazyEnumMemberCount.Value;

        /// <summary>
        /// Get the list of tabs in the menu
        /// </summary>
        public IList<MenuTuple> Tabs { get; protected set; }

        /// <summary>
        /// Get the currently selected tab index
        /// </summary>
        public int CurrentTab { get; protected set; }

        /// <summary>
        /// Get the currently selected tab's menu
        /// </summary>
        public ClickableMenuMixin CurrentMenu => Tabs[CurrentTab].Menu;

        /// <summary>
        /// Get the currently selected tab's button
        /// </summary>
        public ClickableComponent CurrentComponent => Tabs[CurrentTab].Component;

        /// <summary>
        /// Get the currently selected tab's icon id
        /// </summary>
        public int CurrentIconId => Tabs[CurrentTab].IconId;

        /// <summary>
        /// Get the base tab ID used for this menu
        /// </summary>
        public int StartingTabId { get; private set; }
        #endregion

        /// <summary>
        /// Create a new tabbed menu
        /// </summary>
        /// <param name="game">The game this is for</param>
        /// <param name="startingTabId">The first tab ID</param>
        public TabbedMenuMixin(IStardewGame game, int startingTabId) : base(game)
        {
            StartingTabId = startingTabId;
            Tabs = new List<MenuTuple>();
        }

        #region TEnum helper methods
        /// <summary>
        /// Get the TEnum as an integer
        /// </summary>
        /// <param name="tab">The tab part of the enum</param>
        /// <returns>Integer representation of the enum</returns>
        public int GetTEnumAsInt(TEnum tab)
        {
            Contract.Requires(Enum.GetUnderlyingType(typeof(TEnum)).Equals(typeof(int)), $"The underlying enum type of enum {typeof(TEnum)} must be an int");
            return (int)Convert.ChangeType(tab, typeof(int));
        }

        /// <summary>
        /// Get the position of the given tab
        /// </summary>
        /// <param name="tab">The tab in question</param>
        /// <returns>Numeric position of the tab</returns>
        public int GetTabPosition(TEnum tab)
        {
            return GetTEnumAsInt(tab) + 1;
        }

        /// <summary>
        /// Get a unique identifier for a tab
        /// </summary>
        /// <param name="tab">The tab</param>
        /// <returns>The tab ID</returns>
        public int GetTabId(TEnum tab)
        {
            return StartingTabId + GetTabPosition(tab);
        }

        /// <summary>
        /// Get the position of our downstairs neighbor
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>The position of the neighbor below us</returns>
        public int GetDownNeighbor(TEnum tab)
        {
            return GetTabPosition(tab) - 1;
        }

        /// <summary>
        /// Get the id of our right neighbor
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>The id of the tab to our right</returns>
        public int GetRightNeighbor(TEnum tab)
        {
            return GetTabId(tab) + 1;
        }

        /// <summary>
        /// Get the id of our left neighbor
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>The id of the tab to our left</returns>
        public int GetLeftNeighbor(TEnum tab)
        {
            return GetTabId(tab) - 1;
        }

        /// <summary>
        /// Get whether this is the last tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>Whether this is the last tab</returns>
        public bool IsLastTab(TEnum tab)
        {
            return GetTabPosition(tab) == EnumMemberCount;
        }

        /// <summary>
        /// Get whether this is the first tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <returns>Whether this is the first tab</returns>
        public bool IsFirstTab(TEnum tab)
        {
            return GetTabPosition(tab) == 0;
        }

        /// <summary>
        /// Determine if this is the current tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <param name="currentTab">The current tab</param>
        /// <returns>Whether they're the same tab</returns>
        public bool IsCurrentTab(TEnum tab, int currentTab)
        {
            return GetTEnumAsInt(tab) == currentTab;
        }

        /// <summary>
        /// Determine if this is the current tab
        /// </summary>
        /// <param name="tab">This tab</param>
        /// <param name="currentTab">The current tab</param>
        /// <returns>Whether they're the same tab</returns>
        public bool IsCurrentTab(TEnum tab, TEnum currentTab)
        {
            return tab.Equals(currentTab);
        }

        /// <summary>
        /// Get the Icon ID associated with the tab
        /// </summary>
        /// <param name="tab">The tab to get the value for</param>
        /// <returns>Icon ID</returns>
        protected virtual int GetIconId(TEnum tab)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Drawing Helper Methods
        // Most code in this section very heavily borrows from https://github.com/LukeSeewald/PublicStardewValleyMods
        public override void draw(SpriteBatch spriteBatch)
        {
            if (!Invisible)
            {
                if (!Game.ShowMenuBackground)
                {
                    spriteBatch.Draw(
                        Game.FadeToBlackRectangle,
                        Game.Bounds,
                        DefaultBackgroundColor
                    );
                }

                var currentTabBoundingBox = new Rectangle(CurrentMenu.xPositionOnScreen, CurrentMenu.yPositionOnScreen, CurrentMenu.width, CurrentMenu.height);
                Game.DrawDialogueBox(currentTabBoundingBox);
                CurrentMenu.draw(spriteBatch);
                spriteBatch.End();

                if (!ForcePreventClose)
                {
                    for (int i = 0; i < TabCount; ++i)
                    {
                        spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

                        var component = Tabs[i].Component;
                        var currentTabVerticalOffset = (CurrentTab == i ? 8 : 0);
                        var spriteX = component.bounds.X + (int)(Game.TileSize / 3.25) + 3;
                        var spriteY = component.bounds.Y + (int)(Game.TileSize / 2.25) + currentTabVerticalOffset;
                        spriteBatch.Draw(Game1.mouseCursors, 
                                         new Vector2(component.bounds.X, component.bounds.Y + currentTabVerticalOffset), 
                                         new Rectangle?(new Rectangle(16, 368, 16, 16)), 
                                         Color.White, 
                                         0.0f, 
                                         Vector2.Zero, 
                                         4f, 
                                         SpriteEffects.None, 
                                         0.0001f);
                        spriteBatch.Draw(Game1.objectSpriteSheet, 
                                         new Vector2(spriteX, spriteY), 
                                         new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Tabs[i].IconId, 16, 16)), 
                                         Color.White, 
                                         0f, 
                                         new Vector2(4f, 4f), 
                                         2.5f, 
                                         SpriteEffects.None, 
                                         0.9f);
                        spriteBatch.End();
                    }
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                }
            }
            else
            {
                CurrentMenu.draw(spriteBatch);
            }

            if (!ForcePreventClose)
            {
                base.draw(spriteBatch);
                DrawHoverTextBox(spriteBatch);
            }

            if (Game1.options.hardwareCursor)
            {
                return;
            }

            spriteBatch.Draw(Game1.mouseCursors, 
                             new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), 
                             new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16)), 
                             Color.White, 
                             0.0f, 
                             Vector2.Zero, 
                             Game1.pixelZoom + Game1.dialogueButtonScale / 150f, 
                             SpriteEffects.None, 
                             1f);
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

            CurrentMenu.receiveLeftClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            CurrentMenu.receiveScrollWheelAction(direction);
        }

        public override void leftClickHeld(int x, int y)
        {
            CurrentMenu.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            CurrentMenu.releaseLeftClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            HoverText = "";
            CurrentMenu.performHoverAction(x, y);
            HoverText = CurrentMenu.HoverText;
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

        /// <summary>
        /// Swap tabs
        /// </summary>
        /// <param name="tabIndex">The tab to swap to</param>
        protected void ChangeTab(int tabIndex)
        {
            CurrentTab = tabIndex;
        }

        /// <summary>
        /// Get the clickable button for this tab
        /// </summary>
        /// <param name="tab">The tab to get</param>
        /// <returns>The button</returns>
        protected ClickableComponent GetTabComponent(TEnum tab)
        {
            var rectangle = MakeTabRectangle(GetTabPosition(tab));
            var tabName = tab.ToString();

            var component = new ClickableComponent(rectangle, tabName)
            {
                myID = GetTabId(tab),
                tryDefaultIfNoDownNeighborExists = true,
                fullyImmutable = true
            };

            // Add the next neighbor if we aren't the last one
            if (!IsLastTab(tab))
            {
                component.rightNeighborID = GetRightNeighbor(tab);
                component.downNeighborID = GetDownNeighbor(tab);
            }

            // Add the previous neighbor if we aren't the first one
            if (!IsFirstTab(tab))
            {
                component.leftNeighborID = GetLeftNeighbor(tab);
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
        #endregion
    }
}
