using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SmartSingularity.PstBackupReportServer
{
    [DataContract]
    public class Client
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
    }
}
