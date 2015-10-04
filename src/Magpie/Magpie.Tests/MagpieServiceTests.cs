using System;
using System.Reflection;
using Magpie.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests
{
    [TestClass]
    public class MagpieServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            AssemblyInjector.Inject();
        }

        [TestMethod]
        public void TestValidJson()
        {
            var mockMagpieService = new MockMagpieService("validContentUrl");
            mockMagpieService.RunInBackground("validContentUrl");
            var appcast = mockMagpieService.RemoteAppcast;
            Assert.IsNotNull(appcast);
            Assert.AreEqual("Magpie", appcast.Title);
            Assert.AreEqual(new Version(0, 0, 1), appcast.Version);
        }

        [TestMethod]
        public void TestAppcastAvailableRaiseEvent()
        {
            var raised = false;
            var mockMagpieService = new MockMagpieService("validContentUrl");
            mockMagpieService.RemoteAppcastAvailableEvent += (s, a) => { raised = true; };
            mockMagpieService.RunInBackground("validContentUrl");
            Assert.IsTrue(raised);
        }
    }

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
            entryAssemblyfield.SetValue(manager, assembly);

            var domain = AppDomain.CurrentDomain;
            var domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }
    }
}
