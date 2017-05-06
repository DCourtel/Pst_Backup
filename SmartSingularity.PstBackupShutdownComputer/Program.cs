using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartSingularity.PstBackupShutdownComputer
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNew = true;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, "FrmShutdownComputer", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    FrmShutdownComputer instance = new FrmShutdownComputer();
                    Application.Run(instance);
                }
            }
        }
    }
}
