using System;
using System.Collections.Generic;
using Microsoft.Win32;
using SmartSingularity.PstBackupLogger;

namespace SmartSingularity.PstBackupSettings
{
    public class ApplicationSettings
    {
        public enum SourceSettings
        {
            GPO,
            Local
        }

        public enum BackupMethod
        {
            Full,
            Differential
        }

        public enum BackupDestinationType
        {
            FileSystem,
            BackupServer
        }

        public enum BackupPolicy
        {
            EveryX,
            Weekly,
            Monthly
        }

        public enum BackupUnit
        {
            Days,
            Weeks,
            Months
        }

        public enum DayOfWeek
        {
            Sunday,
            Monday,
            Thuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }

        #region private fields

        private SourceSettings _readSettingsFrom = SourceSettings.Local;
        private string _baseKey = String.Empty;

        private string _clientID = String.Empty;

        private bool _bckAgtDontBackupThroughtWan = true;
        private string _bckAgtAdditionalSubnets = String.Empty;
        private BackupMethod _bckAgtBackupMethod = BackupMethod.Full;
        private bool _bckAgtSetExclusiveNTFSPermissions = false;
        private string _bckAgtAdditionalNTFSFullcontrol = String.Empty;
        private string _bckAgtAdditionalNTFSReadWrite = String.Empty;

        private bool _eventLogActivated = true;
        private Logger.MessageSeverity _eventLogseverity = Logger.MessageSeverity.Information;

        private bool _filesAndFoldersCompressFiles = false;
        private BackupDestinationType _filesAndFoldersDestinationType = BackupDestinationType.FileSystem;
        private string _filesAndFoldersDestinationPath = String.Empty;
        private string _filesAndFoldersBackupServer = String.Empty;
        private int _filesAndFoldersBackupPort = 443;

        private bool _reportToServer = false;
        private string _reportingServer = String.Empty;
        private int _reportingPort = 443;

        private BackupPolicy _schedulePolicy = BackupPolicy.EveryX;
        private int _scheduleInterval = 7;
        private BackupUnit _scheduleUnit = BackupUnit.Days;
        private DayOfWeek _scheduleyDayOfWeek = DayOfWeek.Monday;
        private int _scheduleyDayOfMonth = 1;

        #endregion private fields

        private ApplicationSettings() { }

        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="readSettingsFrom">Where to read registry values. Can be either 'GPO', so values will be read from "Software\Policies\PST Backup 2013", or 'Local', so values will read from "Software\PST Backup 2013".</param>        
        public ApplicationSettings(SourceSettings readSettingsFrom)
        {
            _readSettingsFrom = readSettingsFrom;
            Logger.IsLogActivated = true;
            Logger.MinimalSeverity = Logger.MessageSeverity.Debug;
            IsBackupAgentDontBackupThroughtWanDefine = false;

            IsEventLogSeverityDefine = false;

            IsFilesAndFoldersCompressFilesDefine = false;

            IsSchedulePolicyDefine = false;
            IsScheduleintervalDefine = false;
            IsScheduleUnitDefine = false;
            IsScheduleDayOfWeekDefine = false;
            IsScheduleDayOfMonthDefine = false;

            switch (_readSettingsFrom)
            {
                case SourceSettings.GPO:
                    _baseKey = @"Software\Policies\PST Backup";
                    break;
                case SourceSettings.Local:
                    _baseKey = @"Software\PST Backup";
                    break;
            }
            LoadSettingsFromRegistry();
        }

        #region intenal properties

        public string ClientID { get { return _clientID; } set { _clientID = value; } }

        public bool BackupAgentDontBackupThroughtWan { get { return _bckAgtDontBackupThroughtWan; } set { _bckAgtDontBackupThroughtWan = value; } }
        public string BackupAgentAdditionalSubnets { get { return _bckAgtAdditionalSubnets; } set { _bckAgtAdditionalSubnets = value; } }
        public BackupMethod BackupAgentBackupMethod { get { return _bckAgtBackupMethod; } set { _bckAgtBackupMethod = value; } }
        public bool BackupAgentSetExclusiveNTFSPermissions { get { return _bckAgtSetExclusiveNTFSPermissions; } set { _bckAgtSetExclusiveNTFSPermissions = value; } }
        public string BackupAgentAdditionalNTFSFullcontrol { get { return _bckAgtAdditionalNTFSFullcontrol; } set { _bckAgtAdditionalNTFSFullcontrol = value; } }
        public string BackupAgentAdditionalNTFSReadWrite { get { return _bckAgtAdditionalNTFSReadWrite; } set { _bckAgtAdditionalNTFSReadWrite = value; } }

        public bool EventLogActivated { get { return _eventLogActivated; } set { _eventLogActivated = value; } }
        public Logger.MessageSeverity EventLogSeverity { get { return _eventLogseverity; } set { _eventLogseverity = value; } }

        public bool FilesAndFoldersCompressFiles { get { return _filesAndFoldersCompressFiles; } set { _filesAndFoldersCompressFiles = value; } }
        public BackupDestinationType FilesAndFoldersDestinationType { get { return _filesAndFoldersDestinationType; } set { _filesAndFoldersDestinationType = value; } }
        public string FilesAndFoldersDestinationPath { get { return _filesAndFoldersDestinationPath; } set { _filesAndFoldersDestinationPath = value; } }
        public string FilesAndFoldersBackupServer { get { return _filesAndFoldersBackupServer; } set { _filesAndFoldersBackupServer = value; } }
        public int FilesAndFoldersBackupPort { get { return _filesAndFoldersBackupPort; } set { _filesAndFoldersBackupPort = value; } }

        public bool ReportingReportToServer { get { return _reportToServer; } set { _reportToServer = value; } }
        public string ReportingServer { get { return _reportingServer; } set { _reportingServer = value; } }
        public int ReportingPort { get { return _reportingPort; } set { _reportingPort = value; } }

        public BackupPolicy SchedulePolicy { get { return _schedulePolicy; } set { _schedulePolicy = value; } }
        public int ScheduleInterval { get { return _scheduleInterval; } set { _scheduleInterval = value; } }
        public BackupUnit ScheduleUnit { get { return _scheduleUnit; } set { _scheduleUnit = value; } }
        public DayOfWeek ScheduleDayOfWeek { get { return _scheduleyDayOfWeek; } set { _scheduleyDayOfWeek = value; } }
        public int ScheduleDayOfMonth { get { return _scheduleyDayOfMonth; } set { _scheduleyDayOfMonth = value; } }

        public bool IsBackupAgentDontBackupThroughtWanDefine { get; set; }
        public bool IsBackupAgentBackupMethodDefine { get; set; }
        public bool IsBackupAgentAdditionalSubnetsDefine { get; set; }

        public bool IsEventLogActivatedDefine { get; set; }
        public bool IsEventLogSeverityDefine { get; set; }

        public bool IsFilesAndFoldersCompressFilesDefine { get; set; }
        public bool IsFilesAndFoldersDestinationTypeDefine { get; set; }
        public bool IsFilesAndFoldersDestinationPathDefine { get; set; }
        public bool IsFilesAndFoldersBackupServerDefine { get; set; }
        public bool IsFilesAndFoldersBackupPortDefine { get; set; }

        public bool IsReportingDefine { get; set; }
        public bool IsReportingServerDefine { get; set; }
        public bool IsReportingPortDefine { get; set; }

        public bool IsSchedulePolicyDefine { get; set; }
        public bool IsScheduleintervalDefine { get; set; }
        public bool IsScheduleUnitDefine { get; set; }
        public bool IsScheduleDayOfWeekDefine { get; set; }
        public bool IsScheduleDayOfMonthDefine { get; set; }

        #endregion intenal properties

        #region public Methods

        /// <summary>
        /// Gets all PST files that have been previously recorded in the registry for being processed by PST Backup.
        /// </summary>
        /// <returns>A List of <RegistryEntry>.</returns>
        public static List<PSTRegistryEntry> GetPstRegistryEntries()
        {
            List<PSTRegistryEntry> registryEntries = new List<PSTRegistryEntry>();

            try
            {
                RegistryKey baseKey = Registry.CurrentUser.OpenSubKey(@"Software\PST Backup\PST Files");

                string[] subKeyNames = baseKey.GetSubKeyNames();

                foreach (string subKeyName in subKeyNames)
                {
                    try
                    {
                        RegistryKey subKey = baseKey.OpenSubKey(subKeyName);
                        registryEntries.Add(GetPstRegistryEntry(subKey));
                        subKey.Close();
                    }
                    catch (Exception) { }
                }
                baseKey.Close();
            }
            catch (Exception) { }

            return registryEntries;
        }

        public void OverrideLocalSettingsWithGPOSettings(ApplicationSettings gpoSettings)
        {

            if (gpoSettings.IsBackupAgentDontBackupThroughtWanDefine)
                BackupAgentDontBackupThroughtWan = gpoSettings.BackupAgentDontBackupThroughtWan;
            if (gpoSettings.IsBackupAgentAdditionalSubnetsDefine)
                BackupAgentAdditionalSubnets = gpoSettings.BackupAgentAdditionalSubnets;
            if (gpoSettings.IsBackupAgentBackupMethodDefine)
                BackupAgentBackupMethod = gpoSettings.BackupAgentBackupMethod;

            if (gpoSettings.IsEventLogActivatedDefine)
                EventLogActivated = gpoSettings.EventLogActivated;
            if (gpoSettings.IsEventLogSeverityDefine)
                EventLogSeverity = gpoSettings.EventLogSeverity;

            if (gpoSettings.IsFilesAndFoldersCompressFilesDefine)
                FilesAndFoldersCompressFiles = gpoSettings.FilesAndFoldersCompressFiles;
            if (gpoSettings.IsFilesAndFoldersDestinationTypeDefine)
                FilesAndFoldersDestinationPath = gpoSettings.FilesAndFoldersDestinationPath;
            if (!String.IsNullOrWhiteSpace(gpoSettings.FilesAndFoldersDestinationPath))
                FilesAndFoldersDestinationPath = gpoSettings.FilesAndFoldersDestinationPath;
            if (!String.IsNullOrWhiteSpace(gpoSettings.FilesAndFoldersBackupServer))
                FilesAndFoldersBackupServer = gpoSettings.FilesAndFoldersBackupServer;
            if (gpoSettings.IsFilesAndFoldersBackupPortDefine)
                FilesAndFoldersBackupPort = gpoSettings.FilesAndFoldersBackupPort;

            if (gpoSettings.IsReportingDefine)
                ReportingReportToServer = gpoSettings.ReportingReportToServer;
            if (gpoSettings.IsReportingServerDefine)
                ReportingServer = gpoSettings.ReportingServer;
            if (gpoSettings.IsReportingPortDefine)
                ReportingPort = gpoSettings.ReportingPort;

            if (gpoSettings.IsSchedulePolicyDefine)
                SchedulePolicy = gpoSettings.SchedulePolicy;
            if (gpoSettings.IsScheduleintervalDefine)
                ScheduleInterval = gpoSettings.ScheduleInterval;
            if (gpoSettings.IsScheduleUnitDefine)
                ScheduleUnit = gpoSettings.ScheduleUnit;
            if (gpoSettings.IsScheduleDayOfWeekDefine)
                ScheduleDayOfWeek = gpoSettings.ScheduleDayOfWeek;
            if (gpoSettings.IsScheduleDayOfMonthDefine)
                ScheduleDayOfMonth = gpoSettings.ScheduleDayOfMonth;
        }

        public void SaveLocalSettings()
        {
            try
            {
                RegistryKey baseKey = Registry.CurrentUser.OpenSubKey(_baseKey, true);

                if (baseKey == null)
                    baseKey = CreateBaseKey();
                RegistryKey settingsKey = baseKey.OpenSubKey("Settings", true);
                if (settingsKey == null)
                    settingsKey = CreateLocalSettingsKeys(baseKey);

                SaveIntValue(settingsKey, "Backup Agent", "DontBackupThroughWan", BackupAgentDontBackupThroughtWan ? 1 : 0);
                SaveStringValue(settingsKey, "Backup Agent", "AdditionalSubnets", BackupAgentAdditionalSubnets.ToString());
                SaveIntValue(settingsKey, "Backup Agent", "BackupMethod", (int)BackupAgentBackupMethod);

                SaveIntValue(settingsKey, "Event Log", "Severity", (int)EventLogSeverity);
                SaveIntValue(settingsKey, "Event Log", "LogEvent", EventLogActivated ? 1 : 0);

                SaveIntValue(settingsKey, "Files And Folders", "CompressFiles", FilesAndFoldersCompressFiles ? 1 : 0);
                SaveIntValue(settingsKey, "Files And Folders", "DestinationType", (int)FilesAndFoldersDestinationType);
                SaveStringValue(settingsKey, "Files And Folders", "DestinationPath", FilesAndFoldersDestinationPath.ToString());
                SaveStringValue(settingsKey, "Files And Folders", "BackupServer", FilesAndFoldersBackupServer.ToString());
                SaveIntValue(settingsKey, "Files And Folders", "BackupPort", FilesAndFoldersBackupPort);

                SaveIntValue(settingsKey, "Reporting", "Report", ReportingReportToServer ? 1 : 0);
                SaveStringValue(settingsKey, "Reporting", "Server", ReportingServer.ToString());
                SaveIntValue(settingsKey, "Reporting", "Port", ReportingPort);

                SaveIntValue(settingsKey, "Schedule", "Policy", (int)SchedulePolicy);
                SaveIntValue(settingsKey, "Schedule", "Interval", ScheduleInterval);
                SaveIntValue(settingsKey, "Schedule", "Unit", (int)ScheduleUnit);
                SaveIntValue(settingsKey, "Schedule", "DayOfWeek", (int)ScheduleDayOfWeek);
                SaveIntValue(settingsKey, "Schedule", "DayOfMonth", ScheduleDayOfMonth);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Inspect the candidateValue, if this one is between minValue and maxValue, then return candidateValue else return defaultValue.
        /// </summary>
        /// <param name="candidateValue">The candidate value to inspect.</param>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="defaultValue">Default value to return if candidateValue is NOT between minValue and maxValue.</param>
        /// <returns>A value that is between minValue and maxValue.</returns>
        public static int GetCorrectIntValue(int candidateValue, int minValue, int maxValue, int defaultValue)
        {
            if (candidateValue < minValue || candidateValue > maxValue)
                return defaultValue;
            else
                return candidateValue;
        }

        #endregion public Methods 

        #region private methods

        /// <summary>
        /// Build a new PSTRegistryEntry with values under the provided subKey.
        /// </summary>
        /// <param name="subKey">SubKey that contains registry values to build the new PSTRegistryEntry.</param>
        /// <returns>Return a new PSTRegistryEntry initialized with values contains under the subKey.</returns>
        private static PSTRegistryEntry GetPstRegistryEntry(RegistryKey subKey)
        {
            PSTRegistryEntry regEntry = new PSTRegistryEntry("");
            try
            {
                regEntry.RegistryPath = subKey.Name;
                regEntry.SourcePath = subKey.GetValue("SourcePath").ToString();
                regEntry.ToBackup = ((int)subKey.GetValue("Backup") == 1) ? true : false;
                regEntry.LastSuccessfulBackup = Convert.ToDateTime(subKey.GetValue("LastSuccessfulBackup").ToString());
            }
            catch (Exception) { }

            return regEntry;
        }

        private void LoadSettingsFromRegistry()
        {
            try
            {
                RegistryKey baseKey;
                baseKey = Registry.CurrentUser.OpenSubKey(_baseKey, _readSettingsFrom == SourceSettings.Local);

                if (baseKey == null)
                    baseKey = CreateBaseKey();
                if (baseKey != null)
                {
                    ClientID = (_readSettingsFrom == SourceSettings.Local) ? GetClientID(baseKey) : String.Empty;

                    RegistryKey settingsKey;
                    settingsKey = baseKey.OpenSubKey("Settings", _readSettingsFrom == SourceSettings.Local);

                    if (settingsKey == null)
                        settingsKey = CreateLocalSettingsKeys(baseKey);

                    if (settingsKey != null)
                    {
                        LoadBackupAgentSettings(settingsKey);
                        LoadEventLogSettings(settingsKey);
                        LoadFilesAndFoldersSettings(settingsKey);
                        LoadReportingSettings(settingsKey);
                        LoadScheduleSettings(settingsKey);

                        settingsKey.Close();
                    }
                    baseKey.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(20002, "An error occure while reading " + (_readSettingsFrom == SourceSettings.Local ? "Local" : "GPO") + @" Settings from HKCU\" + _baseKey + ".\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private RegistryKey CreateBaseKey()
        {
            RegistryKey baseKey = null;
            if (_readSettingsFrom == SourceSettings.Local)
            {
                try
                {
                    baseKey = Registry.CurrentUser.CreateSubKey(_baseKey);
                    CreateLocalSettingsKeys(baseKey);
                }
                catch (Exception ex) { Logger.Write(20003, @"An error occurs while creating base Keys in HKCU\" + _baseKey + ".\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
            }
            return baseKey;
        }

        private string GetClientID(RegistryKey baseKey)
        {
            string clientID = baseKey.GetValue("ClientID", String.Empty).ToString();
            if (String.IsNullOrEmpty(clientID))
            {
                CreateClientID(baseKey);
            }

            return clientID;
        }

        private void CreateClientID(RegistryKey baseKey)
        {
            string clientID = Guid.NewGuid().ToString();
            CreateMissingStringValue(baseKey, "ClientID", clientID);
        }

        private RegistryKey CreateLocalSettingsKeys(RegistryKey baseKey)
        {
            RegistryKey settingsKey = null;
            if (_readSettingsFrom == SourceSettings.Local)
            {
                try
                {
                    baseKey.CreateSubKey(@"Settings\Files And Folders");
                    baseKey.CreateSubKey(@"Settings\Schedule");
                    baseKey.CreateSubKey(@"Settings\Event Log");
                    baseKey.CreateSubKey(@"Settings\Reporting");
                    baseKey.CreateSubKey(@"Settings\Backup Agent");

                    settingsKey = baseKey.OpenSubKey("Settings");
                }
                catch (Exception ex) { Logger.Write(20004, @"An error occurs while creating Local Settings Keys in HKCU\" + _baseKey + ".\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
            }
            return settingsKey;
        }

        private void LoadBackupAgentSettings(RegistryKey settingsKey)
        {
            try
            {
                RegistryKey bckAgtKey;

                bckAgtKey = settingsKey.OpenSubKey("Backup Agent", _readSettingsFrom == SourceSettings.Local);
                if (bckAgtKey == null)
                { bckAgtKey = CreateMissingKey(settingsKey, "Backup Agent"); }

                if (bckAgtKey != null)
                {
                    object data = null;
                    data = bckAgtKey.GetValue("DontBackupThroughWan", null);
                    if (data == null)
                        CreateMissingIntValue(bckAgtKey, "DontBackupThroughWan", BackupAgentDontBackupThroughtWan ? 1 : 0);
                    else
                    {
                        BackupAgentDontBackupThroughtWan = ((int)data) == 1;
                        IsBackupAgentDontBackupThroughtWanDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = bckAgtKey.GetValue("AdditionalSubnets", null);
                    if (data == null)
                        CreateMissingStringValue(bckAgtKey, "AdditionalSubnets", BackupAgentAdditionalSubnets);
                    else
                    {
                        BackupAgentAdditionalSubnets = data.ToString();
                        IsBackupAgentAdditionalSubnetsDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = bckAgtKey.GetValue("BackupMethod", null);
                    if (data == null)
                        CreateMissingIntValue(bckAgtKey, "BackupMethod", (int)BackupAgentBackupMethod);
                    else
                    {
                        BackupAgentBackupMethod = (BackupMethod)((int)data);
                        IsBackupAgentBackupMethodDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = bckAgtKey.GetValue("SetExclusiveNTFSPermissions", null);
                    if (data == null)
                        BackupAgentSetExclusiveNTFSPermissions = false;
                    else
                    {
                        BackupAgentSetExclusiveNTFSPermissions = ((int)data) == 1;
                    }

                    data = bckAgtKey.GetValue("AdditionalNTFSFullcontrol", null);
                    if (data == null)
                        BackupAgentAdditionalNTFSFullcontrol = String.Empty;
                    else
                    {
                        BackupAgentAdditionalNTFSFullcontrol = data.ToString();
                    }

                    data = bckAgtKey.GetValue("AdditionalNTFSReadWrite", null);
                    if (data == null)
                        BackupAgentAdditionalNTFSReadWrite = String.Empty;
                    else
                    {
                        BackupAgentAdditionalNTFSReadWrite = data.ToString();
                    }

                    bckAgtKey.Close();
                }
            }
            catch (Exception) { }
        }

        private void LoadEventLogSettings(RegistryKey settingsKey)
        {
            try
            {
                RegistryKey eventLogKey;
                eventLogKey = settingsKey.OpenSubKey("Event Log", _readSettingsFrom == SourceSettings.Local);

                if (eventLogKey == null)
                    eventLogKey = CreateMissingKey(settingsKey, "Event Log");

                if (eventLogKey != null)
                {
                    object data = null;

                    data = eventLogKey.GetValue("LogEvent", null);
                    if (data == null)
                    {
                        CreateMissingIntValue(eventLogKey, "LogEvent", EventLogActivated ? 1 : 0);
                    }
                    else
                    {
                        EventLogActivated = ((int)data) == 1;
                        IsEventLogActivatedDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = eventLogKey.GetValue("Severity", null);
                    if (data == null)
                        CreateMissingIntValue(eventLogKey, "Severity", (int)EventLogSeverity);
                    else
                    {
                        EventLogSeverity = (Logger.MessageSeverity)int.Parse(data.ToString());
                        IsEventLogSeverityDefine = _readSettingsFrom == SourceSettings.GPO;
                    }
                    eventLogKey.Close();
                }
            }
            catch (Exception) { }
        }

        private void LoadFilesAndFoldersSettings(RegistryKey settingsKey)
        {
            try
            {
                RegistryKey filesAndFoldersKey;
                filesAndFoldersKey = settingsKey.OpenSubKey("Files and Folders", _readSettingsFrom == SourceSettings.Local);

                if (filesAndFoldersKey == null)
                    filesAndFoldersKey = CreateMissingKey(settingsKey, "Files and Folders");

                if (filesAndFoldersKey != null)
                {
                    object data = null;

                    data = filesAndFoldersKey.GetValue("CompressFiles", null);
                    if (data == null)
                        CreateMissingIntValue(filesAndFoldersKey, "CompressFiles", FilesAndFoldersCompressFiles ? 1 : 0);
                    else
                    {
                        FilesAndFoldersCompressFiles = ((int)data) == 1;
                        IsFilesAndFoldersCompressFilesDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = filesAndFoldersKey.GetValue("DestinationType", null);
                    if (data == null)
                        CreateMissingIntValue(filesAndFoldersKey, "DestinationType", FilesAndFoldersCompressFiles ? 1 : 0);
                    else
                    {
                        FilesAndFoldersDestinationType = (BackupDestinationType)((int)data);
                        IsFilesAndFoldersDestinationTypeDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = filesAndFoldersKey.GetValue("DestinationPath", null);
                    if (data == null)
                        CreateMissingStringValue(filesAndFoldersKey, "DestinationPath", FilesAndFoldersDestinationPath);
                    else
                    {
                        FilesAndFoldersDestinationPath = data.ToString();
                        IsFilesAndFoldersDestinationPathDefine = true;
                    }

                    data = filesAndFoldersKey.GetValue("BackupServer", null);
                    if (data == null)
                        CreateMissingStringValue(filesAndFoldersKey, "BackupServer", FilesAndFoldersBackupServer);
                    else
                    {
                        FilesAndFoldersBackupServer = data.ToString();
                        IsFilesAndFoldersBackupServerDefine = true;
                    }

                    data = filesAndFoldersKey.GetValue("BackupPort", null);
                    if (data == null)
                        CreateMissingIntValue(filesAndFoldersKey, "BackupPort", FilesAndFoldersBackupPort);
                    else
                    {
                        FilesAndFoldersBackupPort = (int)data;
                        IsFilesAndFoldersBackupPortDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    filesAndFoldersKey.Close();
                }
            }
            catch (Exception) { }
        }

        private void LoadReportingSettings(RegistryKey settingsKey)
        {
            try
            {
                RegistryKey reportingKey;
                reportingKey = settingsKey.OpenSubKey("Reporting", _readSettingsFrom == SourceSettings.Local);

                if (reportingKey == null)
                    reportingKey = CreateMissingKey(settingsKey, "Reporting");

                if (reportingKey != null)
                {
                    object data = null;

                    data = reportingKey.GetValue("Report", null);
                    if (data == null)
                        CreateMissingIntValue(reportingKey, "Report", ReportingReportToServer ? 1 : 0);
                    else
                    {
                        ReportingReportToServer = ((int)data) == 1;
                        IsReportingDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = reportingKey.GetValue("Server", null);
                    if (data == null)
                        CreateMissingStringValue(reportingKey, "Server", ReportingServer);
                    else
                    {
                        ReportingServer = data.ToString();
                        IsReportingServerDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = reportingKey.GetValue("Port", null);
                    if (data == null)
                        CreateMissingIntValue(reportingKey, "Port", ReportingPort);
                    else
                    {
                        ReportingPort = (int)data;
                        IsReportingPortDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    reportingKey.Close();
                }
            }
            catch (Exception) { }
        }

        private void LoadScheduleSettings(RegistryKey settingsKey)
        {
            try
            {
                RegistryKey scheduleKey;
                scheduleKey = settingsKey.OpenSubKey("Schedule", _readSettingsFrom == SourceSettings.Local);

                if (scheduleKey == null)
                    scheduleKey = CreateMissingKey(settingsKey, "Schedule");

                if (scheduleKey != null)
                {
                    object data = null;
                    data = scheduleKey.GetValue("Policy", null);
                    if (data == null)
                        CreateMissingIntValue(scheduleKey, "Policy", (int)SchedulePolicy);
                    else
                    {
                        SchedulePolicy = (BackupPolicy)((int)data);
                        IsSchedulePolicyDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = scheduleKey.GetValue("Interval", null);
                    if (data == null)
                        CreateMissingIntValue(scheduleKey, "Interval", ScheduleInterval);
                    else
                    {
                        ScheduleInterval = GetCorrectIntValue((int)data, 1, 365, 7);
                        IsScheduleintervalDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = scheduleKey.GetValue("Unit", null);
                    if (data == null)
                        CreateMissingIntValue(scheduleKey, "Unit", (int)ScheduleUnit);
                    else
                    {
                        ScheduleUnit = (BackupUnit)GetCorrectIntValue((int)data, 0, 2, 0);
                        IsScheduleUnitDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = scheduleKey.GetValue("DayOfWeek", null);
                    if (data == null)
                        CreateMissingIntValue(scheduleKey, "DayOfWeek", (int)ScheduleDayOfWeek);
                    else
                    {
                        ScheduleDayOfWeek = (DayOfWeek)GetCorrectIntValue((int)data, 0, 6, 5);
                        IsScheduleDayOfWeekDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    data = scheduleKey.GetValue("DayOfMonth", null);
                    if (data == null)
                        CreateMissingIntValue(scheduleKey, "DayOfMonth", ScheduleDayOfMonth);
                    else
                    {
                        ScheduleDayOfMonth = GetCorrectIntValue((int)data, 1, 31, 1);
                        IsScheduleDayOfMonthDefine = _readSettingsFrom == SourceSettings.GPO;
                    }

                    scheduleKey.Close();
                }
            }
            catch (Exception) { }
        }

        private RegistryKey CreateMissingKey(RegistryKey settingsKey, string keyName)
        {
            if (_readSettingsFrom == SourceSettings.Local)
            {
                return settingsKey.CreateSubKey(keyName);
            }
            return null;
        }

        private void CreateMissingStringValue(RegistryKey keyName, string valueName, string data)
        {
            if (_readSettingsFrom == SourceSettings.Local)
            {
                keyName.SetValue(valueName, data, RegistryValueKind.String);
            }
        }

        private void CreateMissingIntValue(RegistryKey keyName, string valueName, int data)
        {
            if (_readSettingsFrom == SourceSettings.Local)
            {
                keyName.SetValue(valueName, data, RegistryValueKind.DWord);
            }
        }

        private void SaveStringValue(RegistryKey settingsKey, string keyName, string valueName, string data)
        {
            try
            {
                RegistryKey key = settingsKey.OpenSubKey(keyName, true);
                if (key == null)
                    key = CreateMissingKey(settingsKey, keyName);
                key.SetValue(valueName, data, RegistryValueKind.String);
            }
            catch (Exception) { }
        }

        private void SaveIntValue(RegistryKey settingsKey, string keyName, string valueName, int data)
        {
            try
            {
                RegistryKey key = settingsKey.OpenSubKey(keyName, true);
                if (key == null)
                    key = CreateMissingKey(settingsKey, keyName);
                key.SetValue(valueName, data, RegistryValueKind.DWord);
            }
            catch (Exception) { }
        }

        #endregion private methods
    }
}
