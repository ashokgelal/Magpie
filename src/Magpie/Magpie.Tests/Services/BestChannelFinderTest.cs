using System.Collections.Generic;
using Magpie.Interfaces;
using Magpie.Models;
using Magpie.Services;
using Magpie.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests.Services
{
    [TestClass]
    public class BestChannelFinderTest
    {
        [TestMethod]
        public void ReturnsNullIfChannelListIsEmpty()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            Assert.IsNull(sut.Find(1, new List<Channel>()));
        }

        [TestMethod]
        public void ReturnsNullIfChannelListIsNull()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            Assert.IsNull(sut.Find(1, null));
        }

        [TestMethod]
        public void ReturnsHighestVersionChannelIfNoChannelMatches()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var channels = new List<Channel> {new MockChannel(2, "2.1.1"), new MockChannel(3, "1.1.1")};
            var selectedChannel = sut.Find(1, channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(selectedChannel, channels[0]);
        }

        [TestMethod]
        public void ReturnsHighestVersionChannelIfMultipleChannelsMatchId()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var channels = new List<Channel>
            {
                new MockChannel(2, "2.1.1"),
                new MockChannel(2, "2.2.1"),
                new MockChannel(2, "1.1.1")
            };
            var selectedChannel = sut.Find(2, channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(selectedChannel, channels[1]);
        }

        [TestMethod]
        public void ReturnsTheBestChannelBasedOnVersion()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var channels = new List<Channel> {new MockChannel(2, "1.1.1"), new MockChannel(3, "1.0.1")};
            var selectedChannel = sut.Find(3, channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(channels[0], selectedChannel);
        }

        [TestMethod]
        public void ReturnsTheBestChannelBasedOnVersionAndId()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var channels = new List<Channel>
            {
                new MockChannel(2, "1.1.1"),
                new MockChannel(3, "1.0.1"),
                new MockChannel(4, "1.2.1")
            };
            var selectedChannel = sut.Find(4, channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(channels[2], selectedChannel);
        }

        [TestMethod]
        public void ReturnsOnlyChannelLessThanEqualId()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var channels = new List<Channel>
            {
                new MockChannel(2, "1.1.1"),
                new MockChannel(3, "1.2.1"),
                new MockChannel(4, "2.2.2")
            };
            var selectedChannel = sut.Find(3, channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(channels[1], selectedChannel);
        }

        [TestMethod]
        public void ReturnsMoreStableVersionInCaseOfVersionMatch()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var channels = new List<Channel>
            {
                new MockChannel(2, "1.1.1"),
                new MockChannel(3, "2.2.2"),
                new MockChannel(4, "2.2.2")
            };
            var selectedChannel = sut.Find(4, channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(channels[1], selectedChannel);
        }
    }
}