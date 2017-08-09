using System;
using System.ServiceProcess;
using System.ServiceModel;
using SmartSingularity.PstBackupReportServer;


namespace SmartSingularity.PstBackupWinService
{
    public partial class PstBackupWinService : ServiceBase
    {
        internal static ServiceHost _svcHost = null;

        public PstBackupWinService()
        {
            InitializeComponent();

            this.EventLog.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
            this.EventLog.WriteEntry("Service is starting ...", System.Diagnostics.EventLogEntryType.Information, 0);
            
            if (_svcHost != null)
            {
                _svcHost.Close();
            }
            
            _svcHost = new ServiceHost(typeof(ReportServer), new Uri[] { new Uri(@"http://localhost:43000/Report") });
            _svcHost.Open();            
        }

        protected override void OnStop()
        {
            this.EventLog.WriteEntry("Service is stopping...", System.Diagnostics.EventLogEntryType.Information, 1);

            if (_svcHost != null)
            {
                _svcHost.Close();
                _svcHost = null;
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
