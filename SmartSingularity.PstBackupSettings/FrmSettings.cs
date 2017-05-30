using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartSingularity.PstBackupSettings
{
    public partial class FrmSettings : Form
    {
        private ApplicationSettings _localSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
        private ApplicationSettings _gpoSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.GPO);

        public FrmSettings()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");

            InitializeComponent();

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = "Pst Backup - v" + version.ToString();

            SetDaysOfWeek();    // Fill the comboBox with days of week in the langage of the current user

            cmbBxEvery.SelectedIndex = 0;
            cmbBxWeekly.SelectedIndex = 0;
            cmbBxEventLogSeverity.SelectedIndex = 0;
        }

        #region (Methods)

        /// <summary>
        /// Fill the comboBox with days of week in the langage of the current user
        /// </summary>
        private void SetDaysOfWeek()
        {
            for (int i = 0; i < 7; i++)
            {
                this.cmbBxWeekly.Items.Add(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames[i]);
            }
        }

        /// <summary>
        /// Read regitry entries to find PST files mount into Outlook
        /// </summary>
        private void LoadPstRegistryEntries()
        {
            chkLstBxPstFiles.Items.Clear();
            List<PSTRegistryEntry> regEntries = ApplicationSettings.GetPstRegistryEntries();

            foreach (PSTRegistryEntry regEntry in regEntries)
            {
                if (!String.IsNullOrWhiteSpace(regEntry.SourcePath))
                    chkLstBxPstFiles.Items.Add(regEntry, regEntry.ToBackup);
            }
        }

        /// <summary>
        /// Setup the UI as required by Settings
        /// </summary>
        private void SetupUI()
        {
            // Files and Folders

            try
            {
                rdBtnFileSystem.Checked = _localSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem;
                rdBtnBackupServer.Checked = _localSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.BackupServer;
                txtBxDestination.Text = _localSettings.FilesAndFoldersDestinationPath;
                txtBxBackupServerName.Text = _localSettings.FilesAndFoldersBackupServer;
                nupBackupServerPort.Value = (decimal)_localSettings.FilesAndFoldersBackupPort;
                chkBxCompressFile.Checked = _localSettings.FilesAndFoldersCompressFiles && rdBtnFileSystem.Checked && (_localSettings.BackupAgentBackupMethod == ApplicationSettings.BackupMethod.Full);
                chkBxCompressFile.Enabled = rdBtnFileSystem.Checked && (_localSettings.BackupAgentBackupMethod == ApplicationSettings.BackupMethod.Full);
            }
            catch (Exception) { }

            // Schedule

            try
            {
                switch (_localSettings.SchedulePolicy)
                {
                    case ApplicationSettings.BackupPolicy.EveryX:
                        rdBtnEvery.Checked = true;
                        rdBtnWeekly.Checked = false;
                        rdBtnMonthly.Checked = false;
                        nupEvery.Value = (decimal)_localSettings.ScheduleInterval;
                        cmbBxEvery.SelectedIndex = (int)_localSettings.ScheduleUnit;
                        cmbBxWeekly.SelectedIndex = (int)_localSettings.ScheduleDayOfWeek;
                        nupMonthly.Value = (decimal)_localSettings.ScheduleDayOfMonth;
                        break;
                    case ApplicationSettings.BackupPolicy.Weekly:
                        rdBtnEvery.Checked = false;
                        rdBtnWeekly.Checked = true;
                        rdBtnMonthly.Checked = false;
                        nupEvery.Value = (decimal)_localSettings.ScheduleInterval;
                        cmbBxEvery.SelectedIndex = (int)_localSettings.ScheduleUnit;
                        cmbBxWeekly.SelectedIndex = (int)_localSettings.ScheduleDayOfWeek;
                        nupMonthly.Value = (decimal)_localSettings.ScheduleDayOfMonth;
                        break;
                    default:
                        rdBtnEvery.Checked = false;
                        rdBtnWeekly.Checked = false;
                        rdBtnMonthly.Checked = true;
                        nupEvery.Value = (decimal)_localSettings.ScheduleInterval;
                        cmbBxEvery.SelectedIndex = (int)_localSettings.ScheduleUnit;
                        cmbBxWeekly.SelectedIndex = (int)_localSettings.ScheduleDayOfWeek;
                        nupMonthly.Value = (decimal)_localSettings.ScheduleDayOfMonth;
                        break;
                }
            }
            catch (Exception) { }

            // Event Log

            try
            {
                chkBxEventLog.Checked = _localSettings.EventLogActivated;
                cmbBxEventLogSeverity.SelectedIndex = (int)_localSettings.EventLogSeverity;
            }
            catch (Exception) { }

            // Reporting

            try
            {
                chkBxReporting.Checked = _localSettings.ReportingReportToServer;
                txtBxReportingServerName.Text = _localSettings.ReportingServer;
                nupReportingServerPort.Value = (decimal)_localSettings.ReportingPort;
            }
            catch (Exception) { }

            // Backup Agent
            try
            {
                rdBtnMethodFull.Checked = _localSettings.BackupAgentBackupMethod == 0;
                rdBtnMethodDifferential.Checked = _localSettings.BackupAgentBackupMethod == ApplicationSettings.BackupMethod.Differential;
                chkBxDontBackupOverWan.Checked = _localSettings.BackupAgentDontBackupThroughtWan;
                txtBxAdditionalSubnets.Text = _localSettings.BackupAgentAdditionalSubnets;
            }
            catch (Exception) { }
        }

        /// <summary>
        ///  Disable controls that must not be edit as the setting is controled by GPO
        /// </summary>
        private void LockUIasRequiredByGPO()
        {
            // Files And Folders

            try
            {
                if (_gpoSettings.IsFilesAndFoldersDestinationTypeDefine)
                {
                    rdBtnFileSystem.Enabled = false;
                    rdBtnBackupServer.Enabled = false;
                }
                txtBxDestination.Enabled = rdBtnFileSystem.Checked && !_gpoSettings.IsFilesAndFoldersDestinationPathDefine;
                txtBxBackupServerName.Enabled = rdBtnBackupServer.Checked && !_gpoSettings.IsFilesAndFoldersBackupServerDefine;
                nupBackupServerPort.Enabled = rdBtnBackupServer.Checked && !_gpoSettings.IsFilesAndFoldersBackupPortDefine;
                chkBxCompressFile.Enabled = !_gpoSettings.IsFilesAndFoldersCompressFilesDefine && (_localSettings.FilesAndFoldersDestinationType == ApplicationSettings.BackupDestinationType.FileSystem) && (_localSettings.BackupAgentBackupMethod == ApplicationSettings.BackupMethod.Full);
                if(_gpoSettings.FilesAndFoldersBackupAllPst)
                {
                    SelectAllPstFiles();
                    chkLstBxPstFiles.Enabled = false;
                }
            }
            catch (Exception) { }

            // Schedule

            try
            {
                if (_gpoSettings.IsSchedulePolicyDefine)
                {
                    rdBtnEvery.Enabled = false;
                    rdBtnWeekly.Enabled = false;
                    rdBtnMonthly.Enabled = false;
                }
                nupEvery.Enabled = rdBtnEvery.Checked && !_gpoSettings.IsScheduleintervalDefine;
                cmbBxEvery.Enabled = rdBtnEvery.Checked && !_gpoSettings.IsScheduleUnitDefine;
                cmbBxWeekly.Enabled = rdBtnWeekly.Checked && !_gpoSettings.IsScheduleDayOfWeekDefine;
                nupMonthly.Enabled = rdBtnMonthly.Checked && !_gpoSettings.IsScheduleDayOfMonthDefine;
            }
            catch (Exception) { }

            // Event Log

            try
            {
                chkBxEventLog.Enabled = !_gpoSettings.IsEventLogActivatedDefine;
                cmbBxEventLogSeverity.Enabled = !_gpoSettings.IsEventLogSeverityDefine && chkBxEventLog.Checked;
            }
            catch (Exception) { }

            // Report

            try
            {
                chkBxReporting.Enabled = !_gpoSettings.IsReportingDefine;
                txtBxReportingServerName.Enabled = !_gpoSettings.IsReportingServerDefine && chkBxReporting.Checked;
                nupReportingServerPort.Enabled = !_gpoSettings.IsReportingPortDefine && chkBxReporting.Checked;
            }
            catch (Exception) { }

            // Backup Agent

            try
            {
                rdBtnMethodFull.Enabled = !_gpoSettings.IsBackupAgentBackupMethodDefine;
                rdBtnMethodDifferential.Enabled = !_gpoSettings.IsBackupAgentBackupMethodDefine;
                chkBxDontBackupOverWan.Enabled = !_gpoSettings.IsBackupAgentDontBackupThroughtWanDefine;
                txtBxAdditionalSubnets.Enabled = !_gpoSettings.IsBackupAgentAdditionalSubnetsDefine && chkBxDontBackupOverWan.Checked;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Select all lines in the CheckListBox. Selecting all Pst files
        /// </summary>
        private void SelectAllPstFiles()
        {
            for (int i = 0; i < chkLstBxPstFiles.Items.Count; i++)
            {
                chkLstBxPstFiles.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// Save settings in the UI into the registry
        /// </summary>
        private void UpdateLocalSettingsWithUI()
        {
            // Files and Folders

            try
            {
                if (!_gpoSettings.IsFilesAndFoldersCompressFilesDefine)
                {
                    _localSettings.FilesAndFoldersCompressFiles = chkBxCompressFile.Checked;
                }
                if (!_gpoSettings.IsFilesAndFoldersDestinationTypeDefine)
                {
                    _localSettings.FilesAndFoldersDestinationType = rdBtnFileSystem.Checked ? ApplicationSettings.BackupDestinationType.FileSystem : ApplicationSettings.BackupDestinationType.BackupServer;
                }
                if (!_gpoSettings.IsFilesAndFoldersDestinationPathDefine)
                {
                    _localSettings.FilesAndFoldersDestinationPath = txtBxDestination.Text;
                }
                if (!_gpoSettings.IsFilesAndFoldersBackupServerDefine)
                {
                    _localSettings.FilesAndFoldersBackupServer = txtBxBackupServerName.Text;
                }
                if (!_gpoSettings.IsFilesAndFoldersBackupPortDefine)
                {
                    _localSettings.FilesAndFoldersBackupPort = (int)nupBackupServerPort.Value;
                }
            }
            catch (Exception) { }

            // Schedule

            try
            {
                if (!_gpoSettings.IsScheduleDayOfMonthDefine)
                {
                    _localSettings.ScheduleDayOfMonth = (int)nupMonthly.Value;
                }
                if (!_gpoSettings.IsScheduleDayOfWeekDefine)
                {
                    _localSettings.ScheduleDayOfWeek = (ApplicationSettings.DayOfWeek)cmbBxWeekly.SelectedIndex;
                }
                if (!_gpoSettings.IsScheduleintervalDefine)
                {
                    _localSettings.ScheduleInterval = (int)nupEvery.Value;
                }
                if (!_gpoSettings.IsSchedulePolicyDefine)
                {
                    if (rdBtnEvery.Checked)
                    {
                        _localSettings.SchedulePolicy = ApplicationSettings.BackupPolicy.EveryX;
                    }
                    else if (rdBtnWeekly.Checked)
                    {
                        _localSettings.SchedulePolicy = ApplicationSettings.BackupPolicy.Weekly;
                    }
                    else if (rdBtnMonthly.Checked)
                    {
                        _localSettings.SchedulePolicy = ApplicationSettings.BackupPolicy.Monthly;
                    }
                }
                if (!_gpoSettings.IsScheduleUnitDefine)
                {
                    _localSettings.ScheduleUnit = (ApplicationSettings.BackupUnit)cmbBxEvery.SelectedIndex;
                }
            }
            catch (Exception) { }

            // Event Log

            try
            {
                if (!_gpoSettings.IsEventLogActivatedDefine)
                {
                    _localSettings.EventLogActivated = chkBxEventLog.Checked;
                }
                if (!_gpoSettings.IsEventLogSeverityDefine)
                {
                    _localSettings.EventLogSeverity = (PstBackupLogger.Logger.MessageSeverity)cmbBxEventLogSeverity.SelectedIndex;
                }
            }
            catch (Exception) { }

            // Reporting

            try
            {
                if (!_gpoSettings.IsReportingDefine)
                {
                    _localSettings.ReportingReportToServer = chkBxReporting.Checked;
                }
                if (!_gpoSettings.IsReportingPortDefine)
                {
                    _localSettings.ReportingPort = (int)nupReportingServerPort.Value;
                }
                if (!_gpoSettings.IsReportingServerDefine)
                {
                    _localSettings.ReportingServer = txtBxReportingServerName.Text;
                }
            }
            catch (Exception) { }

            // Backup Agent

            try
            {
                if (!_gpoSettings.IsBackupAgentAdditionalSubnetsDefine)
                {
                    _localSettings.BackupAgentAdditionalSubnets = txtBxAdditionalSubnets.Text;
                }
                if (!_gpoSettings.IsBackupAgentBackupMethodDefine)
                {
                    if (rdBtnMethodFull.Checked)
                    {
                        _localSettings.BackupAgentBackupMethod = 0;
                    }
                    else if (rdBtnMethodDifferential.Checked)
                    {
                        _localSettings.BackupAgentBackupMethod = ApplicationSettings.BackupMethod.Differential;
                    }
                }
                if (!_gpoSettings.IsBackupAgentDontBackupThroughtWanDefine)
                {
                    _localSettings.BackupAgentDontBackupThroughtWan = chkBxDontBackupOverWan.Checked;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Save informations about PST files mounted in Outlook, into registry
        /// </summary>
        private void SavePSTFilesToRegistry()
        {
            try
            {
                for (int i = 0; i < chkLstBxPstFiles.Items.Count; i++)
                {
                    PSTRegistryEntry pstFile = (PSTRegistryEntry)chkLstBxPstFiles.Items[i];
                    pstFile.ToBackup = chkLstBxPstFiles.CheckedItems.Contains(chkLstBxPstFiles.Items[i]);
                    pstFile.Save();
                }
            }
            catch (Exception ex)
            {
                PstBackupLogger.Logger.Write(30000, "Error when saving PstFiles Settings : " + ex.Message, 0, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Validate user inputs or enable the OK button accordingly
        /// </summary>
        private void ValidateData()
        {
            // Files and Folders
            bool filesAndFoldersOk = (rdBtnFileSystem.Checked && !String.IsNullOrWhiteSpace(txtBxDestination.Text)) || (rdBtnBackupServer.Checked && !String.IsNullOrWhiteSpace(txtBxBackupServerName.Text));

            // Schedule Tab
            bool scheduleOk = (rdBtnEvery.Checked && cmbBxEvery.SelectedIndex != -1) || (rdBtnWeekly.Checked && cmbBxWeekly.SelectedIndex != -1) || (rdBtnMonthly.Checked && cmbBxWeekly.SelectedIndex != -1);

            // Event Log
            bool eventLogOk = !chkBxEventLog.Checked || cmbBxEventLogSeverity.SelectedIndex != -1;

            // Report
            bool reportOk = !chkBxReporting.Checked || !String.IsNullOrWhiteSpace(txtBxReportingServerName.Text);

            btnOk.Enabled = filesAndFoldersOk && scheduleOk && eventLogOk && reportOk;
        }

        #endregion (Methods)

        #region (UI Events management)

        // OnLoad

        /// <summary>
        /// Occurs when loading the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSettings_Load(object sender, EventArgs e)
        {
            LoadPstRegistryEntries();
            _localSettings.OverrideLocalSettingsWithGPOSettings(_gpoSettings);
            _localSettings.SaveLocalSettings();

            SetupUI();
            LockUIasRequiredByGPO();
        }

        // Files and Folders Tab

        /// <summary>
        /// Occurs when the user change the destination type (File System or Backup Server)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnDestination_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnFileSystem.Checked) // Destination : File System
            {
                txtBxDestination.Enabled = !_gpoSettings.IsFilesAndFoldersDestinationPathDefine;
                txtBxBackupServerName.Enabled = false;
                nupBackupServerPort.Enabled = false;
                chkBxCompressFile.Enabled = rdBtnMethodFull.Checked;
                chkBxCompressFile.Checked = rdBtnMethodFull.Checked && _localSettings.FilesAndFoldersCompressFiles;
            }
            else                        // Destination : Backup Server
            {
                txtBxDestination.Enabled = false;
                chkBxCompressFile.Checked = false;
                chkBxCompressFile.Enabled = false;
                txtBxBackupServerName.Enabled = !_gpoSettings.IsFilesAndFoldersBackupServerDefine;
                nupBackupServerPort.Enabled = !_gpoSettings.IsFilesAndFoldersBackupPortDefine;
            }

            ValidateData();
        }

        /// <summary>
        /// Occurs when user change the destination path of the backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBxDestination_TextChanged(object sender, EventArgs e)
        {
            ValidateData();
        }

        /// <summary>
        /// Occurs when the user change the backup server name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBxBackupServerName_TextChanged(object sender, EventArgs e)
        {
            ValidateData();
        }

        // Schedule Tab

        /// <summary>
        /// Occurs when the user change the schedule type (Every x, Weekly or Monthly)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnSchedule_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnEvery.Checked)          // Backup Schedule Every x
            {
                nupEvery.Enabled = !_gpoSettings.IsScheduleintervalDefine;
                cmbBxEvery.Enabled = !_gpoSettings.IsScheduleUnitDefine;
                cmbBxWeekly.Enabled = false;
                nupMonthly.Enabled = false;
            }
            else if (rdBtnWeekly.Checked)    // Backup Schedule Weekly
            {
                nupEvery.Enabled = false;
                cmbBxEvery.Enabled = false;
                cmbBxWeekly.Enabled = !_gpoSettings.IsScheduleDayOfWeekDefine;
                nupMonthly.Enabled = false;
            }
            else if (rdBtnMonthly.Checked)   // Backup Schedule Monthly
            {
                nupEvery.Enabled = false;
                cmbBxEvery.Enabled = false;
                cmbBxWeekly.Enabled = false;
                nupMonthly.Enabled = !_gpoSettings.IsScheduleDayOfMonthDefine;
            }

            ValidateData();
        }

        /// <summary>
        /// Occurs when the user change the unit of «Every x» Backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbBxEvery_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateData();
        }

        // Event Log Tab

        /// <summary>
        /// Occurs when the user change if he wish to log events or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBxEventLog_CheckedChanged(object sender, EventArgs e)
        {
            cmbBxEventLogSeverity.Enabled = chkBxEventLog.Checked;

            ValidateData();
        }

        // Report Tab

        /// <summary>
        /// Occurs when the user change if he wish to send report to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBxReporting_CheckedChanged(object sender, EventArgs e)
        {
            txtBxReportingServerName.Enabled = chkBxReporting.Checked && !_gpoSettings.IsReportingServerDefine;
            nupReportingServerPort.Enabled = chkBxReporting.Checked && !_gpoSettings.IsReportingPortDefine;

            ValidateData();
        }

        /// <summary>
        /// Occurs when the user change the name of the reporting server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBxReportingServerName_TextChanged(object sender, EventArgs e)
        {
            ValidateData();
        }

        // Backup Agent Tab

        /// <summary>
        /// Occurs when the user switch between full and differential backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBtnMethodFull_CheckedChanged(object sender, EventArgs e)
        {
            chkBxCompressFile.Enabled = rdBtnMethodFull.Checked && rdBtnFileSystem.Checked;
            chkBxCompressFile.Checked = !chkBxCompressFile.Enabled ? false : (rdBtnFileSystem.Checked && _localSettings.FilesAndFoldersCompressFiles);
        }

        /// <summary>
        /// Occurs when the user change if he wish to backup over WAN or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBxDontBackupOverWan_CheckedChanged(object sender, EventArgs e)
        {
            txtBxAdditionalSubnets.Enabled = !_gpoSettings.IsBackupAgentAdditionalSubnetsDefine && chkBxDontBackupOverWan.Checked;
        }

        // Cancel Button

        /// <summary>
        /// Occurs when the user click on the Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        // Ok Button

        /// <summary>
        /// Occurs when the user click on the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            UpdateLocalSettingsWithUI();
            _localSettings.SaveLocalSettings();
            SavePSTFilesToRegistry();
            DialogResult = DialogResult.OK;
        }

        #endregion (UI Events management)
    }
}
