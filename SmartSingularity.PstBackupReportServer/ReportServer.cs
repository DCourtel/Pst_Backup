using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSingularity.PstBackupReportServer
{
    public class ReportServer : IReportServer
    {
        /// <summary>
        /// Register the result of a backup of a PST file into the database
        /// </summary>
        /// <param name="pstFile">All informations on the PST file that have been saved</param>
        /// <param name="backupSession">Informations on the backup session</param>
        public void RegisterBackupResult(PstFile pstFile, BackupSession backupSession)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register new PstBackup Client in the database or update the record if the client already exists
        /// </summary>
        /// <param name="client">All informations on the client computer</param>
        public void RegisterClient(Client client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register, in the database, all informations on PST files mounted in Outlook
        /// </summary>
        /// <param name="pstFiles">A list of all PST files</param>
        public void RegisterPstFiles(List<PstFile> pstFiles)
        {
            throw new NotImplementedException();
        }
    }
}
