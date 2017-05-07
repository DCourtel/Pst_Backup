using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSingularity.PstBackupExceptions
{
    public class BackupCanceledException:Exception
    {
        public BackupCanceledException(string pstFilename)
        {
            PstFilename = pstFilename;
        }

        /// <summary>
        /// Full path of the file that was being processed when cancel reqest occurred
        /// </summary>
        public string PstFilename { get; private set; }
    }
}
