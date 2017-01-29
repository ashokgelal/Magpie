using System;
using Magpie.Models;

namespace Magpie.Tests.Mocks
{
    internal class MockChannel : Channel
    {
        public MockChannel(int id, string version = null)
        {
            Id = id;
            if (version != null)
            {
                Version = new Version(version);
            }
        }
    }
}
