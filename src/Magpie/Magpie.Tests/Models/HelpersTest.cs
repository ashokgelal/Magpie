using System;
using MagpieUpdater.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magpie.Tests.Models
{
    [TestClass]
    public class HelpersTest
    {
        [TestMethod]
        public void TestVersionsAreEqual()
        {
            var version1 = new Version(0, 4, 3, 2);
            var version2 = new Version(0, 4, 3, 2);
            Assert.IsFalse(version1.IsHigherThan(version2));
            Assert.IsFalse(version2.IsHigherThan(version1));
        }

        [TestMethod]
        public void TestOnlyMajorNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 0, 2);
            var lowerVersion = new Version(4, 4, 0, 2);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestOnlyMinorNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 3, 2);
            var lowerVersion = new Version(5, 3, 3, 2);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestOnlyBuildNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 3, 2);
            var lowerVersion = new Version(5, 4, 2, 2);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestOnlyRevisionNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 3, 2);
            var lowerVersion = new Version(5, 4, 3, 1);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestMajorAndMinorNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 3, 2);
            var lowerVersion = new Version(4, 6, 3, 2);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestMinorAndBuildNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 3, 2);
            var lowerVersion = new Version(5, 2, 5, 2);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestBuildAndRevisionNumbersDiffer()
        {
            var higherVersion = new Version(5, 4, 3, 2);
            var lowerVersion = new Version(5, 4, 1, 6);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }

        [TestMethod]
        public void TestUnknownRevisionNumber()
        {
            var higherVersion = new Version(3, 4, 0);
            var lowerVersion = new Version(3, 3, 9, 9);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));

            higherVersion = new Version(3, 4, 0, 9);
            lowerVersion = new Version(3, 4, 0);
            Assert.IsTrue(higherVersion.IsHigherThan(lowerVersion));
            Assert.IsFalse(lowerVersion.IsHigherThan(higherVersion));
        }
    }
}