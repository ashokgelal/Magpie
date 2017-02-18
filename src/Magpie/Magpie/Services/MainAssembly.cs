using System;
using System.Linq;
using System.Reflection;

namespace MagpieUpdater.Services
{
    internal static class MainAssembly
    {
        internal static string ProductName
        {
            get
            {
                var assemblyProductAttribute
                    = Assembly.GetEntryAssembly()
                        .GetCustomAttributes(typeof(AssemblyProductAttribute))
                        .OfType<AssemblyProductAttribute>()
                        .FirstOrDefault();
                return assemblyProductAttribute != null
                    ? assemblyProductAttribute.Product
                    : Assembly.GetEntryAssembly().GetName().Name;
            }
        }

        internal static Version Version
        {
            get { return Assembly.GetEntryAssembly().GetName().Version; }
        }
    }
}