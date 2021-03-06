﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;

namespace SmartSingularity.PstBackupEngine
{
    public class BackupResultInfo
    {
        public enum BackupResult
        {
            NotStarted = -1,
            Success = 0,
            Canceled = 10,
            Failed = 20,
            Postponed = 30
        }

        public BackupResultInfo(PSTRegistryEntry pstFile)
        {
            System.IO.FileInfo fileToSave = new System.IO.FileInfo(pstFile.SourcePath);
            LocalPath = fileToSave.FullName;
            FileSize = fileToSave.Length;
            ChunkCount = 0;
            StartTime = DateTime.UtcNow;
            ErrorCode = BackupResult.NotStarted;
            ErrorMessage = "An unknown error occurs.";
        }

        #region (Properties)

        /// <summary>
        /// Gets or Sets the full path to the PST file that have been saved
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or Sets the full path to the backup file
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// Gets or Sets the UTC time when the backup start
        /// </summary>
        public DateTime StartTime { get; set; }   

        /// <summary>
        /// Gets or Sets the UTC time when the backup ends
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or Sets the size of the PST file
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or Sets the number of chunks that have been sent to the remote destination in case of a differential backup
        /// </summary>
        public int ChunkCount { get; set; }

        /// <summary>
        /// Gets or Sets if the PST file have been compressed during the backup
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or Sets the size of the compressed file
        /// </summary>
        public long CompressedSize { get; set; }

        /// <summary>
        /// Gets or Sets the result of the backup operation
        /// </summary>
        public BackupResult ErrorCode { get; set; }

        /// <summary>
        /// Gets or Sets the error message in case where the backup have failed
        /// </summary>
        public string ErrorMessage { get; set; }

        #endregion (Properties)
    }
}
