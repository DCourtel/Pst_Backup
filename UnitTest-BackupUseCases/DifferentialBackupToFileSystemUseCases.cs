using System;
using System.IO;
using System.Security.Cryptography;
using SmartSingularity.PstBackupClientDb;
using SUT = SmartSingularity.PstBackupEngine.DifferentialBackupEngine;
using SmartSingularity.PstBackupSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest_BackupUseCases
{
    public class DifferentialBackupToFileSystemUseCases
    {
        [TestClass]
        public class Backup_Should
        {
            private static ApplicationSettings _appSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
            private const string _destinationPath = @"\\192.168.0.250\share\Transit\PstFiles\courtel1";
            private const string _sourcePath = @"E:\Pst Backup\Use Cases";
            private const string _pstFilename = "archive2007.pst";
            private const string _partialFilename = _pstFilename + ".partial";
            private const string _dbFilename = _sourcePath + @"\Pst Backup.sqlite3";
            private static PSTRegistryEntry _pstFileToSave;
            private MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

            [ClassInitialize]
            public static void ClassInitialize(TestContext context)
            {
                _appSettings.FilesAndFoldersDestinationPath = _destinationPath;
                _appSettings.FilesAndFoldersDestinationType = ApplicationSettings.BackupDestinationType.FileSystem;
                _appSettings.BackupAgentBackupMethod = ApplicationSettings.BackupMethod.Differential;
                _pstFileToSave = new PSTRegistryEntry(Path.Combine(_sourcePath, _pstFilename));
            }

            private void DeleteDb()
            {
                if (File.Exists(_dbFilename))
                {
                    File.Delete(_dbFilename);
                    if (File.Exists(_dbFilename))
                        throw new Exception("Can not delete database file");
                }
            }

            private void CopyPartialFile()
            {
                File.Copy(Path.Combine(_sourcePath, _partialFilename), Path.Combine(_destinationPath, _partialFilename), true);
                if (!File.Exists(Path.Combine(_destinationPath, _partialFilename)))
                    throw new Exception("Can not copy Partial file");
            }

            private void CopyInterruptedFile()
            {
                File.Copy(Path.Combine(_sourcePath, "archive2007-Interrupted.pst.partial"), Path.Combine(_destinationPath, "archive2007.pst.partial"), true);
                if (!File.Exists(Path.Combine(_destinationPath, "archive2007.pst.partial")))
                    throw new Exception("Can not copy Partial file");

            }

            private void CopyPstFile()
            {
                File.Copy(Path.Combine(_sourcePath, _pstFilename), Path.Combine(_destinationPath, _pstFilename), true);
                if (!File.Exists(Path.Combine(_destinationPath, _pstFilename)))
                    throw new Exception("Can not copy PST file");
            }

            private void DeleteFile(string filename)
            {
                if(File.Exists(filename))
                {
                    File.Delete(filename);
                    if (File.Exists(filename))
                        throw new Exception("Can not delete the file");
                }
            }

            private bool AreFilesEquals(string file1, string file2)
            {
                using (FileStream stream1 = new FileStream(file1, FileMode.Open, FileAccess.Read))
                using (FileStream stream2 = new FileStream(file2, FileMode.Open, FileAccess.Read))
                {
                    return GetFileHash(stream1) == GetFileHash(stream2);
                }
            }

            private string GetFileHash(FileStream stream)
            {
                return BitConverter.ToString(_md5.ComputeHash(stream)).Replace("-", String.Empty);
            }
#if (DEBUG)
            [TestMethod]
            public void RegisterNewPstFileThenDeletePartialFileAndCreateNewOne_WhenPstFileIsNotRegisteredInDbButPartialFileExists()
            {
                // PST File is not registred in Db but partial file exists.
                // Arrange
                DeleteDb();
                CopyPartialFile();
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                Assert.IsFalse(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(Path.Combine(_sourcePath, _pstFilename), Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void RegisterNewPstFileThenBackupFile_WhenPstFileIsNotRegisteredInDbAndPartialFileDoNotExists()
            {
                // PST File is not registred in Db and partial file exists.
                // Arrange
                DeleteDb();
                DeleteFile(Path.Combine(_destinationPath, _partialFilename));
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                Assert.IsFalse(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsFalse(File.Exists(Path.Combine(_destinationPath, _partialFilename)));

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(Path.Combine(_sourcePath, _pstFilename), Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void UseInformationsFromDbThenBackupFile_WhenPstFileIsRegisteredInDbAndPstFileDoesNotExists()
            {
                // PST File is registred in Db but PST file does not exists.
                // Arrange
                DeleteDb();
                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1,_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _pstFilename), true);
                Assert.IsFalse(File.Exists(Path.Combine(_destinationPath, _pstFilename)));

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void UseInformationsFromDbThenBackupFile_WhenPstFileIsRegisteredInDbAndPstFileExists()
            {
                // PST File is registred in Db and  PST file exists.
                // Arrange
                DeleteDb();
                CopyPstFile();
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1, _pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _pstFilename), true);
                Assert.IsTrue(File.Exists(Path.Combine(_destinationPath, _pstFilename)));

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void UseInformationsFromDbThenBackupFile_WhenPstFileIsRegisteredInDbAndPartialFileExists()
            {
                // PST File is registred in Db and partial file exists.
                // Arrange
                DeleteDb();
                CopyPartialFile();
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1, _pstFileToSave.SourcePath, Path.Combine(_destinationPath, _partialFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _partialFilename), true);                
                Assert.IsTrue(File.Exists(Path.Combine(_destinationPath, _partialFilename)));

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void UseInformationsFromDbThenBackupFile_WhenPstFileIsRegisteredInDbAndPartialFileDoesNotExists()
            {
                // PST File is registred in Db but partial file does not exists.
                // Arrange
                DeleteDb();
                DeleteFile(Path.Combine(_destinationPath, _partialFilename));
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1, _pstFileToSave.SourcePath, Path.Combine(_destinationPath, _partialFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _partialFilename), true);
                Assert.IsFalse(File.Exists(Path.Combine(_destinationPath, _partialFilename)));

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void BackupToNewDestination_WhenDestinationChangeAfterPartialBakupOccurs()
            {
                // PST File is registred in Db and partial file exists but the destination folder have changed
                // Arrange
                DeleteDb();
                CopyPartialFile();
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1, _pstFileToSave.SourcePath, Path.Combine(_destinationPath, _partialFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _partialFilename), true);
                Assert.IsTrue(File.Exists(Path.Combine(_destinationPath, _partialFilename)));

                // Act
                _appSettings.FilesAndFoldersDestinationPath = @"\\192.168.0.250\share\Transit\PstFiles\courtel2";
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(@"\\192.168.0.250\share\Transit\PstFiles\courtel2", _pstFilename)));
                Assert.AreEqual(Path.Combine(@"\\192.168.0.250\share\Transit\PstFiles\courtel2", _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(@"\\192.168.0.250\share\Transit\PstFiles\courtel2", _pstFilename));
                DeleteFile(Path.Combine(_destinationPath, _partialFilename));
            }

            [TestMethod]
            public void BackupToNewDestination_WhenDestinationChangeAfterCompletBakupOccurs()
            {
                // PST File is registred in Db and PST file exists but the destination folder have changed
                // Arrange
                DeleteDb();
                CopyPstFile();
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1, _pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _pstFilename), true);
                Assert.IsTrue(File.Exists(Path.Combine(_destinationPath, _pstFilename)));

                // Act
                _appSettings.FilesAndFoldersDestinationPath = @"\\192.168.0.250\share\Transit\PstFiles\courtel2";
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(@"\\192.168.0.250\share\Transit\PstFiles\courtel2", _pstFilename)));
                Assert.AreEqual(Path.Combine(@"\\192.168.0.250\share\Transit\PstFiles\courtel2", _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(@"\\192.168.0.250\share\Transit\PstFiles\courtel2", _pstFilename));
                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }

            [TestMethod]
            public void FinishBackup_WhenDbContainsPartialHashes()
            {
                // Pst File is registered in Db but the hash table does not contains all hashes for the backup file
                // Arrange
                DeleteDb();
                CopyPartialFile();
                SUT _backupEngine = new SUT(_appSettings, _dbFilename);
                ClientDb _clientDb = new ClientDb(_dbFilename);
                _clientDb.RegisterNewPstFile(1, _pstFileToSave.SourcePath, Path.Combine(_destinationPath, _partialFilename));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.AreEqual(_clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), Path.Combine(_destinationPath, _partialFilename), true);

                // Act
                _backupEngine.Backup((object)_pstFileToSave);
                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
                CopyInterruptedFile();
                _clientDb.RenameBackupFile(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _partialFilename));
                int remainsHashes = (int)(_clientDb.GetHashes(1).Count * 75 / 100);
                _clientDb.DeleteHashes(1, remainsHashes);
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(_pstFileToSave.SourcePath));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename)));
                Assert.AreEqual(Path.Combine(_destinationPath, _pstFilename), _clientDb.GetBackupFilePath(_pstFileToSave.SourcePath), true);

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }
#endif
        }
    }
}
