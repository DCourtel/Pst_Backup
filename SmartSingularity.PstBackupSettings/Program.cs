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
            // ToDo : Avoid launching multiple instance of this form
            Application.Run(new FrmSettings());
        }
    }
}
