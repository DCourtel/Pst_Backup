using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupReportServer
{
    public class ReportServer : IReportServer, IDisposable
    {
        private ReportServerDb _reportServerDb;

        public ReportServer()
        {
            _reportServerDb = new ReportServerDb(@"E:\Pst Backup\Test Files\ReportServerDb\Test-PstBackup.mdf");
            _reportServerDb.Connect();
        }

        public ReportServer(string dbPath)
        {
            _reportServerDb = new ReportServerDb(dbPath);
            _reportServerDb.Connect();
        }

        public void Dispose()
        {
            try
            {
                _reportServerDb.Disconnect();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Register the result of a backup of a PST file into the database
        /// </summary>
        /// <param name="pstFile">All informations on the PST file that have been saved</param>
        /// <param name="backupSession">Informations on the backup session</param>
        public void RegisterBackupResult(PstFile pstFile, BackupSession backupSession)
        {
            //ToDo: Implement this method
        }

        /// <summary>
        /// Register new PstBackup Client in the database or update the record if the client already exists
        /// </summary>
        /// <param name="client">All informations on the client computer</param>
        public void RegisterClient(Client client)
        {
            Logger.Write(30020, $"Registering the client {client.ComputerName}\\{client.Username} [{client.Version.ToString()}]", Logger.MessageSeverity.Debug);
            _reportServerDb.RegisterClient(client);
        }

        /// <summary>
        /// Register, in the database, all informations on the PST file mounted in Outlook
        /// </summary>
        /// <param name="pstFile">A PST file</param>
        /// <param name="backupSession">Informations on the backup session</param>
        public void RegisterPstFile(string clientId, PstFile pstFile)
        {
            Logger.Write(30021, $"Registering the PST file {pstFile.LocaPath}", Logger.MessageSeverity.Debug);
            _reportServerDb.RegisterPstFile(clientId, pstFile);
        }
    }
}
