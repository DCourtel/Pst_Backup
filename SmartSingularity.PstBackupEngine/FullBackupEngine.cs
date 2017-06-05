using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;
using SmartSingularity.PstBackupFileSystem;
using SmartSingularity.PstBackupExceptions;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupEngine
{
    public class FullBackupEngine : CoreBackupEngine
    {
        public FullBackupEngine(ApplicationSettings appSettings) : base(appSettings) { }

        public override void Backup(object objPstFileToSave)
        {
            PSTRegistryEntry pstFileToSave = (PSTRegistryEntry)objPstFileToSave;
            BackupResultInfo backupResult = new BackupResultInfo(pstFileToSave);
            backupResult.IsCompressed = AppSettings.FilesAndFoldersCompressFiles;
            try
            {
                Logger.Write(30015, "Starting to backup " + pstFileToSave.SourcePath + " to file system with full method\r\n", Logger.MessageSeverity.Debug);
                // Backup to SMB destination
                backupResult = backupResult.IsCompressed ? BackupWithCompression(pstFileToSave, backupResult) : BackupWithoutCompression(pstFileToSave, backupResult);

                backupResult.ErrorCode = BackupResultInfo.BackupResult.Success;
                backupResult.ErrorMessage = String.Empty;
                pstFileToSave.LastSuccessfulBackup = DateTime.UtcNow;
            }
            catch (BackupCanceledException ex)
            {
                Logger.Write(24, "Backup of " + ex.PstFilename + " have been canceled by the user", Logger.MessageSeverity.Warning);
            }
            catch (NotEnoughEstimatedDiskSpace ex)
            {
                Logger.Write(10002, "Not enough estimated disk space on " + ex.Destination, Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                backupResult.ErrorCode = BackupResultInfo.BackupResult.Failed;
                backupResult.ErrorMessage = "Not enough estimated disk space on " + ex.Destination;
            }
            catch (Exception ex)
            {
                Logger.Write(20025, "An error occurs while saving a PST file with full method\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
                backupResult.ErrorCode = BackupResultInfo.BackupResult.Failed;
                backupResult.ErrorMessage = ex.Message;
            }
            if (!base.IsCancelRequired)
            {
                backupResult.EndTime = DateTime.UtcNow;
                pstFileToSave.Save();
                BackupFinished(new BackupFinishedEventArgs(pstFileToSave, backupResult));
            }
        }

        private BackupResultInfo BackupWithCompression(PSTRegistryEntry pstFileToSave, BackupResultInfo backupResult)
        {
            FileInfo sourceFile = new FileInfo(pstFileToSave.SourcePath);
            string finalDestinationFolder = FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath);

            DirectoryInfo compressionFolder = new DirectoryInfo(FileSystem.GetTemporaryFolder());
            if (HasEnoughDiskspace(compressionFolder, sourceFile.Length / 2))
            {
                string destinationFilePath = Path.Combine(compressionFolder.FullName, sourceFile.Name + ".gz.temp");
                CompressFile(sourceFile.FullName, destinationFilePath);
                CopyFile(destinationFilePath, Path.Combine(finalDestinationFolder, sourceFile.Name + ".gz.temp"), true);
                FileInfo compressedFile = new FileInfo(Path.Combine(finalDestinationFolder, sourceFile.Name + ".gz"));
                FileSystem.RenameFile(Path.Combine(finalDestinationFolder, sourceFile.Name + ".gz.temp"), compressedFile.FullName);
                backupResult.RemotePath = compressedFile.FullName;
                backupResult.CompressedSize = compressedFile.Length;

                try
                {
                    compressionFolder.Delete(true);
                }
                catch (Exception) { }
            }
            else
            {
                throw new NotEnoughEstimatedDiskSpace(compressionFolder.FullName);
            }

            return backupResult;
        }

        private BackupResultInfo BackupWithoutCompression(PSTRegistryEntry pstFileToSave, BackupResultInfo backupResult)
        {
            FileInfo sourceFile = new FileInfo(pstFileToSave.SourcePath);
            string finalDestinationFolder = FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath);

            CopyFile(sourceFile.FullName, Path.Combine(finalDestinationFolder, sourceFile.Name + ".temp"), false);
            FileSystem.RenameFile(Path.Combine(finalDestinationFolder, sourceFile.Name + ".temp"), Path.Combine(finalDestinationFolder, sourceFile.Name));
            backupResult.RemotePath = Path.Combine(finalDestinationFolder, sourceFile.Name);
            backupResult.CompressedSize = backupResult.FileSize;

            return backupResult;
        }

        private void CompressFile(string sourceFilePath, string outputFilePath)
        {
            BackupProgressEventArgs progressEventArgs = new BackupProgressEventArgs(0);
            Logger.Write(30016, "Compressing " + sourceFilePath + " to " + outputFilePath, Logger.MessageSeverity.Debug);

            using (Stream sourceFile = FileSystem.GetOutlookFile(sourceFilePath))
            using (Stream outputFile = new System.IO.Compression.GZipStream(File.Create(outputFilePath), System.IO.Compression.CompressionMode.Compress))
            {
                int bufferLength = 1024 * 1024;
                byte[] buffer = new byte[bufferLength];
                long sourceLength = sourceFile.Length;
                double totalReadBytes = 0;
                int readBytes = 0;
                double percentage = 0;
                while ((readBytes = sourceFile.Read(buffer, 0, bufferLength)) > 0)
                {
                    totalReadBytes += readBytes;
                    percentage = totalReadBytes * 50.0 / sourceLength;

                    outputFile.Write(buffer, 0, readBytes);

                    if (base.IsCancelRequired)
                    {
                        outputFile.Close();
                        (new System.IO.FileInfo(outputFilePath)).Directory.Delete(true);
                        throw new BackupCanceledException(sourceFilePath);
                    }
                    else
                    {
                        progressEventArgs.Percent = (int)percentage;
                        BackupProgress(progressEventArgs);
                    }
                }
            }
            Logger.Write(30017, "Compression completed", Logger.MessageSeverity.Debug);
        }

        private void CopyFile(string sourceFilePath, string outputFilePath, bool isCompressed)
        {
            BackupProgressEventArgs progressEventArgs = new BackupProgressEventArgs(0);
            Logger.Write(30018, "Copying " + sourceFilePath + " to " + outputFilePath, Logger.MessageSeverity.Debug);

            using (Stream sourceFile = FileSystem.GetOutlookFile(sourceFilePath))
            using (Stream outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                int bufferLength = 1024 * 1024;
                byte[] buffer = new byte[bufferLength];
                long sourceLength = sourceFile.Length;
                double totalReadBytes = 0;
                int readBytes = 0;
                double percentage = 0;
                while ((readBytes = sourceFile.Read(buffer, 0, bufferLength)) > 0)
                {
                    totalReadBytes += readBytes;
                    if (isCompressed)
                    { percentage = 50.0 + totalReadBytes * 50.0 / sourceLength; }
                    else
                    { percentage = totalReadBytes * 100.0 / sourceLength; }

                    outputFile.Write(buffer, 0, readBytes);

                    if (base.IsCancelRequired)
                    {
                        outputFile.Close();
                        (new System.IO.FileInfo(outputFilePath)).Directory.Delete(true);
                        throw new BackupCanceledException(sourceFilePath);
                    }
                    else
                    {
                        progressEventArgs.Percent = (int)percentage;
                        BackupProgress(progressEventArgs);
                    }
                }
            }
            Logger.Write(30019, "Copy completed", Logger.MessageSeverity.Debug);
        }

        /// <summary>
        /// Compare the free diskspace on the destination disk and the size of a file to know if there is enough diskspace to host the file
        /// </summary>
        /// <param name="destinationFolder">A folder on the hard drive to check</param>
        /// <param name="fileSize">The size of a file that could be host on the hard drive</param>
        /// <returns>Returns true if there is enough diskspace on the drive to host a file of the provided size</returns>
        private bool HasEnoughDiskspace(DirectoryInfo destinationFolder, long fileSize)
        {
            try
            {
                DriveInfo localDrive = new DriveInfo(destinationFolder.Root.ToString());
                return localDrive.AvailableFreeSpace > fileSize;
            }
            catch (Exception ex)
            {
                Logger.Write(10003, "Unable to determine if there is enough diskspace on " + destinationFolder + "\r\n" + ex.Message, Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
            }
            return true;
        }
    }
}
