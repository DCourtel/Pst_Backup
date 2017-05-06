using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;

namespace SmartSingularity.PstBackupEngine
{
    public class BackupResult
    {
        public BackupResult(PSTRegistryEntry pstFile)
        {
            System.IO.FileInfo fileToSave = new System.IO.FileInfo(pstFile.SourcePath);
            LocalPath = fileToSave.FullName;
            FileSize = fileToSave.Length;
            SentChunks = 0;
            BackupStartTime = DateTime.UtcNow;
            HasFailed = true;
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
        public DateTime BackupStartTime { get; set; }   

        /// <summary>
        /// Gets or Sets the UTC time when the backup ends
        /// </summary>
        public DateTime BackupEndTime { get; set; }

        /// <summary>
        /// Gets or Sets the size of the PST file
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or Sets the number of chunks that have been sent to the remote destination in case of a differential backup
        /// </summary>
        public int SentChunks { get; set; }

        /// <summary>
        /// Gets or Sets if the PST file have been compressed during the backup
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or Sets the size of the compressed file
        /// </summary>
        public long CompressedSize { get; set; }

        /// <summary>
        /// Gets or Sets if the backup have failed
        /// </summary>
        public bool HasFailed { get; set; }

        /// <summary>
        /// Gets or Sets the error message in case where the backup have failed
        /// </summary>
        public string ErrorMessage { get; set; }

        #endregion (Properties)
    }
}
