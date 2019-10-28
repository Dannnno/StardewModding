using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dannnno.StardewMods.Predictor.Shared
{

    /// <summary>
    /// Class to manage state in Game1 that I might modify
    /// </summary>
    public class GameStateContextManager : IDisposable
    {
        /// <summary>
        /// Get the original values in all managed properties
        /// </summary>
        private IDictionary<string, object> ManagedPropertyOriginalValues { get; set; }

        /// <summary>
        /// Get the monitor to use for logging
        /// </summary>
        private IMonitor Monitor { get; set; }

        /// <summary>
        /// Get the game whose properties we want to rollback
        /// </summary>
        private IStardewGame Wrapper { get; set; }

        /// <summary>
        /// Get the list of properties we need to manage
        /// </summary>
        private IList<PropertyInfo> ManagedProperties { get; set; }

        /// <summary>
        /// Create a new instance of the context manager, with initial Game1 values
        /// </summary>
        /// <param name="wrapper">The game object we are going to be rolling back</param>
        /// <param name="monitor">The monitor to log debug messages to, if desired</param>
        public GameStateContextManager(IStardewGame wrapper, IMonitor monitor = null)
        {
            Wrapper = wrapper;
            Monitor = monitor;
            ManagedPropertyOriginalValues = new Dictionary<string, object>();

            ManagedProperties = wrapper.GetType()
                                       .GetProperties()
                                       .Where(prop => Attribute.IsDefined(prop, typeof(StardewManagedPropertyAttribute)))
                                       .ToList();

            foreach (var prop in ManagedProperties)
            {
                var value = prop.GetValue(wrapper);
                ManagedPropertyOriginalValues[prop.Name] = value;
                Monitor?.Log($"Current[{prop.Name}]: {value}");
            }
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

                    foreach (var prop in ManagedProperties)
                    {
                        var value = prop.GetValue(Wrapper);
                        var original = ManagedPropertyOriginalValues[prop.Name];
                        if (value != original)
                        {
                            prop.SetValue(Wrapper, original);
                            Monitor?.Log($"Current[{prop.Name}]: {value}, resetting to: {original}");
                        }
                    }
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
}
