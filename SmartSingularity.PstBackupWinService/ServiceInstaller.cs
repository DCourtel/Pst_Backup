using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace SmartSingularity.PstBackupWinService
{
    [RunInstaller(true)]
    public class ServiceInstaller:Installer
    {
        public ServiceInstaller()
        {
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            System.ServiceProcess.ServiceInstaller serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            processInstaller.Username = null;
            processInstaller.Password = null;

            serviceInstaller.DisplayName = "PstBackup Server";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = true;

            serviceInstaller.ServiceName = "PstBackupServer";
            serviceInstaller.Description = "Handles informations received from clients and writes it into the database.";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}