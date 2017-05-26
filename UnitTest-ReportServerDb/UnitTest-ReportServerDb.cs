using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSingularity.PstBackupReportServer;
using SUT = SmartSingularity.PstBackupReportServerDb.PstBackupReportServerDb;

namespace UnitTest_ReportServerDb
{
    [TestClass]
    public class ReportServerDb
    {
        public static string _dbPath = @"E:\Pst Backup\Test Files\ReportServerDb\Test-PstBackup.mdf";

        [TestClass]
        public class RegisterClient_Should
        {
            [TestMethod]
            public void RegisterTheClient_WhenClientDoNotExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                Client clientToRegister = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };

                // Act
                Assert.IsFalse(sut.IsClientExists(clientToRegister));
                sut.RegisterClient(clientToRegister);

                // Assert
                Assert.IsTrue(sut.IsClientExists(clientToRegister));
            }

            [TestMethod]
            public void UpdateClientInfo_WhenClientAlreadyExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                Client clientToRegister = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };

                // Act
                Assert.IsFalse(sut.IsClientExists(clientToRegister));
                sut.RegisterClient(clientToRegister);
                clientToRegister.ComputerName = "newName";
                clientToRegister.Username = "newUser";
                clientToRegister.Version = new Version("1.0.1709.12");
                sut.RegisterClient(clientToRegister);
                Client updatedClient = sut.GetClient(clientToRegister.Id);

                // Assert
                Assert.IsTrue(updatedClient.Equals(clientToRegister));
            }
        }

        [TestClass]
        public class DeleteClient_Should
        {
            [TestMethod]
            public void DeleteClient_WhenClientExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                Client clientToDelete = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };

                // Act
                sut.RegisterClient(clientToDelete);
                Assert.IsTrue(sut.IsClientExists(clientToDelete));
                sut.DeleteClient(clientToDelete);

                // Assert
                Assert.IsFalse(sut.IsClientExists(clientToDelete));
            }

            [TestMethod]
            public void DeleteOnlyOneClient_WhenClientExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                Client clientToDelete = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };
                Client clientToKeep1 = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };
                Client clientToKeep2 = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };

                // Act
                sut.RegisterClient(clientToDelete);
                sut.RegisterClient(clientToKeep1);
                sut.RegisterClient(clientToKeep2);
                Assert.IsTrue(sut.IsClientExists(clientToDelete));
                Assert.IsTrue(sut.IsClientExists(clientToKeep1));
                Assert.IsTrue(sut.IsClientExists(clientToKeep2));
                sut.DeleteClient(clientToDelete);

                // Assert
                Assert.IsFalse(sut.IsClientExists(clientToDelete));
                Assert.IsTrue(sut.IsClientExists(clientToKeep1));
                Assert.IsTrue(sut.IsClientExists(clientToKeep2));
            }
        }
    }
}
