using System;
using System.ServiceProcess;
using SmartSingularity.PstBackupReportServer;


namespace SmartSingularity.PstBackupWinService
{
    public partial class PstBackupWinService : ServiceBase
    {
        private ReportServer _server;

        public PstBackupWinService()
        {
            InitializeComponent();
            
            this.EventLog.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
           this.EventLog.WriteEntry("Service is starting ...", System.Diagnostics.EventLogEntryType.Information, 0);
            try
            {
                string databasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                databasePath = System.IO.Path.Combine(databasePath, "PstBackup.mdf");
                this.EventLog.WriteEntry($"Database is located at : {databasePath}",  System.Diagnostics.EventLogEntryType.Information);
                _server = new ReportServer(databasePath);
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry("An error occurred while trying to start Server : " + ex.Message, System.Diagnostics.EventLogEntryType.Error, 10);
                this.Stop();
                this.ExitCode = -1;
                throw;
            }
        }

        protected override void OnStop()
        {
            this.EventLog.WriteEntry("Service is stopping...", System.Diagnostics.EventLogEntryType.Information, 1);
            try
            {
                _server?.Dispose();
                _server = null;
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry("An error occured while trying to stop Server : " + ex.Message, System.Diagnostics.EventLogEntryType.Error, 11);
            }
        }

        protected override void OnShutdown()
        {
            this.EventLog.WriteEntry("Service is stopping because the computer is shutting down...", System.Diagnostics.EventLogEntryType.Information, 2);
            this.OnStop();
            this.ExitCode = 0;
        }
    }
}
