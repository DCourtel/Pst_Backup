using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSingularity.PstBackupReportServer;
using SUT = SmartSingularity.PstBackupReportServer.ReportServerDb;

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
                    ComputerName = "Céos"
                };

                // Act
                Assert.IsFalse(sut.IsClientExists(clientToRegister));
                sut.RegisterClient(clientToRegister);
                clientToRegister.ComputerName = "newName";
                clientToRegister.Username = "Domain\\newUser";
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

        [TestClass]
        public class RegisterPstFile_Should
        {
            [TestMethod]
            public void RegisterThePstFile_WhenPstFileDoNotExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                string clientId = Guid.NewGuid().ToString();
                PstFile pstFileToRegister = new PstFile()
                {
                    LocalPath = @"C:\Pst Files\Courtel\Archive1.pst",
                    IsSetToBackup = true,
                    Size = 57_345_786
                };

                // Act
                Assert.IsFalse(sut.IsPstFileExists(clientId, pstFileToRegister.LocalPath));
                sut.RegisterPstFile(clientId, pstFileToRegister);

                // Assert
                Assert.IsTrue(sut.IsPstFileExists(clientId, pstFileToRegister.LocalPath));
            }

            [TestMethod]
            public void RegisterRightInformationsFromPstFile_WhenPstFileDoNotExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                string clientId = Guid.NewGuid().ToString();
                PstFile pstFileToRegister = new PstFile()
                {
                    LocalPath = @"C:\Pst Files\Courtel\Archive1.pst",
                    IsSetToBackup = true,
                    Size = 57_345_786,
                    LastSuccessfulBackup = null
                };

                // Act
                Assert.IsFalse(sut.IsPstFileExists(clientId, pstFileToRegister.LocalPath));
                sut.RegisterPstFile(clientId, pstFileToRegister);
                PstFile insertedPstFile = sut.GetPstFile(clientId, pstFileToRegister.LocalPath);

                // Assert
                Assert.AreEqual(pstFileToRegister.LocalPath, insertedPstFile.LocalPath, true);
                Assert.AreEqual(pstFileToRegister.IsSetToBackup, insertedPstFile.IsSetToBackup);
                Assert.AreEqual(pstFileToRegister.Size, insertedPstFile.Size);
                Assert.AreEqual(pstFileToRegister.LastSuccessfulBackup, insertedPstFile.LastSuccessfulBackup);
            }

            [TestMethod]
            public void UpdatePstFileInfo_WhenLastSuccessfulBackupChange()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                string clientId = Guid.NewGuid().ToString();
                PstFile pstFileToRegister = new PstFile()
                {
                    LocalPath = @"C:\Pst Files\Courtel\Archive1.pst",
                    IsSetToBackup = true,
                    Size = 57_345_786,
                    LastSuccessfulBackup = null
                };

                // Act
                Assert.IsFalse(sut.IsPstFileExists(clientId, pstFileToRegister.LocalPath));
                sut.RegisterPstFile(clientId, pstFileToRegister);
                pstFileToRegister.LastSuccessfulBackup = DateTime.UtcNow;
                sut.RegisterPstFile(clientId, pstFileToRegister);
                PstFile updatedPstFile = sut.GetPstFile(clientId, pstFileToRegister.LocalPath);

                // Assert
                Assert.IsNotNull(updatedPstFile.LastSuccessfulBackup);
                Assert.AreEqual(updatedPstFile.LastSuccessfulBackup.Value.Date, DateTime.UtcNow.Date);
            }

            [TestMethod]
            public void UpdatePstFileInfo_WhenLastSuccessfulBackupChangeToNull()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                string clientId = Guid.NewGuid().ToString();
                PstFile pstFileToRegister = new PstFile()
                {
                    LocalPath = @"C:\Pst Files\Courtel\Archive1.pst",
                    IsSetToBackup = true,
                    Size = 57_345_786
                };

                // Act
                Assert.IsFalse(sut.IsPstFileExists(clientId, pstFileToRegister.LocalPath));
                sut.RegisterPstFile(clientId, pstFileToRegister);

                pstFileToRegister.LastSuccessfulBackup = DateTime.UtcNow;
                sut.RegisterPstFile(clientId, pstFileToRegister);
                Assert.IsNotNull(sut.GetPstFile(clientId, pstFileToRegister.LocalPath).LastSuccessfulBackup);
                pstFileToRegister.LastSuccessfulBackup = null;
                sut.RegisterPstFile(clientId, pstFileToRegister);
                PstFile updatedPstFile = sut.GetPstFile(clientId, pstFileToRegister.LocalPath);

                // Assert
                Assert.IsNull(updatedPstFile.LastSuccessfulBackup);
            }
        }
    }
}
