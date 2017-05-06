using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupShutdownComputer
{
    public partial class FrmShutdownComputer : Form
    {
        private Timer _timer = new Timer();
        private int _countdown = 60;

        public FrmShutdownComputer()
        {
            InitializeComponent();

            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _countdown--;
            lblCountdown.Text = this._countdown.ToString();

            if (this._countdown <= 0)
            {
                Shutdown();
            }
        }

        private void Shutdown()
        {
            try
            {
                Logger.Write(14, "The computer will shutdown now", Logger.MessageSeverity.Information);
                _timer.Stop();
                System.Diagnostics.Process.Start("shutdown", "/p /f");
            }
            catch (Exception) { }
        }

        private void btnShutdownNow_Click(object sender, EventArgs e)
        {
            Logger.Write(14, "User request to shutdown now", Logger.MessageSeverity.Information);
            Shutdown();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Logger.Write(13, "Shutdown aborted by user", Logger.MessageSeverity.Information);
            _timer.Stop();
            Close();
        }

        private void FrmShutdownComputer_Shown(object sender, EventArgs e)
        {
            Logger.Write(22, "Countdown to shutdown start", Logger.MessageSeverity.Information);
            _timer.Start();
        }
    }
}
