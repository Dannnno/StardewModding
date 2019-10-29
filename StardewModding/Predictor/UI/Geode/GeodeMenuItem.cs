using System.Collections.Generic;

namespace Dannnno.StardewMods.Predictor.UI
{
    internal class GeodeMenuItem: MenuItemMixin
    {
        public GeodeMenuItem(string from, string to): base()
        {
            Mappings[from] = to;
        }
    }
}