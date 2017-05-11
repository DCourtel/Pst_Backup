using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SmartSingularity.PstBackupReportServer
{
    [DataContract]
    public class PstFile
    {
        /// <summary>
        /// Gets or Sets the full path to the PST file on the client computer
        /// </summary>
        [DataMember]
        public string LocaPath { get; set; }    

        /// <summary>
        /// Gets or Sets the full path to the PST file once saved
        /// </summary>
        [DataMember]
        public string RemotePath { get; set; }  

        /// <summary>
        /// Gets or Sets the size of the PST file
        /// </summary>
        [DataMember]
        public long Size { get; set; }  

        /// <summary>
        /// Gets or Sets if the saved file is compressed
        /// </summary>
        [DataMember]
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or Sets if the PST file is set to be backup
        /// </summary>
        [DataMember]
        public bool NeedBackup { get; set; }

        /// <summary>
        /// Gets or Sets if the PST file is scheduled to be saved
        /// </summary>
        [DataMember]
        public bool IsScheduled { get; set; }   

        /// <summary>
        /// Gets or Sets the size of the PST file
        /// </summary>
        [DataMember]
        public long CompressedSize { get; set; }

        /// <summary>
        /// Gets or Sets the number of chunk that have been sent during a differential backup
        /// </summary>
        [DataMember]
        public int ChunksSent { get; set; }
    }
}
