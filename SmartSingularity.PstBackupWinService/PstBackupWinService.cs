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

            string databasePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            databasePath = System.IO.Path.Combine(databasePath, "PstBackup" ,"PstBackup.mdf");
            this.EventLog.WriteEntry($"Database is located at : {databasePath}", System.Diagnostics.EventLogEntryType.Information);
            _server = new ReportServer(databasePath);
        }

        protected override void OnStop()
        {
            this.EventLog.WriteEntry("Service is stopping...", System.Diagnostics.EventLogEntryType.Information, 1);
            _server?.Dispose();
            _server = null;
        }

        protected override void OnShutdown()
        {
            this.EventLog.WriteEntry("Service is stopping because the computer is shutting down...", System.Diagnostics.EventLogEntryType.Information, 2);
            this.OnStop();
            this.ExitCode = 0;
        }
    }
}
