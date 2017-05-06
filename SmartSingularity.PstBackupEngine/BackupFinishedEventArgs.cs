using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupSettings;

namespace SmartSingularity.PstBackupEngine
{
    public class BackupFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Create an instance of the event args
        /// </summary>
        /// <param name="pstRegistryEntry">A reference to the PstRegistryEntry</param>
        /// <param name="backupResult">A reference to the Result of the backup</param>
        public BackupFinishedEventArgs(PSTRegistryEntry pstRegistryEntry, BackupResult backupResult)
        {
            PstRegistryEntry = pstRegistryEntry;
            Result = backupResult;
        }

        /// <summary>
        /// A reference to the PstRegistryEntry
        /// </summary>
        public PSTRegistryEntry PstRegistryEntry { get; private set; }

        /// <summary>
        /// Gets or Sets the result of the backup
        /// </summary>
        public BackupResult Result { get; private set; }
    }
}
