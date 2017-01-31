using System.Collections.Generic;
using Magpie.Tests.Mocks;
using Magpie.Tests.Models;
using MagpieUpdater.Interfaces;
using MagpieUpdater.Models;
using MagpieUpdater.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Magpie.Tests.Services
{
    [TestClass]
    public class BestChannelFinderTest
    {
        private readonly string _twoChannelJson = @"{'channels': [{'version': '5.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'},
                                                    {'version': '6.9.9', 'release_notes_url': 'release_notes_url_http_3', 'artifact_url': 'artifact_url_http_3', 'build_date': '02/18/2017'}]}" .MakeJson();

        private readonly string _threeChannelJson = @"{'channels': [{'version': '5.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'},
                                                    {'id': 2, 'version': '5.9.9', 'release_notes_url': 'release_notes_url_http_3', 'artifact_url': 'artifact_url_http_3', 'build_date': '02/18/2017'}, {'version': '6.9.9', 'release_notes_url': 'release_notes_url_http_3', 'artifact_url': 'artifact_url_http_3', 'build_date': '02/18/2017'}]}" .MakeJson();
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

        [TestMethod]
        public void WhenChannelsHaveNoIdsReturnsChannelWithHighestVersion()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var appcast = RemoteAppcast.MakeFromJson(_twoChannelJson);
            var selectedChannel = sut.Find(4, appcast.Channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(appcast.Channels[1], selectedChannel);
        }

        [TestMethod]
        public void WhenChannelsHaveMixIdsReturnsChannelWithHighestVersionLessThanSubscribedId()
        {
            var sut = new BestChannelFinder(Substitute.For<IDebuggingInfoLogger>());
            var appcast = RemoteAppcast.MakeFromJson(_threeChannelJson);
            // subscribed to 3, and there is an id of 2 in the json
            var selectedChannel = sut.Find(3, appcast.Channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(appcast.Channels[1], selectedChannel);

            // subscribed to 1, but because there is id that matches it, it returns the highest version of all
            selectedChannel = sut.Find(1, appcast.Channels);
            Assert.IsNotNull(selectedChannel);
            Assert.AreEqual(appcast.Channels[2], selectedChannel);
        }
    }
}