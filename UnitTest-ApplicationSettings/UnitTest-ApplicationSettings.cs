using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SUT = SmartSingularity.PstBackupSettings.ApplicationSettings;
namespace UnitTest_PstBackupSettings
{
    public class ApplicationSettings
    {
        internal static void DeleteLocalSettings()
        {
            if (IsRegistryKeyExists(RegistryHive.CurrentUser, @"SOFTWARE\Pst Backup"))
            {
                RegistryKey HKCU = Registry.CurrentUser;
                HKCU.DeleteSubKeyTree(@"SOFTWARE\Pst Backup", false);
                RegistryKey localSettings = HKCU.OpenSubKey(@"SOFTWARE\Pst Backup", false);
                if (localSettings != null)
                { throw new Exception("Local settings not deleted"); }
            }
        }

        internal static void DeleteGpoSettings()
        {
            if (IsRegistryKeyExists(RegistryHive.CurrentUser, @"Software\Policies\PST Backup"))
            {
                RegistryKey HKCU = Registry.CurrentUser;
                HKCU.DeleteSubKeyTree(@"Software\Policies\PST Backup", false);
                RegistryKey gpoSettings = HKCU.OpenSubKey(@"Software\Policies\PST Backup", false);
                if (gpoSettings != null)
                { throw new Exception("Gpo settings not deleted"); }
            }
        }

        internal static void DeletePstFilesEntries()
        {
            if (IsRegistryKeyExists(RegistryHive.CurrentUser, @"Software\PST Backup\PST Files"))
            {
                RegistryKey HKCU = Registry.CurrentUser;
                HKCU.DeleteSubKeyTree(@"Software\PST Backup\PST Files", false);
                RegistryKey pstFileEntries = HKCU.OpenSubKey(@"Software\PST Backup\PST Files", false);
                if (pstFileEntries != null)
                { throw new Exception("PST file entries not deleted"); }
            }
        }

        internal static void ImportRegistrySettings(string filename)
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            processInfo.FileName = "cmd.exe";
            processInfo.Arguments = "/C reg import \"" + filename + "\"";
            processInfo.ErrorDialog = false;
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;

            System.Diagnostics.Process process = new System.Diagnostics.Process();

            process.StartInfo = processInfo;
            process.Start();
            process.WaitForExit();
        }

        internal static void DeleteRegistryValue(RegistryHive hive, string keyName, string valueName)
        {
            RegistryKey targetKey = GetBaseKey(hive).OpenSubKey(keyName, true);
            targetKey.DeleteValue(valueName, false);
            if (IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup", "ClientID", RegistryValueKind.String))
                throw new Exception("Unable to delete RegistryValue");
        }

        internal static bool IsRegistryKeyExists(RegistryHive hive, string keyName)
        {
            return GetBaseKey(hive).OpenSubKey(keyName, false) != null;
        }

        internal static bool IsRegistryValueExists(RegistryHive hive, string keyName, string valueName, RegistryValueKind valueKind)
        {
            RegistryKey targetKey = GetBaseKey(hive).OpenSubKey(keyName);
            object targetValue = targetKey.GetValue(valueName, null);

            return targetValue != null && targetKey.GetValueKind(valueName) == valueKind;
        }

        internal static bool IsRegistryValueEquals(RegistryHive hive, string keyName, string valueName, object expectedValue)
        {
            RegistryKey targetKey = GetBaseKey(hive).OpenSubKey(keyName);
            object actualValue = targetKey.GetValue(valueName, null);

            return actualValue.Equals(expectedValue);
        }

        internal static object GetRegistryValue(RegistryHive hive, string keyName, string valueName)
        {
            RegistryKey targetKey = GetBaseKey(hive).OpenSubKey(keyName);
            object actualValue = targetKey.GetValue(valueName, null);

            return actualValue;
        }

        internal static RegistryKey GetBaseKey(RegistryHive hive)
        {
            RegistryKey baseKey;

            switch (hive)
            {
                case RegistryHive.CurrentUser:
                    baseKey = Registry.CurrentUser;
                    break;
                case RegistryHive.LocalMachine:
                    baseKey = Registry.LocalMachine;
                    break;
                default:
                    throw new ArgumentException("Unsupported hive");
            }

            return baseKey;
        }

        [TestClass]
        public class Constructor_Should
        {
            [TestMethod]
            public void CreateLocalSettingsWithDefaultValues_WhenDoesNotExists()
            {
                // Arrange
                DeleteLocalSettings();
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);

                // Assert
                Assert.IsTrue(IsRegistryKeyExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent"));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent", "AdditionalSubnets", RegistryValueKind.String));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent", "AdditionalSubnets", String.Empty));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent", "DontBackupThroughWan", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent", "DontBackupThroughWan", 1));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent", "BackupMethod", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Backup Agent", "BackupMethod", 0));

                Assert.IsTrue(IsRegistryKeyExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Event Log"));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Event Log", "LogEvent", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Event Log", "LogEvent", 1));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Event Log", "Severity", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Event Log", "Severity", 1));

                Assert.IsTrue(IsRegistryKeyExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders"));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "CompressFiles", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "CompressFiles", 0));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "DestinationType", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "DestinationType", 0));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "DestinationPath", RegistryValueKind.String));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "DestinationPath", String.Empty));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "BackupServer", RegistryValueKind.String));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "BackupServer", String.Empty));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "BackupPort", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Files And Folders", "BackupPort", 443));

                Assert.IsTrue(IsRegistryKeyExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting"));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting", "Report", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting", "Report", 0));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting", "Server", RegistryValueKind.String));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting", "Server", String.Empty));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting", "Port", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Reporting", "Port", 443));

                Assert.IsTrue(IsRegistryKeyExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule"));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "Policy", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "Policy", 0));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "Interval", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "Interval", 7));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "Unit", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "Unit", 0));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "DayOfWeek", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "DayOfWeek", 1));
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "DayOfMonth", RegistryValueKind.DWord));
                Assert.IsTrue(IsRegistryValueEquals(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup\Settings\Schedule", "DayOfMonth", 1));

                // Restaure Good Settings
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\Settings\DifferentialToSmbFolder.reg");
            }

            [TestMethod]
            public void SetGpoSettingsToNotDefine_WhenGpoSettingsAreMissing()
            {// Arrange
                DeleteGpoSettings();
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.GPO);

                // Assert
                Assert.IsFalse(appSettings.IsBackupAgentAdditionalSubnetsDefine);
                Assert.IsFalse(appSettings.IsBackupAgentBackupMethodDefine);
                Assert.IsFalse(appSettings.IsBackupAgentDontBackupThroughtWanDefine);
                Assert.IsFalse(appSettings.IsEventLogActivatedDefine);
                Assert.IsFalse(appSettings.IsEventLogSeverityDefine);
                Assert.IsFalse(appSettings.IsFilesAndFoldersBackupPortDefine);
                Assert.IsFalse(appSettings.IsFilesAndFoldersBackupServerDefine);
                Assert.IsFalse(appSettings.IsFilesAndFoldersCompressFilesDefine);
                Assert.IsFalse(appSettings.IsFilesAndFoldersDestinationPathDefine);
                Assert.IsFalse(appSettings.IsFilesAndFoldersDestinationTypeDefine);
                Assert.IsFalse(appSettings.IsReportingDefine);
                Assert.IsFalse(appSettings.IsReportingPortDefine);
                Assert.IsFalse(appSettings.IsReportingServerDefine);
                Assert.IsFalse(appSettings.IsScheduleDayOfMonthDefine);
                Assert.IsFalse(appSettings.IsScheduleDayOfWeekDefine);
                Assert.IsFalse(appSettings.IsScheduleintervalDefine);
                Assert.IsFalse(appSettings.IsSchedulePolicyDefine);
                Assert.IsFalse(appSettings.IsScheduleUnitDefine);
            }

            [TestMethod]
            public void ReadBackupAgentSettingsFromGPO_WhenInstructToDoSo()
            {
                // Arrange
                DeleteGpoSettings();
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\GPO\Backup Agent\Differential-DontBackupThroughWan-ExceptForTwoSubnets.reg");
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.GPO);

                // Assert
                Assert.AreEqual(appSettings.FilesAndFoldersAdditionalNTFSFullcontrol, @"ad\Courtel");
                Assert.AreEqual(appSettings.FilesAndFoldersAdditionalNTFSReadWrite, "%userLogin%");
                Assert.AreEqual(appSettings.BackupAgentAdditionalSubnets, "192.168.10.0/24;192.168.20.0/24");
                Assert.IsTrue(appSettings.IsBackupAgentAdditionalSubnetsDefine);
                Assert.AreEqual(appSettings.BackupAgentBackupMethod, SUT.BackupMethod.Differential);
                Assert.IsTrue(appSettings.IsBackupAgentBackupMethodDefine);                
                Assert.IsTrue(appSettings.BackupAgentDontBackupThroughtWan);
                Assert.IsTrue(appSettings.IsBackupAgentDontBackupThroughtWanDefine);
                Assert.IsTrue(appSettings.FilesAndFoldersSetExclusiveNTFSPermissions);
            }

            [TestMethod]
            public void ReadEventLogSettingsFromGPO_WhenInstructToDoSo()
            {
                // Arrange
                DeleteGpoSettings();
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\GPO\Event Log\LogEvent-Severity2.reg");
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.GPO);

                // Assert
                Assert.IsTrue(appSettings.EventLogActivated);
                Assert.IsTrue(appSettings.IsEventLogActivatedDefine);
                Assert.AreEqual(appSettings.EventLogSeverity, SmartSingularity.PstBackupLogger.Logger.MessageSeverity.Warning);
                Assert.IsTrue(appSettings.IsEventLogSeverityDefine);
            }

            [TestMethod]
            public void ReadFilesAndFoldersSettingsFromGPO_WhenInstructToDoSo()
            {
                // Arrange
                DeleteGpoSettings();
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\GPO\Files and Folders\DestinationServer-BackupServer-BackupPort8090-Compress-DestinationPath.reg");
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.GPO);

                // Assert
                Assert.IsTrue(appSettings.FilesAndFoldersCompressFiles);
                Assert.IsTrue(appSettings.IsFilesAndFoldersCompressFilesDefine);
                Assert.AreEqual(appSettings.FilesAndFoldersDestinationType, SUT.BackupDestinationType.BackupServer);
                Assert.IsTrue(appSettings.IsFilesAndFoldersDestinationTypeDefine);
                Assert.AreEqual(appSettings.FilesAndFoldersDestinationPath, @"\\akio9901lms.ad.fr\Backup PST\%userLogin%");
                Assert.IsTrue(appSettings.IsFilesAndFoldersDestinationPathDefine);
                Assert.AreEqual(appSettings.FilesAndFoldersBackupServer, @"Akio0512lms.ad.fr");
                Assert.IsTrue(appSettings.IsFilesAndFoldersBackupServerDefine);
                Assert.AreEqual(appSettings.FilesAndFoldersBackupPort, 8090);
                Assert.IsTrue(appSettings.IsFilesAndFoldersBackupPortDefine);
            }

            [TestMethod]
            public void ReadReportingSettingsFromGPO_WhenInstructToDoSo()
            {
                // Arrange
                DeleteGpoSettings();
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\GPO\Reporting\ReportToServer0512LMS-OnPort8090.reg");
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.GPO);

                // Assert
                Assert.IsFalse(appSettings.ReportingReportToServer);
                Assert.IsTrue(appSettings.IsReportingDefine);
                Assert.AreEqual(appSettings.ReportingServer, @"\\Akio0512lms.ad.fr");
                Assert.IsTrue(appSettings.IsReportingServerDefine);
                Assert.AreEqual(appSettings.ReportingPort, 8090);
                Assert.IsTrue(appSettings.IsReportingPortDefine);
            }

            [TestMethod]
            public void ReadScheduleSettingsFromGPO_WhenInstructToDoSo()
            {
                // Arrange
                DeleteGpoSettings();
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\GPO\Schedule\BackupEveryWeek.reg");
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.GPO);

                // Assert
                Assert.AreEqual(appSettings.SchedulePolicy, SUT.BackupPolicy.EveryX);
                Assert.IsTrue(appSettings.IsSchedulePolicyDefine);
                Assert.AreEqual(appSettings.ScheduleInterval, 8);
                Assert.IsTrue(appSettings.IsScheduleintervalDefine);
                Assert.AreEqual(appSettings.ScheduleUnit, SUT.BackupUnit.Weeks);
                Assert.IsTrue(appSettings.IsScheduleUnitDefine);
                Assert.AreEqual(appSettings.ScheduleDayOfWeek, SUT.DayOfWeek.Thursday);
                Assert.IsTrue(appSettings.IsScheduleDayOfWeekDefine);
                Assert.AreEqual(appSettings.ScheduleDayOfMonth, 21);
                Assert.IsTrue(appSettings.IsScheduleDayOfMonthDefine);
            }

            [TestMethod]
            public void CreateClientID_WhenDoesNotExists()
            {
                // Arrange
                DeleteRegistryValue(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup", "ClientID");
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);

                // Assert
                Assert.IsTrue(IsRegistryValueExists(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup", "ClientID", RegistryValueKind.String));
            }

            [TestMethod]
            public void NotChangeClientID_WhenAlreadyExists()
            {
                // Arrange
                SUT appSettings;
                string expectedValue;
                string actualValue;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                expectedValue = GetRegistryValue(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup", "ClientID").ToString();
                appSettings = new SUT(SUT.SourceSettings.Local);
                actualValue = GetRegistryValue(RegistryHive.CurrentUser, @"SOFTWARE\PST Backup", "ClientID").ToString();

                // Assert
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [TestClass]
        public class GetCorrectIntValue_Should
        {
            [TestMethod]
            public void ReturnCandidateValue_WhenBetweenMinAndMax()
            {
                // Arrange
                int expectedValue = 4;
                int actualValue;

                // Act
                actualValue = SUT.GetCorrectIntValue(expectedValue, 0, 10, 2);

                // Assert
                Assert.AreEqual(expectedValue, actualValue);
            }

            [TestMethod]
            public void ReturnCandidateValue_WhenEqualToMin()
            {
                // Arrange
                int expectedValue = 0;
                int actualValue;

                // Act
                actualValue = SUT.GetCorrectIntValue(expectedValue, 0, 10, 2);

                // Assert
                Assert.AreEqual(expectedValue, actualValue);
            }

            [TestMethod]
            public void ReturnCandidateValue_WhenEqualToMax()
            {
                // Arrange
                int expectedValue = 10;
                int actualValue;

                // Act
                actualValue = SUT.GetCorrectIntValue(expectedValue, 0, 10, 2);

                // Assert
                Assert.AreEqual(expectedValue, actualValue);
            }

            [TestMethod]
            public void ReturnDefaultValue_WhenOutOfRange()
            {
                // Arrange
                int expectedValue = 2;
                int actualValue;

                // Act
                actualValue = SUT.GetCorrectIntValue(20, 0, 10, 2);

                // Assert
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [TestClass]
        public class GetPstRegistryEntries_Should
        {
            [TestMethod]
            public void CreatePstRegistryEntriesAccordingToRegistryValues_WhenCall()
            {
                // Arrange
                DeletePstFilesEntries();
                ImportRegistrySettings(@"E:\Pst Backup\Registry Files\PST Entries\ThreeFiles.reg");
                List<SmartSingularity.PstBackupSettings.PSTRegistryEntry> pstFiles;

                // Act
                pstFiles = SUT.GetPstRegistryEntries();

                // Assert
                Assert.AreEqual(3, pstFiles.Count);

                Assert.IsTrue(pstFiles[0].ToBackup);
                Assert.AreEqual(@"E:\Pst Backup\Pst Files\archive2011.pst", pstFiles[0].SourcePath, true);
                Assert.AreEqual(@"HKEY_CURRENT_USER\Software\PST Backup\PST Files\0", pstFiles[0].RegistryPath,true);
                Assert.AreEqual(new DateTime(2017, 04, 15, 17, 00, 12), pstFiles[0].LastSuccessfulBackup);

                Assert.IsFalse(pstFiles[1].ToBackup);
                Assert.AreEqual(@"E:\Pst Backup\Pst Files\archive2012.pst", pstFiles[1].SourcePath,true);
                Assert.AreEqual(@"HKEY_CURRENT_USER\Software\PST Backup\PST Files\1", pstFiles[1].RegistryPath,true);
                Assert.AreEqual(new DateTime(2017, 04, 15, 17, 12, 12), pstFiles[1].LastSuccessfulBackup);

                Assert.IsTrue(pstFiles[2].ToBackup);
                Assert.AreEqual(@"E:\Pst Backup\Pst Files\archive2010.pst", pstFiles[2].SourcePath,true );
                Assert.AreEqual(@"HKEY_CURRENT_USER\Software\PST Backup\PST Files\2", pstFiles[2].RegistryPath,true);
                Assert.AreEqual(new DateTime(2017, 04, 15, 17, 22, 12), pstFiles[2].LastSuccessfulBackup);
            }
        }

        [TestClass]
        public class IsDestinationDefine_Should
        {
            [TestMethod]
            public void RetunsTrue_WhenFolderDestinationIsWellDefine()
            {
                // Arrange
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                appSettings.FilesAndFoldersDestinationType = SUT.BackupDestinationType.FileSystem;
                appSettings.FilesAndFoldersDestinationPath = @"\\192.168.0.250\PST Files\Courtel\MyComputer";

                // Assert
                Assert.IsTrue(appSettings.IsDestinationProperlyDefine());
            }

            [TestMethod]
            public void RetunsFalse_WhenFolderDestinationIsNotDefine()
            {
                // Arrange
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                appSettings.FilesAndFoldersDestinationType = SUT.BackupDestinationType.FileSystem;
                appSettings.FilesAndFoldersDestinationPath = String.Empty;

                // Assert
                Assert.IsFalse(appSettings.IsDestinationProperlyDefine());
            }

            [TestMethod]
            public void RetunsFalse_WhenFolderDestinationIsWhitespaced()
            {
                // Arrange
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                appSettings.FilesAndFoldersDestinationType = SUT.BackupDestinationType.FileSystem;
                appSettings.FilesAndFoldersDestinationPath = "   ";

                // Assert
                Assert.IsFalse(appSettings.IsDestinationProperlyDefine());
            }

            [TestMethod]
            public void RetunsTrue_WhenBackupServerIsDefine()
            {
                // Arrange
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                appSettings.FilesAndFoldersDestinationType = SUT.BackupDestinationType.BackupServer;
                appSettings.FilesAndFoldersBackupServer = "192.168.0.250";

                // Assert
                Assert.IsTrue(appSettings.IsDestinationProperlyDefine());
            }

            [TestMethod]
            public void RetunsFalse_WhenBackupServerIsNotDefine()
            {
                // Arrange
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                appSettings.FilesAndFoldersDestinationType = SUT.BackupDestinationType.BackupServer;
                appSettings.FilesAndFoldersBackupServer = String.Empty;

                // Assert
                Assert.IsFalse(appSettings.IsDestinationProperlyDefine());
            }

            [TestMethod]
            public void RetunsFalse_WhenBackupServerIsWhitespaced()
            {
                // Arrange
                SUT appSettings;

                // Act
                appSettings = new SUT(SUT.SourceSettings.Local);
                appSettings.FilesAndFoldersDestinationType = SUT.BackupDestinationType.BackupServer;
                appSettings.FilesAndFoldersBackupServer = "  ";

                // Assert
                Assert.IsFalse(appSettings.IsDestinationProperlyDefine());
            }
        }
    }
}
