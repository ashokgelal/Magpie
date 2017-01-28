using System;
using Magpie.Services;
using Magpie.ViewModels;
using Magpie.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests.ViewModels
{
    [TestClass]
    public class SignatureVerificationViewModelTest
    {
        [TestInitialize]
        public void Initialize()
        {
            AssemblyInjector.Inject();
        }

        [TestMethod]
        public void AppIconPathIsSet()
        {
            var appInfo = new AppInfo("app_cast_url")
            {
                AppIconPath = "test_icon_path"
            };
            var sut = new SignatureVerificationWindowViewModel(appInfo);
            Assert.AreEqual("test_icon_path", sut.AppIconPath);
        }

        [TestMethod]
        public void TitleIsProperlySet()
        {
            var appInfo = new AppInfo("app_cast_url");
            var sut = new SignatureVerificationWindowViewModel(appInfo);
            Assert.AreEqual("Magpie.Tests Signature Error", sut.Title);
        }
    }
}
