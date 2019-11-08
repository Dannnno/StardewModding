using System;

namespace Dannnno.StardewMods.Abstraction
{
    /// <summary>
    /// Attribute that indicates a property is managed in the StardewValley game
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class StardewManagedPropertyAttribute : Attribute { }
}
