using Dannnno.StardewMods.Abstraction;
using Dannnno.StardewMods.Predictor.Geodes;
using Dannnno.StardewMods.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dannnno.StardewMods.Predictor.UI
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// Get the elements of a dictionary as unpackables
        /// </summary>
        /// <typeparam name="KeyType">Type of the key</typeparam>
        /// <typeparam name="ValueType">Type of the value</typeparam>
        /// <param name="me">Dictionary we want</param>
        /// <returns>Pairs as a value tuple</returns>
        public static IEnumerable<ValueTuple<KeyType, ValueType>> AsTuple<KeyType, ValueType>(this IDictionary<KeyType, ValueType> me) => me.Select(pair => (pair.Key, pair.Value));

        /// <summary>
        /// Zip together two sequences
        /// </summary>
        /// <typeparam name="T1">The left sequence's type</typeparam>
        /// <typeparam name="T2">The right sequence's type</typeparam>
        /// <param name="left">The left sequence</param>
        /// <param name="right">The right sequence</param>
        /// <returns>The pair of values</returns>
        public static IEnumerable<ValueTuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) => left.Zip(right, (l, r) => (l, r));

        /// <summary>
        /// Enumerate the items in an enumerable
        /// </summary>
        /// <typeparam name="T1">Type of the sequence's elements</typeparam>
        /// <param name="left">The sequence</param>
        /// <param name="start">The starting value for counting</param>
        /// <returns>The enumeration of them</returns>
        public static IEnumerable<ValueTuple<int, T1>> Enumerate<T1>(this IEnumerable<T1> left, int start = 0) => Enumerable.Range(start, int.MaxValue - start).Zip(left);
    }

    /// <summary>
    /// Menu class for individual tabs
    /// </summary>
    /// <remarks>
    /// Heavily borrowed from/adapted from https://github.com/LukeSeewald/PublicStardewValleyMods
    /// </remarks>
    internal class GeodeMenuTab : MenuTab<GeodeMenuItem>
    {
        #region Constants
        private const int DefaultSearchDistance = 1;
        #endregion

        #region Properties
        /// <summary>
        /// Get the current geode count
        /// </summary>
        private uint GeodeCount => Game.GeodeCount;

        /// <summary>
        /// Get or set the distance to search ahead
        /// </summary>
        public uint Distance { get; set; }

        /// <summary>
        /// The predictor we're going to use
        /// </summary>
        public GeodePredictor Predictor { get; set; }
        #endregion

        /// <summary>
        /// Create the Geode predictor menu
        /// </summary>
        /// <param name="game">Game the menu is for</param>
        /// <param name="geodePredictor">How to predict geodes</param>
        /// <param name="distance">How far we want to search</param>
        public GeodeMenuTab(IStardewGame game, GeodePredictor geodePredictor, uint distance = DefaultSearchDistance) : base("Geodes", game)
        {
            Predictor = geodePredictor;
            Distance = distance;
        }

        /// <summary>
        /// Populate the items in our list
        /// </summary>
        protected override void PopulateItems()
        {
            Items = new List<GeodeMenuItem>();

            foreach ((var index, var geodeMapping) in Predictor.PredictTreasureFromGeodeByRangeDistance(Distance, Distance)
                                                               .Enumerate((int)GeodeCount))
            {
                var data = geodeMapping.Select(pair => new GeodeResultModel()
                {
                    Index = index,
                    DisplayName = "temp",
                    ParentGeodeId = pair.Key.ParentSheetIndex,
                    TreasureId = pair.Value.ParentSheetIndex,
                    AlreadyDonatedToMuseum = false,
                    HoverText = "hover temp",
                    HistoricalResult = index < GeodeCount,
                    Current = index == GeodeCount
                }).ToList();

                Items.Add(new GeodeMenuItem(data));
            }
        }

        protected override void DrawHeader(SpriteBatch spriteBatch)
        {
            base.DrawHeader(spriteBatch);

            int geodeCount = Predictor.GeodeList.Count;

            //spriteBatch, ViewableItems[i].bounds.X, ViewableItems[i].bounds.Y, xPositionOnScreen
            //int spriteX = slotX + bounds.X;
            //int spriteY = slotY + bounds.Y;

            //int NameX = slotX + bounds.X + (int)(Game1.tileSize * 1.5);
            //int TextY = slotY + bounds.Y + Game1.pixelZoom * 3;

            int actualWidth = width - ScrollBarRunner.Width;
            int headerWidth = actualWidth / (geodeCount + 1);
            int spaceBetweenItems = (actualWidth / geodeCount) / (geodeCount + 2);

            for (int i = 0; i < geodeCount; ++i)
            {
                spriteBatch.Draw(Game1.objectSpriteSheet,
                                 new Rectangle((int)(xPositionOnScreen // Starting at the top left
                                                    + (i + 1) * spaceBetweenItems // Add some buffer
                                                    + (i + .5) * headerWidth), // Fill in the space itself, centered
                                               yPositionOnScreen + 200, // Start a bit below the top of the menu
                                               SpriteSize,
                                               SpriteSize),
                                 new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, Predictor.GeodeList[i].ParentSheetIndex, 16, 16)),
                                 Color.White);
            }

            drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarRunner.X, ScrollBarRunner.Y, ScrollBarRunner.Width, ScrollBarRunner.Height, Color.White, Game1.pixelZoom, false);
        }

    }
}