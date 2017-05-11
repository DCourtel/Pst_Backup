using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;
using SmartSingularity.PstBackupFileSystem;
using SmartSingularity.PstBackupNetwork;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupEngine
{
    public class CoreBackupEngine
    {
        private ApplicationSettings _appSettings;
        private bool _cancelRequired = false;
        private object _cancelLock = new object();

        /// <summary>
        /// Gets or Sets if the backup should be interrupt immediately
        /// </summary>
        public bool IsCancelRequired
        {
            get
            {
                lock (_cancelLock)
                { return _cancelRequired; }
            }
            set
            {
                lock (_cancelLock)
                { _cancelRequired = value; }
            }
        }

        public ApplicationSettings AppSettings { get { return _appSettings; } }

        public CoreBackupEngine(ApplicationSettings appSettings)
        {
            _appSettings = appSettings;
            if (appSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem && !Directory.Exists(FileSystem.ExpandDestinationFolder(appSettings.FilesAndFoldersDestinationPath)))
            {
                Logger.Write(30010, "Creating backup folder\r\n" + appSettings.FilesAndFoldersDestinationPath, Logger.MessageSeverity.Debug);
                FileSystem.CreateDestinationFolder(appSettings.FilesAndFoldersDestinationPath, appSettings.BackupAgentSetExclusiveNTFSPermissions, appSettings.BackupAgentAdditionalNTFSFullcontrol, appSettings.BackupAgentAdditionalNTFSReadWrite);
            }
        }

        public virtual void Backup(object pstFileToSave) { }

        /// <summary>
        /// Returns an instance of class that implement the correct backup method based on the settings
        /// </summary>
        /// <param name="appSettings">Settings of the application</param>
        /// <returns>Returns an instance of IBackupEngine</returns>
        public static CoreBackupEngine GetBackupEngine(ApplicationSettings appSettings)
        {
            switch (appSettings.BackupAgentBackupMethod)
            {
                case ApplicationSettings.BackupMethod.Full:
                    return new FullBackupEngine(appSettings);
                case ApplicationSettings.BackupMethod.Differential:
                    return new DifferentialBackupEngine(appSettings);
                default:
                    Logger.Write(10001, "Unable to determine the backup method\r\n" + appSettings.BackupAgentBackupMethod, Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                    return new FullBackupEngine(appSettings);
            }
        }

        /// <summary>
        /// Select which PST files need to be saved based on the last successful backup and the backup policy.
        /// </summary>
        /// <param name="allPstFiles">A list of PST File to browse</param>
        /// <param name="pstFilesToSave">A list of PST File that need to be saved</param>
        /// <param name="pstFilesToNotSave">A list of PST File that do not need to be saved</param>
        public void SelectPstFilesToSave(List<PSTRegistryEntry> allPstFiles, out List<PSTRegistryEntry> pstFilesToSave, out List<PSTRegistryEntry> pstFilesToNotSave)
        {
            pstFilesToSave = new List<PSTRegistryEntry>();
            pstFilesToNotSave = new List<PSTRegistryEntry>();

            foreach (var pstFile in allPstFiles)
            {
                if (pstFile.ToBackup &&
                    Scheduler.IsPstFileNeedtoBeSaved(pstFile.LastSuccessfulBackup, _appSettings) &&
                    !Network.IsWanLink(_appSettings.FilesAndFoldersDestinationPath, _appSettings.BackupAgentAdditionalSubnets))
                {
                    Logger.Write(20, pstFile + "\r\n have been added to the list of files to save", Logger.MessageSeverity.Information);
                    pstFilesToSave.Add(pstFile);
                }
                else
                {
                    Logger.Write(21, pstFile + "\r\n have been added to the list of files to not save", Logger.MessageSeverity.Information);
                    pstFilesToNotSave.Add(pstFile);
                }
            }
        }
        
        public virtual void BackupFinished(BackupFinishedEventArgs e)
        {
            EventHandler<BackupFinishedEventArgs> handler = OnBackupFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void BackupProgress(BackupProgressEventArgs e)
        {
            EventHandler<BackupProgressEventArgs> handler = OnBackupProgress;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region (Event Delegates)

        public event EventHandler<BackupFinishedEventArgs> OnBackupFinished;
        public event EventHandler<BackupProgressEventArgs> OnBackupProgress;


        #endregion (Event Delegates)
    }
}
