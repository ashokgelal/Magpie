using System;
using Magpie.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests.Models
{
    [TestClass]
    public class VersionExtensionsTest
    {
        [TestMethod]
        public void TestRevision()
        {
            var version = new Version(1, 2, 3, 4);
            var otherVersion = new Version(1, 2, 3, 0);
            Assert.IsTrue(version.IsHigherThan(otherVersion));
            Assert.IsFalse(otherVersion.IsHigherThan(version));
        }

        [TestMethod]
        public void TestBuild()
        {
            var version = new Version(1, 2, 4, 4);
            var otherVersion = new Version(1, 2, 3, 4);
            Assert.IsTrue(version.IsHigherThan(otherVersion));
            Assert.IsFalse(otherVersion.IsHigherThan(version));
        }

        [TestMethod]
        public void TestMinor()
        {
            var version = new Version(1, 2, 3, 4);
            var otherVersion = new Version(1, 1, 3, 4);
            Assert.IsTrue(version.IsHigherThan(otherVersion));
            Assert.IsFalse(otherVersion.IsHigherThan(version));
        }
        [TestMethod]
        public void TestMajor()
        {
            var version = new Version(1, 2, 3, 4);
            var otherVersion = new Version(0, 2, 3, 4);
            Assert.IsTrue(version.IsHigherThan(otherVersion));
            Assert.IsFalse(otherVersion.IsHigherThan(version));
        }
    }
}