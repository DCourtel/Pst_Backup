using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SmartSingularity.PstBackupReportServer
{
    [DataContract]
    public class Client:IEquatable<Client>
    {
        /// <summary>
        /// Gets or Sets the unique ID of the client
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets the version of PST Backup on this client computer
        /// </summary>
        [DataMember]
        public Version Version {get; set; }

        /// <summary>
        /// Gets or Sets the name of the computer
        /// </summary>
        [DataMember]
        public string ComputerName { get; set; }    

        /// <summary>
        /// Gets or Sets the name of the user
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Gets or Sets the Date Time of the last contact
        /// </summary>
        [DataMember]
        public DateTime LastContactDate { get; set; }

        public bool Equals(Client other)
        {
            return String.Compare(this.Id, other.Id, true) == 0 &&
                this.Version.Equals(other.Version) &&
                String.Compare(this.ComputerName, other.ComputerName, true) == 0 &&
                String.Compare(this.Username, other.Username, true) == 0;
        }
    }
}
