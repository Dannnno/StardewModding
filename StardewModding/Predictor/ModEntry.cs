using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Dannnno.StardewMods.ExampleMod
{
    using StardewObject = StardewValley.Object;

    /// <summary>
    /// Mod entry point for predictions
    /// </summary>
    public class ModEntry : Mod
    {
        internal class GameStatContextManager: IDisposable
        {
            private uint originalGeodeCrackCount;

            public GameStatContextManager()
            {
                originalGeodeCrackCount = Game1.stats.GeodesCracked;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        // Reset the modified game stats
                        Game1.stats.GeodesCracked = originalGeodeCrackCount;
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~GameStatContextManager()
            // {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion

        }

        /// <summary>
        /// The number of geodes that have already been cracked by the player
        /// </summary>
        public uint GeodeCount => Game1.stats.GeodesCracked;
        public ulong UniqueGameId => Game1.uniqueIDForThisGame;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            PredictTreasureFromGeode(20);
        }

        private object PredictTreasureFromGeode(uint distance = 1)
        {
            var geodes = GetStardewGeodeObjects().ToList();

            using (var disposer = new GameStatContextManager())
            {
                uint endingValue = GeodeCount + distance;
                for (; GeodeCount < endingValue; ++Game1.stats.geodesCracked)
                {
                    var builder = new StringBuilder();
                    builder.Append($"{GeodeCount}, {Game1.stats.geodesCracked}: ");
                    foreach (var geodeKind in geodes)
                    {
                        var nextTreasure = Utility.getTreasureFromGeode(geodeKind);
                        builder.Append($"{geodeKind.ParentSheetIndex}={nextTreasure.ParentSheetIndex} ");
                    }
                    Monitor.Log(builder.ToString(), LogLevel.Alert);
                }
            }
            Monitor.Log($"{GeodeCount}", LogLevel.Alert);

            return null;
        }

        private IEnumerable<StardewObject> GetStardewGeodeObjects()
        {
            var content = Helper.Content.Load<Dictionary<int, string>>("Data/ObjectInformation", ContentSource.GameContent);

            foreach (var item in content)
            {
                if (item.Value.Contains("Geode"))
                {
                    yield return new StardewObject(item.Key, 1);
                }
            }
        }


        private StardewObject ParseGeodeObjectInformation(int key, string value)
        {
            string name, type, display, description;
            List<int> mineralsToSpawn;
            /* Vanilla geode objects as of SV 1.3.6
             * Structure is Key=name/price/edibility/type/display name/description/minerals to spawn
             *   535=Geode/50/-300/Basic/Geode/A blacksmith can break this open for you./538 542 548 549 552 555 556 557 558 566 568 569 571 574 576 121
             *   536=Frozen Geode/100/-300/Basic/Frozen Geode/A blacksmith can break this open for you./541 544 545 546 550 551 559 560 561 564 567 572 573 577 123
             *   537=Magma Geode/150/-300/Basic/Magma Geode/A blacksmith can break this open for you./539 540 543 547 553 554 562 563 565 570 575 578 122
             *   749=Omni Geode/0/-300/Basic/Omni Geode/A blacksmith can break this open for you. These geodes contain a wide variety of Minerals./538 542 548 549 552 555 556 557 558 566 568 569 571 574 576 541 544 545 546 550 551 559 560 561 564 567 572 573 577 539 540 543 547 553 554 562 563 565 570 575 578 121 122 123
             */
            var values = value.Split('/');

            if (values.Length != 7)
            {
                return null;
            }

            name = values[0];
            int.TryParse(values[1], out int price);
            int.TryParse(values[2], out int edibility);
            type = values[3];
            display = values[4];
            description = values[5];
            mineralsToSpawn = values[6].Split(' ')
                                       .Select(mineral => int.Parse(mineral))
                                       .ToList();

            return new StardewObject(key, 1);
        }


        /// <summary>
        /// The kinds of geodes
        /// </summary>
        internal enum GeodeTypeEnum
        {
            All = 0,
            Regular = 1,
            Frozen = 2,
            Magma = 3,
            Omni = 4
        }
    }
}