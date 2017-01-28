using System;
using Magpie.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests.Models
{
    [TestClass]
    public class AppInfoTest
    {
        [TestMethod]
        public void SubscribedChannelIsOneByDefault()
        {
            var appInfo = new AppInfo("test_url");
            Assert.AreEqual(1, appInfo.SubscribedChannel);
        }

        [TestMethod]
        public void CanExplicitlySetSubscribedChannel()
        {
            var appInfo = new AppInfo("test_url", 2);
            Assert.AreEqual(2, appInfo.SubscribedChannel);
        }

        [TestMethod]
        public void DefaultSignatureFilenameIsSetFromConstructor()
        {
            var appInfo = new AppInfo("test_url");
            Assert.AreEqual(SignatureVerifier.DefaultDSAPubKeyFileName, appInfo.PublicSignatureFilename);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExceptionIfAppCastUrlIsNull()
        {
            var _ = new AppInfo(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExceptionIfAppCastUrlIsEmpty()
        {
            var _ = new AppInfo("");
            var __ = new AppInfo("   ");
        }

        [TestMethod]
        public void WhenSettingAppImagePathAppIconPathIsSetToFullPath()
        {
            var appInfo = new AppInfo("test_url");
            appInfo.SetAppIcon("test_namespace", "test_path");
            Assert.AreEqual("pack://application:,,,/test_namespace;component/test_path", appInfo.AppIconPath);
        }
    }
}
