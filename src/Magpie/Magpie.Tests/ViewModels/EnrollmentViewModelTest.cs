using Magpie.Tests.Mocks;
using MagpieUpdater.Models;
using MagpieUpdater.Services;
using MagpieUpdater.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests.ViewModels
{
    [TestClass]
    public class EnrollmentViewModelTest
    {
        [TestMethod]
        public void PropertiesAreInitializedCorrectly()
        {
            var channel = new MockChannel(1, build: "Test Build", enrollmentEula: "www.example.com");
            var enrollment = new Enrollment(channel);
            var appInfo = new AppInfo("testString") {AppIconPath = "iconPath"};
            var sut = new EnrollmentViewModel(enrollment, appInfo);
            Assert.AreEqual("Test Build", sut.ChannelName);
            Assert.AreEqual("www.example.com", sut.EnrollmentEulaUrl);
            Assert.AreEqual("iconPath", sut.AppIconPath);
        }

        [TestMethod]
        public void CanNotEnrollWithInvalidEmail()
        {
            var sut = new EnrollmentViewModel(new Enrollment(new Channel()), new AppInfo("testString"))
            {
                EmailAddress = "test"
            };
            Assert.IsFalse(sut.EnrollCommand.CanExecute(null));
            sut.EmailAddress = null;
            Assert.IsFalse(sut.EnrollCommand.CanExecute(null));
            sut.EmailAddress = string.Empty;
            Assert.IsFalse(sut.EnrollCommand.CanExecute(null));
            sut.EmailAddress = "test@";
            Assert.IsFalse(sut.EnrollCommand.CanExecute(null));
        }

        [TestMethod]
        public void CanEnrollWithValidEmail()
        {
            var sut = new EnrollmentViewModel(new Enrollment(new Channel()), new AppInfo("testString"))
            {
                EmailAddress = "test@example.com"
            };
            Assert.IsTrue(sut.EnrollCommand.CanExecute(null));
        }

        [TestMethod]
        public void ExecuteEnrollCommand_FillsInEnrollment()
        {
            var enrollment = new Enrollment(new Channel());
            var sut = new EnrollmentViewModel(enrollment, new AppInfo("testString"))
            {
                EmailAddress = "test@example.com"
            };
            sut.EnrollCommand.Execute(null);
            Assert.IsTrue(enrollment.IsEnrolled);
            Assert.AreEqual("test@example.com", enrollment.Email);
        }
    }
}
