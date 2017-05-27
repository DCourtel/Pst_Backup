using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SmartSingularity.PstBackupReportServer
{
    [DataContract]
    public class PstFile:IEquatable<PstFile>
    {
        /// <summary>
        /// Gets or Sets the full path to the PST file on the client computer
        /// </summary>
        [DataMember]
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or Sets the unique Id of the file.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or Sets if the PST file is set to be backup
        /// </summary>
        [DataMember]
        public bool IsSetToBackup { get; set; }

        /// <summary>
        /// Gets or Sets the size of the PST file
        /// </summary>
        [DataMember]
        public long Size { get; set; }

        /// <summary>
        /// Date of the last successful backup. Null if the file have never been successfully saved.
        /// </summary>
        [DataMember]
        public DateTime? LastSuccessfulBackup {get; set;}

        public bool Equals(PstFile other)
        {
            return (String.Compare(this.LocalPath, other.LocalPath, false)==0 && 
                this.Id.Equals(other.Id) && 
                this.IsSetToBackup == other.IsSetToBackup && 
                this.Size == other.Size &&
                this.LastSuccessfulBackup == other.LastSuccessfulBackup);
        }
    }
}
