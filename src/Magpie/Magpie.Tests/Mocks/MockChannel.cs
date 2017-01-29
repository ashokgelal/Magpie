using System;
using MagpieUpdater.Models;

namespace Magpie.Tests.Mocks
{
    internal class MockChannel : Channel
    {
        internal MockChannel(int id, string version = null)
        {
            Id = id;
            if (version != null)
            {
                Version = new Version(version);
            }
        }

        internal void SetArtifactUrl(string url)
        {
            ArtifactUrl = url;
        }
    }
}