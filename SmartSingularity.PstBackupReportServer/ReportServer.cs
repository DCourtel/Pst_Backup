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

#if (DEBUG)
        public ReportServer()
        {
            _reportServerDb = new ReportServerDb(@"E:\Pst Backup\Test Files\ReportServerDb\Test-PstBackup.mdf");
            _reportServerDb.Connect();
        }
#endif

#if (!DEBUG)
        public ReportServer()
        {
            // ToDo : Handles Log Settings
            Logger.IsLogActivated = true;
            Logger.MinimalSeverity = Logger.MessageSeverity.Debug;
            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            databasePath = System.IO.Path.Combine(databasePath, "PstBackup", "PstBackup.mdf");
            Logger.Write(1, $"Démarrage de ReportServer. Database at {databasePath}", Logger.MessageSeverity.Information);
            _reportServerDb = new ReportServerDb(databasePath);
            _reportServerDb.Connect();
        } 
#endif

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
        /// <param name="clientId">Unique Id of the client that own the PST file</param>
        /// <param name="backupSession">Informations on the backup session</param>
        public void RegisterBackupResult(string clientId, BackupSession backupSession)
        {
            Logger.Write(30022, $"Logging backup result for {backupSession.LocalPath}", Logger.MessageSeverity.Debug);
            _reportServerDb.LogBackupResult(clientId, backupSession);                
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
        /// <param name="clientId">Unique Id of the client that own the PST file</param>
        /// <param name="backupSession">Informations on the backup session</param>
        public void RegisterPstFile(string clientId, PstFile pstFile)
        {
            Logger.Write(30021, $"Registering the PST file {pstFile.LocalPath}", Logger.MessageSeverity.Debug);
            _reportServerDb.RegisterPstFile(clientId, pstFile);
        }
    }
}
