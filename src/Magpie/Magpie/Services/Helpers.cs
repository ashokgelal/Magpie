using System;

namespace Magpie.Services
{
    internal static class Helpers
    {
        internal static bool IsHigherThan(this Version version, Version otherVersion)
        {
            return version.Revision > otherVersion.Revision
                               || version.Build > otherVersion.Build
                               || version.Minor > otherVersion.Minor
                               || version.Major > otherVersion.Major;
        }
    }
}
