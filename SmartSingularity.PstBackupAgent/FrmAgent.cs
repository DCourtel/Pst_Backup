using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartSingularity.PstBackupSettings;
using SmartSingularity.PstBackupEngine;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupAgent
{
    public partial class FrmAgent : Form
    {
        private ApplicationSettings _localSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
        private ApplicationSettings _gpoSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.GPO);
        private List<PSTRegistryEntry> pstFilesToSave;
        private List<PSTRegistryEntry> pstFilesToNotSave;
        private System.Threading.Thread _backupThread;
        private CoreBackupEngine _bckEngine;
        private int _currentFileIndex = 0;
        private ReportService.ReportServerClient proxy;
        private System.Resources.ResourceManager _resMan = new System.Resources.ResourceManager("SmartSingularity.PstBackupAgent.Localization.Resources", typeof(FrmAgent).Assembly);

        public FrmAgent()
        {
            InitializeComponent();
            SetLogo();
            _localSettings.OverrideLocalSettingsWithGPOSettings(_gpoSettings);
            txtBxDestination.Text = _localSettings.FilesAndFoldersDestinationPath;
            try
            {
                proxy = new ReportService.ReportServerClient("BasicHttpBinding_IReportServer");
            }
            catch (Exception ex)
            {
                Logger.Write(20027, "An error occurs while instanciating the client proxy. Report server will be unreachable\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
            RegisterClient();
        }

        #region (Methods)

        private void RegisterClient()
        {
            try
            {
                if (_localSettings.ReportingReportToServer)
                {
                    ReportService.Client client = new ReportService.Client()
                    {
                        Id = _localSettings.ClientId,
                        ComputerName = System.Net.Dns.GetHostEntry("").HostName,
                        Username = $"{Environment.UserName}.{Environment.UserDomainName}",
                        Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
                    };
                    proxy.RegisterClient(client);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(20028, "An error occurs while trying to register/update client on report server\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void RegisterPstFiles(List<PSTRegistryEntry> pstFilesToRegister, string clientId)
        {
            if (_localSettings.ReportingReportToServer)
            {
                foreach (PSTRegistryEntry regEntry in pstFilesToRegister)
                {
                    try
                    {
                        ReportService.PstFile pstFile = GetPstFile(regEntry);
                        proxy.RegisterPstFile(clientId, pstFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(20029, "An error occurs while trying to register a PstFile\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
                    }
                }
            }
        }

        private void ReportBackupSessionResult(BackupResultInfo bckResult, bool isSchedule)
        {
            if (_localSettings.ReportingReportToServer)
            {
                ReportService.BackupSession bckSession = new ReportService.BackupSession()
                {
                    LocalPath = bckResult.LocalPath,
                    RemotePath = bckResult.RemotePath,
                    BackupMethod = _localSettings.BackupAgentBackupMethod,
                    ChunkCount = bckResult.ChunkCount,
                    CompressedSize = bckResult.CompressedSize,
                    DestinationType = _localSettings.FilesAndFoldersDestinationType,
                    EndTime = bckResult.EndTime,
                    ErrorCode = bckResult.ErrorCode,
                    ErrorMessage = bckResult.ErrorMessage,
                    IsCompressed = bckResult.IsCompressed,
                    IsSchedule = isSchedule,
                    StartTime = bckResult.StartTime
                };
                proxy.RegisterBackupResult(_localSettings.ClientId, bckSession); 
            }
        }

        private ReportService.PstFile GetPstFile(PSTRegistryEntry regEntry)
        {
            ReportService.PstFile pstFile = new ReportService.PstFile()
            {
                LocalPath = regEntry.SourcePath,
                IsSetToBackup = regEntry.ToBackup,
                Size = new System.IO.FileInfo(regEntry.SourcePath).Length
            };
            return pstFile;
        }

        public void Backup()
        {
            try
            {
                Logger.Write(16, "Starting backup session", Logger.MessageSeverity.Information);

                if (_localSettings.IsDestinationProperlyDefine())
                {
                    _bckEngine = CoreBackupEngine.GetBackupEngine(_localSettings);
                    _bckEngine.OnBackupFinished += BckEngine_OnBackupFinished;
                    _bckEngine.OnBackupProgress += BckEngine_OnBackupProgress;

                    List<PSTRegistryEntry> allPstFiles = ApplicationSettings.GetPstRegistryEntries();
                    Logger.Write(17, "Found " + allPstFiles.Count + " Pst file(s) registered in Outlook.", Logger.MessageSeverity.Information);
#if (DEBUG)
                    System.Random rnd = new Random(DateTime.Now.Millisecond);
                    for (int i = 0; i < allPstFiles.Count; i++)
                    {
                        allPstFiles[i].LastSuccessfulBackup = DateTime.Now.Subtract(new TimeSpan(rnd.Next(72, 300), 0, 0, 0));
                    }
#endif
                    (_bckEngine as CoreBackupEngine).SelectPstFilesToSave(allPstFiles, out pstFilesToSave, out pstFilesToNotSave);

                    RegisterPstFiles(pstFilesToSave, _localSettings.ClientId);
                    RegisterPstFiles(pstFilesToNotSave, _localSettings.ClientId);
                    pstFilesToSave.Sort();
                    DisplayFileList(pstFilesToSave);
                    if (pstFilesToSave.Count > 0)
                    {
                        SetMaximumOverAllProgressBar(pstFilesToSave.Count);
                        LaunchBackup(pstFilesToSave[0]);
                        this.ShowDialog();
                    }
                }
                else
                {
                    Logger.Write(10007, "Destination not correctly define. Review application's settings", Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                    MessageBox.Show(_resMan.GetString("DestinationNotCorrectlyDefine"));
                }
            }
            catch (Exception ex)
            {
                Logger.Write(20020, "An error occurs while launching backup.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
                MessageBox.Show(_resMan.GetString("ErrorWhileTryingToBackupFiles"));
            }
        }

        private void LaunchBackup(PSTRegistryEntry pstFileToSave)
        {
            try
            {
                Logger.Write(23, "Starting to save " + pstFileToSave.SourcePath, Logger.MessageSeverity.Information);
                Action SetCurrentFileFinished = () =>
                   { chkLstBxPstFiles.SetItemCheckState(_currentFileIndex, CheckState.Indeterminate); };
                if (this.InvokeRequired)
                    this.Invoke(SetCurrentFileFinished);
                else
                    SetCurrentFileFinished();

                _backupThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(_bckEngine.Backup));
                _backupThread.IsBackground = true;
                _backupThread.Start(pstFilesToSave[0]);
            }
            catch (Exception ex)
            {
                Logger.Write(20021, "An error occurs while starting to backup a PST file.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void SetLogo()
        {
            try
            {
                System.IO.FileInfo appPath = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string logoPath = System.IO.Path.Combine(appPath.DirectoryName, "logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    pctBxLogo.Image = new Bitmap(logoPath);
                    pctBxLogo.BorderStyle = BorderStyle.FixedSingle;
                }
            }
            catch (Exception ex) { Logger.Write(20022, "Error while setting Logo.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
        }

        private void DisplayFileList(List<PSTRegistryEntry> fileList)
        {
            DateTime neverBackup = new DateTime();
            string pstList = String.Empty;

            try
            {
                foreach (PSTRegistryEntry pstFile in fileList)
                {
                    DateTime lastBackup = pstFile.LastSuccessfulBackup;
                    chkLstBxPstFiles.Items.Add(pstFile.SourcePath + " : " + (lastBackup == neverBackup ? _resMan.GetString("NeverSaved") : lastBackup.ToShortDateString()));
                    pstList += pstFile.SourcePath + "\r\n";
                }
                Logger.Write(18, "List of PST files to backup : " + pstList, Logger.MessageSeverity.Information);
            }
            catch (Exception ex)
            {
                Logger.Write(20023, "Error while displaying PST files list.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void SetMaximumOverAllProgressBar(int fileCount)
        {
            prgBrOverAll.Maximum = fileCount * 100;
        }

        private void UpdateUI(int percent)
        {
            try
            {
                Action updateProgressBar = () =>
                    {
                        prgBrOverAll.Value = _currentFileIndex * 100 + percent;
                        prgBrCurrentFile.Value = percent;
                        if (percent == 100)
                        {
                            chkLstBxPstFiles.SetItemChecked(_currentFileIndex, true);
                            chkLstBxPstFiles.Items[_currentFileIndex] = pstFilesToSave[0].SourcePath + " : " + DateTime.Today.ToShortDateString();
                        }
                    };
                if (!this.Disposing || !_bckEngine.IsCancelRequired)
                    this.Invoke(updateProgressBar);
            }
            catch (Exception) { }
        }

        #endregion (Methods)

        #region (Events)

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Logger.Write(11, "User have pressed Cancel button", Logger.MessageSeverity.Information);
            try
            {
                _bckEngine.IsCancelRequired = true;
                btnCancel.Enabled = false;
                btnCancel.Refresh();
                _backupThread.Join(3000);
                proxy.Close();
            }
            catch (Exception) { }
            Close();
        }

        private void BckEngine_OnBackupProgress(object sender, BackupProgressEventArgs e)
        {
            UpdateUI(e.Percent);
        }

        private void BckEngine_OnBackupFinished(object sender, BackupFinishedEventArgs e)
        {
            ReportBackupSessionResult(e.Result, true);
            UpdateUI(100);
            pstFilesToSave.RemoveAt(0);
            _currentFileIndex++;
            if (pstFilesToSave.Count > 0)
            {
                LaunchBackup(pstFilesToSave[0]);
            }
            else
            {
                Logger.Write(19, "All PST Files have been processed", Logger.MessageSeverity.Information);
                if (chkBxShutdownComputer.Checked)
                {
                    try
                    {
                        System.Diagnostics.Process.Start("SmartSingularity.PstBackupShutdownComputer.exe");
                    }
                    catch (Exception) { }
                }
                Action closeApp = () =>
                {
                    try
                    {
                        proxy.Close();
                    }
                    catch (Exception) { }
                    Close();
                };
                if (this.InvokeRequired)
                    this.Invoke(closeApp);
                else
                    closeApp();
            }
        }
    }

    #endregion (Events)
}