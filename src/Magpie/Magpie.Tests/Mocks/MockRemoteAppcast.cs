using System;
using Magpie.Models;

namespace Magpie.Tests.Mocks
{
    public class MockRemoteAppcast : RemoteAppcast
    {
        public MockRemoteAppcast(Version version)
        {
            Version = version;
        }
    }
}
