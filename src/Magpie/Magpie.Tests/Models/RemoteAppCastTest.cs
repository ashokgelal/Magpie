using System.Linq;
using MagpieUpdater.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests.Models
{
    [TestClass]
    public class RemoteAppCastTest
    {
        private readonly string _oneChannelJson =
            @"{'foo': 'bar', 'channels': [{ 'id': 2, 'version': '5.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'}]}"
                .MakeJson();

        private readonly string _twoChannelJson =
            @"{'channels': [{ 'id': 2, 'version': '5.8.8', 'release_notes_url': 'release_notes_url_http', 'artifact_url': 'artifact_url_http', 'build_date': '01/28/2017'},
                                                    { 'id': 3, 'version': '5.9.9', 'release_notes_url': 'release_notes_url_http_3', 'artifact_url': 'artifact_url_http_3', 'build_date': '02/18/2017'}]}"
                .MakeJson();

        private readonly string _invalidJson = @"{'channels': [{ 'id': 2".MakeJson();

        [TestMethod]
        public void CanBeSerializedFromJsonWithOnlyOneChannelInfo()
        {
            var appcast = RemoteAppcast.MakeFromJson(_oneChannelJson);
            Assert.AreEqual(1, appcast.Channels.Count);
            var channel = appcast.Channels.First();
            Assert.AreEqual(2, channel.Id);
            Assert.AreEqual("5.8.8", channel.Version.ToString());
            Assert.AreEqual("release_notes_url_http", channel.ReleaseNotesUrl);
            Assert.AreEqual("artifact_url_http", channel.ArtifactUrl);
            Assert.AreEqual(01, channel.BuildDate.Month);
            Assert.AreEqual(28, channel.BuildDate.Day);
            Assert.AreEqual(2017, channel.BuildDate.Year);
        }

        [TestMethod]
        public void CanBeSerializedFromJsonWithMultipleChannelInfo()
        {
            var appcast = RemoteAppcast.MakeFromJson(_twoChannelJson);
            Assert.AreEqual(2, appcast.Channels.Count);
            var channel = appcast.Channels[1];
            Assert.AreEqual(3, channel.Id);
            Assert.AreEqual("5.9.9", channel.Version.ToString());
            Assert.AreEqual("release_notes_url_http_3", channel.ReleaseNotesUrl);
            Assert.AreEqual("artifact_url_http_3", channel.ArtifactUrl);
            Assert.AreEqual(02, channel.BuildDate.Month);
            Assert.AreEqual(18, channel.BuildDate.Day);
            Assert.AreEqual(2017, channel.BuildDate.Year);
        }

        [TestMethod]
        public void AllValuesAreAddedToRawDictionary()
        {
            var appcast = RemoteAppcast.MakeFromJson(_oneChannelJson);
            Assert.AreEqual(2, appcast.RawDictionary.Count);
            Assert.IsTrue(appcast.RawDictionary.ContainsKey("foo"));
            Assert.AreEqual("bar", appcast.RawDictionary["foo"]);
        }

        [TestMethod]
        public void MakeFromJsonReturnsEmptyAppcastForInvalidJson()
        {
            var appcast = RemoteAppcast.MakeFromJson(_invalidJson);
            Assert.IsNotNull(appcast);
            Assert.AreEqual(0, appcast.Channels.Count);
            Assert.AreEqual(0, appcast.RawDictionary.Count);
        }

        [TestMethod]
        public void MakeFromJsonReturnsEmptyAppcastForEmptyJson()
        {
            var appcast = RemoteAppcast.MakeFromJson(string.Empty);
            Assert.IsNotNull(appcast);
            Assert.AreEqual(0, appcast.Channels.Count);
            Assert.AreEqual(0, appcast.RawDictionary.Count);
        }
    }

    internal static class StringExtension
    {
        internal static string MakeJson(this string input)
        {
            return input.Replace("'", "\"");
        }
    }
}