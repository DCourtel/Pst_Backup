using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SUT = SmartSingularity.PstBackupClientDb.ClientDb;

namespace UnitTest_ClientDb
{
    [TestClass]
    public class ClientDb
    {
        [TestClass]
        public class IsDbExists_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\dbTest.sqlite3");

            [TestMethod]
            public void ReturnTrue_WhenTheDbFileExists()
            {
                // Arrange
                System.IO.FileInfo dbFile = new System.IO.FileInfo(@"E:\Pst Backup\Test Files\dbTest.sqlite3");

                // Act
                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }

                _clientDb.CreateDb();

                // Assert
                Assert.IsTrue(SUT.IsDbExists(dbFile.FullName));
            }

            [TestMethod]
            public void ReturnFalse_WhenTheDbFileDoesNotExists()
            {
                Assert.IsFalse(SUT.IsDbExists(String.Empty));
            }
        }

        [TestClass]
        public class IsDbWellFormated_Should
        {
            [TestMethod]
            public void ReturnTrue_WhenTheDbFileIsWellFormated()
            {
                SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\WellFormated DbFile.sqlite3");
                Assert.IsTrue(_clientDb.IsDbWellFormated());
            }

            [TestMethod]
            public void ReturnFalse_WhenTheDbFileIsNotWellFormated()
            {
                SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\Malformed DbFile.sqlite3");
                Assert.IsFalse(_clientDb.IsDbWellFormated());
            }

            [TestMethod]
            public void ReturnFalse_WhenTheDbFileDoesNotExists()
            {
                SUT _clientDb = new SUT(String.Empty);
                Assert.IsFalse(_clientDb.IsDbWellFormated());
            }
        }

        [TestClass]
        public class IsPstFileRegister_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\db With Pst Files.sqlite3");

            [TestMethod]
            public void ReturnTrue_WhenTheDatabaseHaveRowForThePstFile()
            {
                // Arrange 
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";

                // Act


                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile));
            }

            [TestMethod]
            public void ReturnFalse_WhenTheDatabaseHaveNotRowForThePstSourceFile()
            {
                // Arrange 
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 3008.pst";

                // Act


                // Assert
                Assert.IsFalse(_clientDb.IsPstFileRegistered(sourceFile));
            }
        }

        [TestClass]
        public class RegisterNewPstFile_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\dbForRegisteringFile.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                System.IO.FileInfo dbFile = new System.IO.FileInfo(_clientDb.GetDbPath);

                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }
                _clientDb.CreateDb();
            }

            [TestMethod]
            public void InsertANewRowInTheDatabase_WhenCalled()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";

                // Act
                _clientDb.RegisterNewPstFile(1, sourceFile, destinationFile);

                // Assert
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile));
            }

            [TestMethod]
            public void DoNotCreateHashesInTheDatabase_WhenCalled()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";
                List<string> hashes = new List<string>();

                // Act
                _clientDb.RegisterNewPstFile(1, sourceFile, destinationFile);
                hashes = _clientDb.GetHashes(1);

                // Assert
                Assert.AreEqual(0, hashes.Count);
            }

            [TestMethod]
            [ExpectedException(typeof(System.Data.SQLite.SQLiteException))]
            public void ThrowException_WhenTryingToRegisterAPstFileThatIsAlreadyRegistered()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";

                // Act
                _clientDb.RegisterNewPstFile(1, sourceFile, destinationFile);
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile));
                _clientDb.RegisterNewPstFile(2, sourceFile, destinationFile);
            }

            [TestMethod]
            [ExpectedException(typeof(System.Data.SQLite.SQLiteException))]
            public void ThrowException_WhenTryingToRegisterTwoPstFileWithTheSameFileId()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";

                // Act
                _clientDb.RegisterNewPstFile(1, sourceFile, destinationFile);
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile));
                _clientDb.RegisterNewPstFile(1, @"E:\Pst Backup\Pst Files\Année 2009.pst", destinationFile);
            }

        }

        [TestClass]
        public class DeletePstFile_Should
        {
            [TestMethod]
            public void DeleteRows_WhenSourceFileAndDestinationFileMatch()
            {
                // Arrange
                SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\dbForDeletingFile.sqlite3");
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";
                System.IO.FileInfo dbFile = new System.IO.FileInfo(_clientDb.GetDbPath);

                // Act
                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }
                _clientDb.CreateDb();
                _clientDb.RegisterNewPstFile(1, sourceFile, destinationFile);
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile));
                Assert.IsTrue(_clientDb.GetHashes(1).Count == 0);
                _clientDb.DeletePstFile(sourceFile);

                // Assert
                Assert.IsFalse(_clientDb.IsPstFileRegistered(sourceFile));
                Assert.IsTrue(_clientDb.GetHashes(1).Count == 0);
            }
        }

        [TestClass]
        public class GetFileID_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\dbForGetFileID.sqlite3");

            [TestMethod]
            public void ReturnTheFileIDOfTheRecord_WhenTheRecordExists()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";
                System.IO.FileInfo dbFile = new System.IO.FileInfo(_clientDb.GetDbPath);

                // Act
                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }
                _clientDb.CreateDb();
                _clientDb.RegisterNewPstFile(5, sourceFile, destinationFile);

                // Assert
                Assert.AreEqual(5, _clientDb.GetFileID(sourceFile));
            }

            [TestMethod]
            public void ReturnZero_WhenTheSourceFileDidNotMatch()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";
                System.IO.FileInfo dbFile = new System.IO.FileInfo(_clientDb.GetDbPath);

                // Act
                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }
                _clientDb.CreateDb();
                _clientDb.RegisterNewPstFile(5, sourceFile, destinationFile);

                // Assert
                Assert.AreEqual(0, _clientDb.GetFileID("E:\no pst file.pst"));
            }

            [TestMethod]
            public void ReturnZero_WhenTheDestinationFileDidNotMatch()
            {
                // Arrange
                string sourceFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";
                System.IO.FileInfo dbFile = new System.IO.FileInfo(_clientDb.GetDbPath);

                // Act
                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }
                _clientDb.CreateDb();
                _clientDb.RegisterNewPstFile(5, sourceFile, destinationFile);

                // Assert
                Assert.AreEqual(0, _clientDb.GetFileID("E:\no pst file.pst"));
            }
        }

        [TestClass]
        public class GetAvailableFileId_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\dbForDeletingFile.sqlite3");

            [TestMethod]
            public void ReturnAFileIdDifferentFromZeroAndNotAlreadyUsed_WhenCalled()
            {
                // Arrange
                string sourceFile1 = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string destinationFile1 = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2008.pst.partial";

                string sourceFile2 = @"E:\Pst Backup\Pst Files\Année 2009.pst";
                string destinationFile2 = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2009.pst.partial";

                string sourceFile3 = @"E:\Pst Backup\Pst Files\Année 2010.pst";
                string destinationFile3 = @"\\akio9901lms.ad.fr\Pst Backup\Pst Files\Année 2010.pst.partial";
                System.IO.FileInfo dbFile = new System.IO.FileInfo(_clientDb.GetDbPath);

                // Act
                if (dbFile.Exists)
                {
                    dbFile.Delete();
                    dbFile.Refresh();
                    Assert.IsFalse(dbFile.Exists);
                }
                _clientDb.CreateDb();
                _clientDb.RegisterNewPstFile(1, sourceFile1, destinationFile1);
                _clientDb.RegisterNewPstFile(2, sourceFile2, destinationFile2);
                _clientDb.RegisterNewPstFile(3, sourceFile3, destinationFile3);
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile1));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile2));
                Assert.IsTrue(_clientDb.IsPstFileRegistered(sourceFile3));
                Assert.AreEqual(1, _clientDb.GetFileID(sourceFile1));
                Assert.AreEqual(2, _clientDb.GetFileID(sourceFile2));
                Assert.AreEqual(3, _clientDb.GetFileID(sourceFile3));
                int availableFileID = _clientDb.GetAvailableFileId();

                // Assert
                Assert.AreEqual(4, availableFileID);
            }
        }

        [TestClass]
        public class GetHashes_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForGetHashes.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if(SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                }
                _clientDb.CreateDb();
                _clientDb.InsertHash(1, 0, "00000000000000000000000000000000");
                _clientDb.InsertHash(1, 1, "11111111111111111111111111111111");
                _clientDb.InsertHash(1, 2, "22222222222222222222222222222222");
                _clientDb.InsertHash(1, 3, "33333333333333333333333333333333");
                _clientDb.InsertHash(1, 4, "44444444444444444444444444444444");
                _clientDb.InsertHash(1, 5, "55555555555555555555555555555555");
            }

            [TestMethod]
            public void ReturnTheRightAmountOfHashes_WhenCalled()
            {
                // Arrange
                List<string> _hashes;

                // Act
                _hashes = _clientDb.GetHashes(1);

                // Assert
                Assert.AreEqual(6, _hashes.Count);
            }

            [TestMethod]
            public void ReturnAnEmptyList_WhenTheFileIsNotRegistered()
            {
                // Arrange
                List<string> _hashes;

                // Act
                _hashes = _clientDb.GetHashes(12);

                // Assert
                Assert.AreEqual(0, _hashes.Count);
            }

            [TestMethod]
            public void ReturnRightHashes_WhenTheFileIsRegistered()
            {
                // Arrange
                List<string> _hashes;

                // Act
                _hashes = _clientDb.GetHashes(1);

                // Assert
                Assert.AreEqual("00000000000000000000000000000000", _hashes[0]);
                Assert.AreEqual("11111111111111111111111111111111", _hashes[1]);
                Assert.AreEqual("22222222222222222222222222222222", _hashes[2]);
                Assert.AreEqual("33333333333333333333333333333333", _hashes[3]);
                Assert.AreEqual("44444444444444444444444444444444", _hashes[4]);
            }
        }

        [TestClass]
        public class GetHash_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForGetHash.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if (SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                }
                _clientDb.CreateDb();
                _clientDb.InsertHash(1, 0, "00000000000000000000000000000000");
                _clientDb.InsertHash(1, 1, "11111111111111111111111111111111");
                _clientDb.InsertHash(1, 2, "22222222222222222222222222222222");
                _clientDb.InsertHash(1, 3, "33333333333333333333333333333333");
                _clientDb.InsertHash(1, 4, "44444444444444444444444444444444");
                _clientDb.InsertHash(1, 5, "55555555555555555555555555555555");
            }

            [TestMethod]
            public void ReturnTheRightHash_WhenTheFileIsRegistered()
            {
                // Arrange
                string _hash;

                // Act
                _hash = _clientDb.GetHash(1, 5);

                // Assert
                Assert.AreEqual("55555555555555555555555555555555", _hash);
            }

            [TestMethod]
            public void ReturnStringEmpty_WhenTheFileIsNotRegistered()
            {
                // Arrange
                string _hash;

                // Act
                _hash = _clientDb.GetHash(14, 0);

                // Assert
                Assert.AreEqual(String.Empty, _hash);
            }

            [TestMethod]
            public void ReturnStringEmpty_WhenTheFileIsRegisteredButTheChunkDoesNotExists()
            {
                // Arrange
                string _hash;

                // Act
                _hash = _clientDb.GetHash(1, 99999990);

                // Assert
                Assert.AreEqual(String.Empty, _hash);
            }
        }

        [TestClass]
        public class InsertHash_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForInsertHash.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if (SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                }
                _clientDb.CreateDb();
                _clientDb.InsertHash(1, 0, "00000000000000000000000000000000");
                _clientDb.InsertHash(1, 1, "11111111111111111111111111111111");
                _clientDb.InsertHash(1, 2, "22222222222222222222222222222222");
                _clientDb.InsertHash(1, 3, "33333333333333333333333333333333");
                _clientDb.InsertHash(1, 4, "44444444444444444444444444444444");
                _clientDb.InsertHash(1, 5, "55555555555555555555555555555555");
            }

            [TestMethod]
            public void InsertTheHash_WhenCalled()
            {
                // Arrange
                string expectedHash = "109487142ACBEDFABDEC123412348907";
                string actualHash = String.Empty;

                // Act
                _clientDb.InsertHash(1, 6, expectedHash);
                actualHash = _clientDb.GetHash(1, 6);

                // Assert
                Assert.AreEqual(expectedHash, actualHash, true);
            }
        }

        [TestClass]
        public class DeleteHash_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForDeleteHash.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if (SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                }
                _clientDb.CreateDb();
            }

            [TestMethod]
            public void DeleteTheHash_WhenCalled()
            {
                // Arrange
                List<string> hashes;
                string hash = String.Empty;
                string expectedHash = "109487142ACBEDFABDEC123412348907";

                // Act
                for (int i = 0; i < 10; i++)
                {
                    _clientDb.InsertHash(1, i, expectedHash);
                }
                hashes = _clientDb.GetHashes(1);
                Assert.AreEqual(10, hashes.Count);
                hash = hashes[0];
                Assert.AreEqual(expectedHash, hash);
                _clientDb.DeleteHash(1, 0);
                hashes = _clientDb.GetHashes(1);
                hash = _clientDb.GetHash(1, 0);

                // Assert
                Assert.AreEqual(9, hashes.Count);
                Assert.AreEqual(string.Empty, hash);
            }
        }

        [TestClass]
        public class ShrinkChunkList_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\ShrinkChunkList.sqlite3");
            private int _chunkSize = SmartSingularity.PstBackupEngine.DifferentialBackupEngine._chunkSize;

            [TestInitialize]
            public void TestInitialize()
            {
                if(SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                    Assert.IsFalse(SUT.IsDbExists(_clientDb.GetDbPath));
                }
                _clientDb.CreateDb();
                _clientDb.Connect();
                string hash = String.Empty;
                for (int i = 0; i < 120; i++)
                {
                    char c = (char)(48 + (i % 10));
                    hash = new string(c, 32);
                    _clientDb.InsertHashToConnectedDb(1, i, hash);
                }
                _clientDb.Disconnect();
            }
            
            [TestMethod]
            public void ShrinkTheListOfChunk_WhenTheFileHaveBeenShrink()
            {
                // Arrange    
                Assert.AreEqual(120, _clientDb.GetHashes(1).Count);

                // Act
                _clientDb.ShrinkChunkList(1, _chunkSize * 116, _chunkSize);

                // Assert
                Assert.AreEqual(116, _clientDb.GetHashes(1).Count);
            }

            [TestMethod]
            public void DoNotChangeTheChunkList_WhenTheFileHaveNotBeenChange()
            {
                // Arrange
                Assert.AreEqual(120, _clientDb.GetHashes(1).Count);

                // Act
                _clientDb.ShrinkChunkList(1, _chunkSize * 120, _chunkSize);

                // Assert
                Assert.AreEqual(120, _clientDb.GetHashes(1).Count);

            }
        }

        [TestClass]
        public class UpdateHash_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForUpdate.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if (SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                    Assert.IsFalse(SUT.IsDbExists(_clientDb.GetDbPath));
                }
                _clientDb.CreateDb();
                _clientDb.Connect();

                string hash = String.Empty;
                for (int i = 0; i < 120; i++)
                {
                    char c = (char)(48 + (i % 10));
                    hash = new string(c, 32);
                    _clientDb.InsertHashToConnectedDb(1, i, hash);
                }
                _clientDb.Disconnect();
            }

            [TestMethod]
            public void WriteTheNewHash_WhenFileIdAndChunkIdMatchARecord()
            {
                // Arrange
                string expectedHash = "01234567890123456789012345678901";
                string actualHash = String.Empty;

                // Act
                actualHash = _clientDb.GetHash(1, 0);
                Assert.AreEqual("00000000000000000000000000000000", actualHash);
                _clientDb.UpdateHash(1, 0, expectedHash);
                actualHash = _clientDb.GetHash(1, 0);

                // Assert
                Assert.AreEqual(expectedHash, actualHash);
            }

            [TestMethod]
            public void DoesNothing_WhenFileIdAndChunkIdDoNotMatchARecord()
            {
                // Arrange
                string expectedHash = "01234567890123456789012345678901";
                string actualHash = String.Empty;

                // Act
                _clientDb.UpdateHash(1, 999999, expectedHash);
            }
        }

        [TestClass]
        public class GetBackupFilePath_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForGetBackupFilePath.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if (SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                    Assert.IsFalse(SUT.IsDbExists(_clientDb.GetDbPath));
                }
                _clientDb.CreateDb();
            }

            [TestMethod]
            public void ReturnTheFullPathOfTheBackupFile_WhenSourceFileExists()
            {
                // Arrange
                string expected = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string actual = String.Empty;

                // Act
                _clientDb.RegisterNewPstFile(1, expected, expected + ".partial");
                Assert.IsTrue(_clientDb.IsPstFileRegistered(expected));
                actual = _clientDb.GetBackupFilePath(expected);

                // Assert
                Assert.AreEqual(expected + ".partial", actual, true);
            }

            [TestMethod]
            public void ReturnStringEmpty_WhenSourceFileDoesNotExists()
            {
                // Arrange
                string expected = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string actual = "wrong";

                // Act
                _clientDb.RegisterNewPstFile(1, expected, expected + ".partial");
                Assert.IsTrue(_clientDb.IsPstFileRegistered(expected));
                actual = _clientDb.GetBackupFilePath(@"E:\Pst Backup\Pst Files\Année 2009.pst");

                // Assert
                Assert.AreEqual(String.Empty, actual, true);
            }
        }

        [TestClass]
        public class RenameBackupFile_Should
        {
            private SUT _clientDb = new SUT(@"E:\Pst Backup\Test Files\DbForRenameBackupFilePath.sqlite3");

            [TestInitialize]
            public void TestInitialize()
            {
                if (SUT.IsDbExists(_clientDb.GetDbPath))
                {
                    System.IO.File.Delete(_clientDb.GetDbPath);
                    Assert.IsFalse(SUT.IsDbExists(_clientDb.GetDbPath));
                }
                _clientDb.CreateDb();
            }

            [TestMethod]
            public void RenameTheBackupFile_WhenSourceFileExists()
            {
                // Arrange
                string pstFile = @"E:\Pst Backup\Pst Files\Année 2008.pst";
                string expectedBackupFile = @"\\akio9901lns\Pst Backup\Pst Files\Année 2008.pst";
                string actual = @"F:\Pst Backup\Pst Files\Année 2008.pst";

                // Act
                _clientDb.RegisterNewPstFile(1,pstFile, actual);
                Assert.AreEqual(actual, _clientDb.GetBackupFilePath(pstFile), true);
                _clientDb.RenameBackupFile(pstFile, expectedBackupFile);

                // Assert
                Assert.AreEqual(expectedBackupFile, _clientDb.GetBackupFilePath(pstFile), true);
            }
        }
    }

}
