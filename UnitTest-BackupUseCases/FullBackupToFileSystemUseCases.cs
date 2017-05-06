using System;
using System.IO;
using System.Security.Cryptography;
using SUT = SmartSingularity.PstBackupEngine.FullBackupEngine;
using SmartSingularity.PstBackupSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest_BackupUseCases
{
    public class FullBackupToFileSystemUseCases
    {
        [TestClass]
        public class Backup_Should
        {
            private static ApplicationSettings _appSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
            private const string _destinationPath = @"\\192.168.0.250\share\Transit\PstFiles\courtel2";
            private const string _sourcePath = @"E:\Pst Backup\Use Cases";
            private const string _decompressionFolder = @"E:\Pst Backup\Use Cases\DecompressionFolder";
            private const string _pstFilename = "archive2007.pst";
            private static PSTRegistryEntry _pstFileToSave;
            private MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

            [ClassInitialize]
            public static void ClassInitialize(TestContext context)
            {
                _appSettings.FilesAndFoldersDestinationPath = _destinationPath;
                _appSettings.FilesAndFoldersDestinationType = ApplicationSettings.BackupDestinationType.FileSystem;
                _appSettings.BackupAgentBackupMethod = ApplicationSettings.BackupMethod.Full;
                _pstFileToSave = new PSTRegistryEntry(Path.Combine(_sourcePath, _pstFilename));
            }

            private void DeleteFile(string filename)
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    if (File.Exists(filename))
                        throw new Exception("Can not delete the file");
                }
            }

            private void UncompressedFile(string compressedFile, string uncompressedFile)
            {
                using (System.IO.Compression.GZipStream compressed = new System.IO.Compression.GZipStream(new FileStream(compressedFile, FileMode.Open, FileAccess.Read), System.IO.Compression.CompressionMode.Decompress))
                using (FileStream uncompressed = new FileStream(uncompressedFile, FileMode.Create, FileAccess.Write))
                {
                    compressed.CopyTo(uncompressed);
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

            [TestMethod]
            public void CompressAndSaveTheFile_WhenCompressionIsActive()
            {
                // Arrange
                _appSettings.FilesAndFoldersCompressFiles = true;
                SUT _backupEngine = new SUT(_appSettings);

                // Act
                _backupEngine.Backup((object)_pstFileToSave);
                UncompressedFile(Path.Combine(_destinationPath, _pstFilename + ".gz"), Path.Combine(_decompressionFolder, _pstFilename));

                // Assert
                Assert.IsTrue(File.Exists(Path.Combine(_destinationPath, _pstFilename + ".gz")));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_decompressionFolder, _pstFilename)));

                DeleteFile(Path.Combine(_destinationPath, _pstFilename + ".gz"));
                DeleteFile(Path.Combine(_decompressionFolder, _pstFilename));
            }

            [TestMethod]
            public void SaveTheFile_WhenCompressionIsNotActive()
            {
                // Arrange
                _appSettings.FilesAndFoldersCompressFiles = false;
                SUT _backupEngine = new SUT(_appSettings);

                // Act
                _backupEngine.Backup((object)_pstFileToSave);

                // Assert
                Assert.IsTrue(File.Exists(Path.Combine(_destinationPath, _pstFilename)));
                Assert.IsTrue(AreFilesEquals(_pstFileToSave.SourcePath, Path.Combine(_destinationPath, _pstFilename)));

                DeleteFile(Path.Combine(_destinationPath, _pstFilename));
            }
        }
    }
}
