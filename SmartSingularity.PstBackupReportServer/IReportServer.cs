using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace SmartSingularity.PstBackupReportServer
{
    [ServiceContract]
    public interface IReportServer
    {
        [OperationContract]
        void RegisterClient(Client client);

        [OperationContract]
        void RegisterPstFile(string clientId, PstFile pstFile);

        [OperationContract]
        void RegisterBackupResult(string clientId, BackupSession backupSession);
    }
}
