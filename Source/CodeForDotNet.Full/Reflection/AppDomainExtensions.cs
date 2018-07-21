using System;
using System.IO;
using System.Reflection;

namespace CodeForDotNet.Reflection
{
    /// <summary>
    /// Contains extensions for working with <see cref="AppDomain"/>s.
    /// </summary>
    public static class AppDomainExtensions
    {
        /// <summary>
        /// Creates an instance then executes a method in a separate application domain,
        /// using the configuration and working directory of the caller's application domain.
        /// </summary>
        /// <remarks>
        /// Disposes the instance upon completion if it implements <see cref="IDisposable"/>.
        /// </remarks>
        public static void Run(this AppDomain currentDomain, MethodBase method, params object[] parameters)
        {
            // Validate
            if (currentDomain == null) throw new ArgumentNullException("currentDomain");
            if (method == null) throw new ArgumentNullException("method");

            // Run...
            AppDomain childDomain = null;
            object instance = null;
            try
            {
                // Create child application domain
                var domainSetup = new AppDomainSetup
                                      {
                                          ConfigurationFile = currentDomain.SetupInformation.ConfigurationFile,
                                          ApplicationBase = Directory.GetCurrentDirectory()
                                      };
                childDomain = AppDomain.CreateDomain(currentDomain.FriendlyName, currentDomain.Evidence, domainSetup);

                // Create instance
                instance = childDomain.CreateInstanceAndUnwrap(
                    method.DeclaringType.Assembly.FullName, method.DeclaringType.FullName);

                // Invoke method with any parameters
                method.Invoke(instance, parameters);
            }
            finally
            {
                try
                {
                    // Dispose when implemented (in case unmanaged resources must be freed)
                    if (instance != null && method.DeclaringType.GetInterface("IDisposable") != null)
                        typeof(IDisposable).GetMethod("Dispose").Invoke(instance, null);
                }
                finally
                {
                    // Unload application domain (important to do always, even when Dispose throws exception)
                    if (childDomain != null)
                        AppDomain.Unload(childDomain);
                }
            }
        }
    }
}
