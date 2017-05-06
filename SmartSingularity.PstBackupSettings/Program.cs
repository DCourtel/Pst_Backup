using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SmartSingularity.PstBackupSettings
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
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, "PstBackupSettings", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    FrmSettings instance = new FrmSettings();
                    Application.Run(instance);
                }
            }
        }
    }
}
