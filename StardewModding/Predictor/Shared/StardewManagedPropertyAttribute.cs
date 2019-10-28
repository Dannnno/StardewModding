using System;

namespace Dannnno.StardewMods.Predictor.Shared
{
    /// <summary>
    /// Attribute that indicates a property is managed in the StardewValley game
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StardewManagedPropertyAttribute : Attribute { }
}
