using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;
using SmartSingularity.PstBackupFileSystem;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupEngine
{
    public class FullBackupEngine : CoreBackupEngine
    {
        public FullBackupEngine(ApplicationSettings appSettings) : base(appSettings) { }

        public override void Backup(object objPstFileToSave)
        {
            try
            {
                PSTRegistryEntry pstFileToSave = (PSTRegistryEntry)objPstFileToSave;
                BackupResult backupResult = new BackupResult(pstFileToSave);

                if (AppSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem)
                {
                    // Backup to SMB destination
                    Logger.Write(30015, "Starting to backup " + pstFileToSave.SourcePath + " to file system with full method\r\n", Logger.MessageSeverity.Debug);
                    if (!base.IsCancelRequired)
                    {
                        FileInfo sourceFile = new FileInfo(pstFileToSave.SourcePath);
                        string finalDestinationFolder = FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath);

                        if (AppSettings.FilesAndFoldersCompressFiles)
                        {
                            backupResult.IsCompressed = true;
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
                                backupResult.HasFailed = false;
                                backupResult.ErrorMessage = String.Empty;
                                backupResult.BackupEndTime = DateTime.UtcNow;
                                pstFileToSave.LastSuccessfulBackup = DateTime.UtcNow;

                                try
                                {
                                    compressionFolder.Delete(true);
                                }
                                catch (Exception) { }
                            }
                            else
                            {
                                Logger.Write(10002, "Not enough disk space on the remot folder", Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                                backupResult.RemotePath = compressionFolder.FullName;
                                backupResult.HasFailed = true;
                                backupResult.ErrorMessage = "Not enough diskspace on the remote folder.";
                                backupResult.BackupEndTime = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            backupResult.IsCompressed = false;
                            CopyFile(sourceFile.FullName, Path.Combine(finalDestinationFolder, sourceFile.Name + ".temp"), false);
                            FileSystem.RenameFile(Path.Combine(finalDestinationFolder, sourceFile.Name + ".temp"), Path.Combine(finalDestinationFolder, sourceFile.Name));
                            backupResult.RemotePath = Path.Combine(finalDestinationFolder, sourceFile.Name);
                            backupResult.CompressedSize = 0;
                            backupResult.HasFailed = false;
                            backupResult.ErrorMessage = String.Empty;
                            backupResult.BackupEndTime = DateTime.UtcNow;
                            pstFileToSave.LastSuccessfulBackup = DateTime.UtcNow;
                        }
                        pstFileToSave.Save();
                        Logger.Write(30012, "PST file have been successfuly saved", Logger.MessageSeverity.Debug);
                    }
                }
                else
                {
                    // Backup to Server

                }
                if (!base.IsCancelRequired)
                {
                    BackupFinished(new BackupFinishedEventArgs(pstFileToSave, backupResult));
                }
            }
            catch (Exception ex)
            {
                Logger.Write(20025, "An error occurs while backuping a PST file with full method\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
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
                    progressEventArgs.Percent = (int)percentage;
                    BackupProgress(progressEventArgs);

                    if (base.IsCancelRequired)
                    {
                        outputFile.Close();
                        File.Delete(outputFilePath);
                        break;
                    }
                }
            }
            Logger.Write(30017, "Compression finished", Logger.MessageSeverity.Debug);
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
                    progressEventArgs.Percent = (int)percentage;
                    BackupProgress(progressEventArgs);

                    if (base.IsCancelRequired)
                    {
                        outputFile.Close();
                        File.Delete(outputFilePath);
                        break;
                    }
                }
            }
            Logger.Write(30019, "Copy finished", Logger.MessageSeverity.Debug);
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
