using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupAddin
{
    public partial class PstBackupRibbon
    {
        private void PstBackupRibbon_Load(object sender, RibbonUIEventArgs e)        {        }

        private void btnSettings_Click(object sender, RibbonControlEventArgs e)
        {
            Logger.Write(30003, "User have request to show settings form.", Logger.MessageSeverity.Debug);
            ThisAddIn.UpdateRegistryEntries();
            SmartSingularity.PstBackupSettings.FrmSettings settings = new PstBackupSettings.FrmSettings();

            settings.ShowDialog();
        }
    }
}
