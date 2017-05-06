using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SUT = SmartSingularity.PstBackupEngine.CoreBackupEngine;
using SmartSingularity.PstBackupSettings;

namespace UnitTest_CoreBackupEngine
{
    public class CoreBackupEngine
    {
        [TestClass]
        public class SelectPstFilesToSave_Should
        {
            private static List<PSTRegistryEntry> _allPstFiles = new List<PSTRegistryEntry>();
            private static List<PSTRegistryEntry> _PstFilesToSave = new List<PSTRegistryEntry>();
            private static List<PSTRegistryEntry> _PstFilesToNotSave = new List<PSTRegistryEntry>();
            private static ApplicationSettings appSettings = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);

            [ClassInitialize]
            public static void ClassInitialize(TestContext context)
            {
                DateTime _today = DateTime.Now;
                DateTime _lastMonth = _today.AddMonths(-1);
                DateTime _yesterday = _today.AddDays(-1);

                PSTRegistryEntry _toBackup_today = new PSTRegistryEntry();
                _toBackup_today.ToBackup = true;
                _toBackup_today.LastSuccessfulBackup = _today;

                PSTRegistryEntry _toBackup_lastMonth = new PSTRegistryEntry();
                _toBackup_lastMonth.ToBackup = true;
                _toBackup_lastMonth.LastSuccessfulBackup = _lastMonth;

                PSTRegistryEntry _toBackup_yesterday = new PSTRegistryEntry();
                _toBackup_yesterday.ToBackup = true;
                _toBackup_yesterday.LastSuccessfulBackup = _yesterday;

                PSTRegistryEntry _toNotBackup_today = new PSTRegistryEntry();
                _toNotBackup_today.ToBackup = false;
                _toNotBackup_today.LastSuccessfulBackup = _today;

                PSTRegistryEntry _toNotBackup_lastMonth = new PSTRegistryEntry();
                _toNotBackup_lastMonth.ToBackup = false;
                _toNotBackup_lastMonth.LastSuccessfulBackup = _lastMonth;

                PSTRegistryEntry _toNotBackup_yesterday = new PSTRegistryEntry();
                _toNotBackup_yesterday.ToBackup = false;
                _toNotBackup_yesterday.LastSuccessfulBackup = _yesterday;

                _allPstFiles.Add(_toBackup_yesterday);
                _allPstFiles.Add(_toBackup_today);
                _allPstFiles.Add(_toBackup_lastMonth);
                _allPstFiles.Add(_toNotBackup_lastMonth);
                _allPstFiles.Add(_toNotBackup_today);
                _allPstFiles.Add(_toNotBackup_yesterday);

                appSettings.SchedulePolicy = ApplicationSettings.BackupPolicy.EveryX;
                appSettings.ScheduleInterval = 8;
                appSettings.ScheduleUnit = ApplicationSettings.BackupUnit.Days;
            }

            [TestMethod]
            public void SelectPstFiles_WhenLastSuccessfulBackupIsExpired()
            {
                // Arrange
                _PstFilesToSave.Clear();
                _PstFilesToNotSave.Clear();
                SUT coreEngine = new SUT(appSettings);

                // Act
                coreEngine.SelectPstFilesToSave(_allPstFiles, out _PstFilesToSave, out _PstFilesToNotSave);

                // Assert
                Assert.IsTrue(_PstFilesToSave.Count == 1);
                Assert.IsTrue(_PstFilesToSave[0].ToBackup);
                Assert.AreEqual(DateTime.Now.AddMonths(-1).Date, _PstFilesToSave[0].LastSuccessfulBackup.Date);
                Assert.AreEqual(DateTime.Now.AddMonths(-1).Month, _PstFilesToSave[0].LastSuccessfulBackup.Month);
                Assert.AreEqual(DateTime.Now.AddMonths(-1).Year, _PstFilesToSave[0].LastSuccessfulBackup.Year);
                Assert.IsTrue(_PstFilesToNotSave.Count == 5);
            }

            [TestMethod]
            public void SelectPstFiles_WhenFileHasNeverBeenSaved()
            {
                // Arrange
                _PstFilesToSave.Clear();
                _PstFilesToNotSave.Clear();
                _allPstFiles.Clear();
                _allPstFiles.Add(new PSTRegistryEntry(String.Empty));
                SUT coreEngine = new SUT(appSettings);

                // Act
                coreEngine.SelectPstFilesToSave(_allPstFiles, out _PstFilesToSave, out _PstFilesToNotSave);

                // Assert
                Assert.IsTrue(_PstFilesToSave.Count == 1);
                Assert.IsTrue(_PstFilesToSave[0].ToBackup);
                Assert.IsTrue(_PstFilesToNotSave.Count == 0);
            }
        }
    }
}
