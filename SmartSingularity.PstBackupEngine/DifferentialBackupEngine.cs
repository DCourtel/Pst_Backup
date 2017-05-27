using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;
using SmartSingularity.PstBackupClientDb;
using SmartSingularity.PstBackupFileSystem;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupEngine
{
    public class DifferentialBackupEngine : CoreBackupEngine
    {
        private string _dbPath = @"E:\Pst Backup\Pst Backup.sqlite3"; // ToDo : Make this path to point to the same folder than the application
        public const int _chunkSize = 1024 * 1024;
        private ClientDb _clientDb;
        private MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();

        public DifferentialBackupEngine(ApplicationSettings appSettings) : base(appSettings)
        {
            _clientDb = new ClientDb(_dbPath);
            if (appSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem)
            {
                _clientDb.Initialize();
            }
        }

#if(DEBUG)
        /// <summary>
        /// This Constructor is used by use cases only
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="dbPath"></param>
        public DifferentialBackupEngine(ApplicationSettings appSettings, string dbPath) : base(appSettings)
        {
            _dbPath = dbPath;
            _clientDb = new ClientDb(_dbPath);
            if (appSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem)
            {
                _clientDb.Initialize();
            }
        }
#endif

        public override void Backup(object objPstFileToSave)
        {
            try
            {
                PSTRegistryEntry pstFileToSave = (PSTRegistryEntry)objPstFileToSave;
                BackupResultInfo backupResult = new BackupResultInfo(pstFileToSave);
                backupResult.IsCompressed = false;

                if (AppSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem)
                {
                    Logger.Write(30011, "Starting to backup " + pstFileToSave.SourcePath + " to file system with differential method", Logger.MessageSeverity.Debug);
                    // Backup to SMB destination
                    int fileId = CheckPrerequisitesForSmbBackup(pstFileToSave.SourcePath);
                    string backupFile = _clientDb.GetBackupFilePath(pstFileToSave.SourcePath);
                    List<string> remoteChunks = _clientDb.GetHashes(fileId);
                    backupResult.ChunkCount = SynchonizeLocalAndRemoteFile(fileId, pstFileToSave.SourcePath, backupFile, remoteChunks);

                    string backupFileNewName = FileSystem.GetNewName(backupFile);
                    FileSystem.RenameFile(backupFile, backupFileNewName);
                    _clientDb.RenameBackupFile(pstFileToSave.SourcePath, backupFileNewName);
                    pstFileToSave.LastSuccessfulBackup = DateTime.Now;
                    pstFileToSave.Save();
                    backupResult.RemotePath = backupFileNewName;
                    backupResult.CompressedSize = 0;
                    backupResult.Result = BackupResultInfo.BackupResult.Failed;
                    backupResult.ErrorMessage = String.Empty;
                    backupResult.BackupEndTime = DateTime.UtcNow;
                    Logger.Write(30012, pstFileToSave + " have been successfuly saved", Logger.MessageSeverity.Debug);
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
                Logger.Write(20024, "An error occurs while backuping a PST file with differential method\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private int CheckPrerequisitesForSmbBackup(string pstFileToSave)
        {
            FileInfo backupFile;
            FileInfo sourceFile = new FileInfo(pstFileToSave);
            int fileId = 0;
            if (_clientDb.IsPstFileRegistered(pstFileToSave))
            {
                // PST file is already known in the database
                fileId = _clientDb.GetFileID(pstFileToSave);
                backupFile = new FileInfo(_clientDb.GetBackupFilePath(pstFileToSave).ToLower());
                if (String.Compare(backupFile.DirectoryName, FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath), true) != 0)
                {
                    // Destination have change
                    backupFile = new FileInfo(Path.Combine(FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath), backupFile.Name));
                    _clientDb.RenameBackupFile(pstFileToSave, backupFile.FullName);
                    _clientDb.DeleteHashes(fileId, 0);
                }
                if (backupFile.Name.EndsWith(".pst"))
                {
                    // Backup File is PST
                    _clientDb.RenameBackupFile(pstFileToSave, backupFile.FullName + ".partial");
                    if (backupFile.Exists)
                    {
                        // Backup.pst exists
                        File.Move(backupFile.FullName, backupFile.FullName + ".partial");
                    }
                    else
                    {
                        // Backup.pst does not exists
                        _clientDb.DeleteHashes(fileId, 0);
                    }
                }
                else
                {
                    // Backup File is Partial
                    if (!backupFile.Exists)
                    {
                        // BackupFile.pst.partial does not exists
                        _clientDb.DeleteHashes(fileId, 0);
                    }
                }
            }
            else
            {
                // PST file is not in the database
                fileId = _clientDb.GetAvailableFileId();
                backupFile = new FileInfo(Path.Combine(FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath), sourceFile.Name + ".partial"));
                _clientDb.RegisterNewPstFile(fileId, pstFileToSave, backupFile.FullName);

                if (backupFile.Exists)
                {
                    // backup.pst.partial already exists
                    backupFile.Delete();
                }
            }

            // Create or Resize
            backupFile = new FileInfo(Path.Combine(FileSystem.ExpandDestinationFolder(AppSettings.FilesAndFoldersDestinationPath), sourceFile.Name + ".partial"));
            if (FileSystem.ResizeFile(backupFile.FullName, sourceFile.Length) == -1)
            {
                // backup file have been shrinked, shrink Hashes table
                _clientDb.ShrinkChunkList(fileId, sourceFile.Length, _chunkSize);
            }
            return fileId;
        }

        private int SynchonizeLocalAndRemoteFile(int fileId, string localFile, string remoteFile, List<string> remoteHashes)
        {
            int byteCount = 0;
            int index = 0;
            byte[] buffer = new byte[_chunkSize];
            int sentChunks = 0;
            string localHash = String.Empty;
            BackupProgressEventArgs progressEventArgs = new BackupProgressEventArgs(0);
            Logger.Write(30013, "Synchronizing " + localFile + " with " + remoteFile, Logger.MessageSeverity.Debug);

            if (!base.IsCancelRequired)
            {
                using (FileStream fileToRead = FileSystem.GetOutlookFile(localFile))
                {
                    using (Stream fileToWrite = File.OpenWrite(remoteFile))
                    {
                        do
                        {
                            if (!base.IsCancelRequired)
                            {
                                byteCount = fileToRead.Read(buffer, 0, _chunkSize);
                                localHash = GetHash(buffer);
                                if (index >= remoteHashes.Count || localHash != remoteHashes[index])
                                {
                                    fileToWrite.Position = (long)index * _chunkSize;
                                    fileToWrite.Write(buffer, 0, byteCount);
                                    sentChunks++;
                                    if (index >= remoteHashes.Count)
                                    {
                                        _clientDb.InsertHash(fileId, index, localHash);
                                    }
                                    else
                                    {
                                        _clientDb.UpdateHash(fileId, index, localHash);
                                    }
                                }
                                index++;
                                progressEventArgs.Percent = (int)(fileToRead.Position * 100 / fileToRead.Length);
                                BackupProgress(progressEventArgs);
                            }
                            else
                            { break; }
                        } while (fileToRead.Position < fileToRead.Length);
                    }
                }
            }
            Logger.Write(30014, "Synchronizing finish. " + sentChunks + " chunk(s) have been sent to the remote destination", Logger.MessageSeverity.Debug);
            return sentChunks;
        }

        private string GetHash(byte[] data)
        {
            return BitConverter.ToString(_md5.ComputeHash(data)).Replace("-", "");
        }
    }
}
