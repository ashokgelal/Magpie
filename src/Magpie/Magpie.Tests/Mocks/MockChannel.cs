using System;
using MagpieUpdater.Models;

namespace Magpie.Tests.Mocks
{
    internal class MockChannel : Channel
    {
        internal MockChannel(int id, string version = null, string build = null, string enrollmentEula = null)
        {
            Id = id;
            if (version != null)
            {
                Version = new Version(version);
            }
            Build = build;
            EnrollmentEulaUrl = enrollmentEula;
        }

        internal void SetArtifactUrl(string url)
        {
            ArtifactUrl = url;
        }
    }
}