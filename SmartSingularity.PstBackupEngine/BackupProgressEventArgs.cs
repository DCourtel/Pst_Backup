using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSingularity.PstBackupEngine
{
    public class BackupProgressEventArgs:EventArgs
    {
        /// <summary>
        /// Create an instance of the event args
        /// </summary>
        /// <param name="currentFilename">Full name of the file that is beening saved</param>
        /// <param name="percent">Percentage of the progression of the current backup (0…100)</param>
        public BackupProgressEventArgs(int percent)
        {
            Percent = percent;
        }

        /// <summary>
        /// Percentage of the progression of the current backup (0…100)
        /// </summary>
        public int Percent { get; set; }
    }
}
