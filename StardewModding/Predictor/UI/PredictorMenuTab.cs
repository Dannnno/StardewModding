using StardewValley.Menus;

namespace Dannnno.StardewMods.Predictor.UI
{
    /// <summary>
    /// Menu class for individual tabs
    /// </summary>
    /// <remarks>
    /// Heavily borrowed from/adapted from https://github.com/LukeSeewald/PublicStardewValleyMods
    /// </remarks>
    internal abstract class PredictorMenuTab : IClickableMenu
    {
        public string HoverText { get; set; }

        public PredictorMenuTab(int x, int y, int width, int height): base(x, y, width, height, true) { }
    }
}