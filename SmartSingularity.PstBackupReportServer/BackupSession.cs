using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SmartSingularity.PstBackupEngine;
using Settings = SmartSingularity.PstBackupSettings.ApplicationSettings;

namespace SmartSingularity.PstBackupReportServer
{
    [DataContract]
    public class BackupSession
    {
        /// <summary>
        /// Gets or Sets the full path to the PST file on the client computer
        /// </summary>
        [DataMember]
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or Sets the full path to the PST file once saved
        /// </summary>
        [DataMember]
        public string RemotePath { get; set; }

        /// <summary>
        /// Gets or Sets if the saved file is compressed
        /// </summary>
        [DataMember]
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or Sets the compressed size of the PST file.
        /// </summary>
        [DataMember]
        public long CompressedSize { get; set; }

        /// <summary>
        /// Gets or Sets the method used to backup the PST file
        /// </summary>
        [DataMember]
        public Settings.BackupMethod BackupMethod { get; set; }

        /// <summary>
        /// Gets or Sets the type of destination used during the backup
        /// </summary>
        [DataMember]
        public Settings.BackupDestinationType DestinationType { get; set; }

        /// <summary>
        /// Gets or Sets if the PST file is schedule for a backup
        /// </summary>
        [DataMember]
        public bool IsSchedule { get; set; }

        /// <summary>
        /// Gets or Sets the date when the backup started
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or Sets the date when the backup ended.
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or Sets the number of chunk that have been sent during a differential backup.
        /// </summary>
        [DataMember]
        public int ChunkCount { get; set; }

        /// <summary>
        /// Gets or Sets the final result of the backup
        /// </summary>
        [DataMember]
        public BackupResultInfo.BackupResult ErrorCode { get; set; }

        /// <summary>
        /// Gets or Sets the error message if there is one.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
    }
}
