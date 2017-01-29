using System;
using Magpie.Interfaces;
using Magpie.Services;
using Magpie.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests.Services
{
    [TestClass]
    public class UpdateDeciderTest
    {
        private UpdateDecider _sut;
        private RegistryIO _registryIO;

        [TestInitialize]
        public void TestInitialize()
        {
            AssemblyInjector.Inject();
            _registryIO = Substitute.For<RegistryIO>();
            _sut = new UpdateDecider(Substitute.For<IDebuggingInfoLogger>(), _registryIO);
        }

        [TestMethod]
        public void ShouldDecideFalseIfItIsFirstRun()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("1");
            Assert.IsFalse(_sut.ShouldUpdate(new MockChannel(1, "2.2")));
        }

        [TestMethod]
        public void ShouldDecideTrueIfItIsNotFirstRun()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns("01/28/2016");
            Assert.IsTrue(_sut.ShouldUpdate(new MockChannel(1, "2.2")));
        }


        [TestMethod]
        public void ShouldDecideFalseForSkipVersion()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns("01/28/2016");
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("2.2");
            Assert.IsFalse(_sut.ShouldUpdate(new MockChannel(1, "2.2")));
        }

        [TestMethod]
        public void ShouldDecideFalseIfLastCheckWasLessThanOneDay()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromMinutes(10)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsFalse(_sut.ShouldUpdate(new MockChannel(1, "2.2")));
        }

        [TestMethod]
        public void ShouldDecideTrueIfLastCheckWasMoreThanOneDay()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsTrue(_sut.ShouldUpdate(new MockChannel(1, "2.2")));
        }

        [TestMethod]
        public void ShouldDecideTrueIfAvailableVersionIsHigherThanInstalledVersion()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsTrue(_sut.ShouldUpdate(new MockChannel(1, "2.2")));
        }

        [TestMethod]
        public void ShouldDecideFalseIfAvailableVersionIsLowerThanInstalledVersion()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsFalse(_sut.ShouldUpdate(new MockChannel(1, "0.2")));
        }

        [TestMethod]
        public void ShouldDecideFalseIfAvailableVersionIsEqualToInstalledVersion()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsFalse(_sut.ShouldUpdate(new MockChannel(1, "1.0.0")));
        }

        [TestMethod]
        public void LastCheckDateIsNotUpdatedInRegistryIfItIsLessThanOneDay()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromMinutes(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            _sut.ShouldUpdate(new MockChannel(1, "2.2"));
            _registryIO.DidNotReceive().WriteToRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>());
        }

        [TestMethod]
        public void LastCheckDateIsUpdatedInRegistryIfMoreThanOneDay()
        {
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            _sut.ShouldUpdate(new MockChannel(1, "2.2"));
            _registryIO.Received(1).WriteToRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>());
        }

        [TestMethod]
        public void IfForcedToDecideItIgnoresLastCheckDateSetting()
        {
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromMinutes(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsTrue(_sut.ShouldUpdate(new MockChannel(1, "2.2"), true));
            _registryIO.DidNotReceive().ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>());
        }

        [TestMethod]
        public void IfForcedToDecideItIgnoresSkipVersionSetting()
        {
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(2)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("");
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("2.2");
            Assert.IsTrue(_sut.ShouldUpdate(new MockChannel(1, "2.2"), true));
            _registryIO.DidNotReceive().ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>());
        }

        [TestMethod]
        public void IfForcedToDecideItIgnoresSkipFirstRunSetting()
        {
            _registryIO.ReadFromRegistry(MagicStrings.LAST_CHECK_DATE, Arg.Any<string>()).Returns(DateTime.Now.Subtract(TimeSpan.FromMinutes(1)).ToShortDateString());
            _registryIO.ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>()).Returns("1");
            _registryIO.ReadFromRegistry(MagicStrings.SKIP_VERSION_KEY, Arg.Any<string>()).Returns("");
            Assert.IsTrue(_sut.ShouldUpdate(new MockChannel(1, "2.2"), true));
            _registryIO.DidNotReceive().ReadFromRegistry(MagicStrings.IS_FIRST_RUN, Arg.Any<string>());
        }
    }
}
