using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SUT = SmartSingularity.PstBackupNetwork;

namespace UnitTest_PstBackupNetwork
{
    public class Network
    {
        [TestClass]
        public class GetHostName_Should
        {
            [TestMethod]
            public void ReturnsTheNameOfTheHost_WhenCalledWithAnUNCPathStartingWithServerName()
            {
                // Arrange
                string expected = "SrvName";

                // Assert
                Assert.AreEqual(expected, SUT.Network.GetHostName(@"\\" + expected + @"\Share$\Pst Backup\%userlogin%"), true);
            }

            [TestMethod]
            public void ReturnsTheNameOfTheHost_WhenCalledWithAnUNCPathStartingWithFQDNServerName()
            {
                // Arrange
                string expected = "SrvName.domain.local";

                // Assert
                Assert.AreEqual(expected, SUT.Network.GetHostName(@"\\" + expected + @"\Share$\Pst Backup\%userlogin%"), true);
            }

            [TestMethod]
            public void ReturnsTheIpAddress_WhenCalledWithAnUNCPathStartingWithIpAddress()
            {
                // Arrange
                string expected = "192.168.0.10";

                // Assert
                Assert.AreEqual(expected, SUT.Network.GetHostName(@"\\" + expected + @"\Share$\Pst Backup\%userlogin%"), true);
            }

            [TestMethod]
            public void ReturnsLocalHost_WhenParameterIsNotAnUNCPath()
            {
                // Arrange
                string expected = "localhost";

                // Assert
                Assert.AreEqual(expected, SUT.Network.GetHostName(@"E:\Share$\Pst Backup\%userlogin%"), true);

            }

            [TestMethod]
            public void ReturnsLocalHost_WhenUNCPathIsMalFormed()
            {
                // Arrange
                string expected = "localhost";

                // Assert
                Assert.AreEqual(expected, SUT.Network.GetHostName(@"\192.168.0.10\Share$\Pst Backup\%userlogin%"), true);

            }
        }

        [TestClass]
        public class IsInSameSubnet_Should
        {
            [TestMethod]
            public void ReturnsTrue_WhenLocalAddressAndRemoteAddressAreInTheSameSubnet()
            {
                // Arrange
                IPAddress _192_168_65_250 = new IPAddress(new byte[] { 192, 168, 65, 250 });
                IPAddress _192_168_65_254 = new IPAddress(new byte[] { 192, 168, 65, 254 });
                IPAddress _192_168_70_250 = new IPAddress(new byte[] { 192, 168, 70, 250 });
                IPAddress _255_255_255_0 = new IPAddress(new byte[] { 255, 255, 255, 0 });
                IPAddress _255_255_192_0 = new IPAddress(new byte[] { 255, 255, 192, 0 });
                string _192_168_65_0_Subnet_24 = "192.168.65.0/24";
                string _192_168_64_0_Subnet_18 = "192.168.64.0/18";

                // Assert
                Assert.IsTrue(SUT.Network.IsInSameSubnet(_192_168_65_250, _192_168_65_254, _255_255_255_0));
                Assert.IsTrue(SUT.Network.IsInSameSubnet(_192_168_65_250, _192_168_70_250, _255_255_192_0));
                Assert.IsTrue(SUT.Network.IsInSameSubnet(_192_168_65_250, _192_168_65_0_Subnet_24));
                Assert.IsTrue(SUT.Network.IsInSameSubnet(_192_168_70_250, _192_168_64_0_Subnet_18));
            }

            [TestMethod]
            public void ReturnsFalse_WhenLocalAddressAndRemoteAddressAreNotInTheSameSubnet()
            {
                // Arrange
                IPAddress _192_168_65_250 = new IPAddress(new byte[] { 192, 168, 65, 250 });
                IPAddress _192_168_70_250 = new IPAddress(new byte[] { 192, 168, 70, 250 });
                IPAddress _123_78_70_14 = new IPAddress(new byte[] { 123, 78, 70, 14 });
                IPAddress _255_255_255_0 = new IPAddress(new byte[] { 255, 255, 255, 0 });
                string _192_168_65_0_Subnet_24 = "192.168.65.0/24";

                // Assert
                Assert.IsFalse(SUT.Network.IsInSameSubnet(_192_168_65_250, _192_168_70_250, _255_255_255_0));
                Assert.IsFalse(SUT.Network.IsInSameSubnet(_192_168_65_250, _123_78_70_14, _255_255_255_0));
                Assert.IsFalse(SUT.Network.IsInSameSubnet(_192_168_70_250, _192_168_65_0_Subnet_24));
            }
        }

        [TestClass]
        public class IsWanLink_Should
        {
            [TestMethod]
            public void RetunsTrue_WhenDestinationIsOnWan()
            {
                // Arrange
                string _IpAddress = @"\\173.194.65.94\share$\%userlogin%";
                string _serverName = @"\\google.fr\share$\%userlogin";

                // Assert
                Assert.IsTrue(SUT.Network.IsWanLink(_IpAddress, String.Empty));
                Assert.IsTrue(SUT.Network.IsWanLink(_serverName, String.Empty));
            }

            [TestMethod]
            public void ReturnsFalse_WhenDestinationIsNotOnWan()
            {
                // Arrange
                string _IpAddress = @"\\127.0.0.1\share$\%userlogin%";

                // Assert
                Assert.IsFalse(SUT.Network.IsWanLink(_IpAddress, String.Empty));
            }
        }
    }
}
