using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Magpie.Tests
{
    internal static class AssemblyInjector
    {
        /// <summary>
        /// Allows setting the Entry Assembly when needed. 
        /// Use AssemblyUtilities.SetEntryAssembly() as first line in XNA ad hoc tests
        /// </summary>
        /// <param name="assembly">Assembly to set as entry assembly</param>
        public static void Inject(Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();
            var manager = new AppDomainManager();
            var entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            if (entryAssemblyfield != null) entryAssemblyfield.SetValue(manager, assembly);

            var domain = AppDomain.CurrentDomain;
            var domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            if (domainManagerField != null) domainManagerField.SetValue(domain, manager);
        }
    }
}
