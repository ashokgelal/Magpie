using System;

namespace Magpie.Services
{
    internal static class Helpers
    {
        internal static bool IsHigherThan(this Version thisVersion, Version otherVersion)
        {
             return thisVersion.CompareTo(otherVersion) > 0;
        }
    }
}