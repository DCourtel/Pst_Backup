using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSingularity.PstBackupSettings;
using SUT = SmartSingularity.PstBackupEngine.Scheduler;


namespace UnitTest_Scheduler
{
    public class Scheduler
    {
        [TestClass]
        public class IsPstFileNeedtoBeSaved_Should
        {
            private static ApplicationSettings _policyEvery7Days = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
            private static ApplicationSettings _policyEvery2Weeks = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
            private static ApplicationSettings _policyEvery2Month = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
            private static ApplicationSettings _policyWeekly = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);
            private static ApplicationSettings _policyMonthly = new ApplicationSettings(ApplicationSettings.SourceSettings.Local);

            private static DateTime _today;
            private static DateTime _tomorrow;
            private static DateTime _8DaysAgo;
            private static DateTime _7DaysAgo;
            private static DateTime _6DaysAgo;
            private static DateTime _15DaysAgo;
            private static DateTime _14DaysAgo;
            private static DateTime _13DaysAgo;
            private static DateTime _1MonthAgo;
            private static DateTime _2MonthAgo;

            [ClassInitialize]
            public static void ClassInitialize(TestContext context)
            {
                _policyEvery7Days.SchedulePolicy = ApplicationSettings.BackupPolicy.EveryX;
                _policyEvery7Days.ScheduleInterval = 7;
                _policyEvery7Days.ScheduleUnit = ApplicationSettings.BackupUnit.Days;

                _policyEvery2Weeks.SchedulePolicy = ApplicationSettings.BackupPolicy.EveryX;
                _policyEvery2Weeks.ScheduleInterval = 2;
                _policyEvery2Weeks.ScheduleUnit = ApplicationSettings.BackupUnit.Weeks;

                _policyEvery2Month.SchedulePolicy = ApplicationSettings.BackupPolicy.EveryX;
                _policyEvery2Month.ScheduleInterval = 2;
                _policyEvery2Month.ScheduleUnit = ApplicationSettings.BackupUnit.Months;

                _policyWeekly.SchedulePolicy = ApplicationSettings.BackupPolicy.Weekly;
                _policyWeekly.ScheduleDayOfWeek = ApplicationSettings.DayOfWeek.Friday;

                _policyMonthly.SchedulePolicy = ApplicationSettings.BackupPolicy.Monthly;
                
                _today = DateTime.Now;
                _tomorrow = _today.AddDays(1);
                _8DaysAgo = _today.AddDays(-8);
                _7DaysAgo = _today.AddDays(-7);
                _6DaysAgo = _today.AddDays(-6);
                _15DaysAgo = _today.AddDays(-15);
                _14DaysAgo = _today.AddDays(-14);
                _13DaysAgo = _today.AddDays(-13);
                _2MonthAgo = _today.AddMonths(-2);
                _1MonthAgo = _today.AddMonths(-1);

            }

            // Backup Every 7 Days

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToEvery7DaysAndLastBackupWas8DaysAgo()
            {
                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_8DaysAgo, _policyEvery7Days));
            }

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToEvery7DaysAndLastBackupWas7DaysAgo()
            {
                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_7DaysAgo, _policyEvery7Days));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToEvery7DaysAndLastBackupWas6DaysAgo()
            {
                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_6DaysAgo, _policyEvery7Days));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToEvery7DaysAndLastBackupWasToday()
            {
                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_today, _policyEvery7Days));
            }

            // Backup Every 2 Weeks

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToEvery2WeeksAndLastBackupWas15DaysAgo()
            {
                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_15DaysAgo, _policyEvery2Weeks));
            }

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToEvery2WeeksAndLastBackupWas14DaysAgo()
            {
                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_14DaysAgo, _policyEvery2Weeks));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToEvery2WeeksAndLastBackupWas13DaysAgo()
            {
                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_13DaysAgo, _policyEvery2Weeks));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToEvery2WeeksAndLastBackupWasToday()
            {
                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_today, _policyEvery2Weeks));
            }

            // Backup Every 2 Month

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToEvery2MonthsAndLastBackupWas2MonthsAnd1DayAgo()
            {
                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_2MonthAgo.AddDays(-1), _policyEvery2Month));
            }

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToEvery2MonthsAndLastBackupWas2MonthsAgo()
            {
                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_2MonthAgo, _policyEvery2Month));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToEvery2MonthsAndLastBackupWas2MonthMinus1DayAgo()
            {
                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_2MonthAgo.AddDays(1), _policyEvery2Month));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToEvery2MonthsAndLastBackupWasToday()
            {
                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_today, _policyEvery2Month));
            }

            // Backup Weekly

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToBackupTodayAndLastBackupWasLastWeek()
            {
                // Arrange
                _policyWeekly.ScheduleDayOfWeek = (ApplicationSettings.DayOfWeek)((int)_today.DayOfWeek);

                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_today.AddDays(-7), _policyWeekly));
            }

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToBackupYesterday()
            {
                // Arrange
                _policyWeekly.ScheduleDayOfWeek = (ApplicationSettings.DayOfWeek)((int)_8DaysAgo.DayOfWeek);

                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_8DaysAgo, _policyWeekly));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToBackupTomorrow()
            {
                // Arrange
                _policyWeekly.ScheduleDayOfWeek = (ApplicationSettings.DayOfWeek)((int)_tomorrow.DayOfWeek);

                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_6DaysAgo, _policyWeekly));
            }
                        
            // Backup Monthly

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToBackupMonthlyAndLastBackupWasLastMonth()
            {
                // Arrange
                _policyMonthly.ScheduleDayOfMonth = _today.Day;

                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_1MonthAgo, _policyMonthly));
            }

            [TestMethod]
            public void ReturnTrue_WhenPolicyIsSetToBackupMonthlyAndLastBackupWasLastMonthMinusOneDay()
            {
                // Arrange
                _policyMonthly.ScheduleDayOfMonth = _today.Day -1;

                // Assert
                Assert.IsTrue(SUT.IsPstFileNeedtoBeSaved(_1MonthAgo, _policyMonthly));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToBackupMonthlyAndLastBackupWasLastMonthPlusOneDay()
            {
                // Arrange
                _policyMonthly.ScheduleDayOfMonth = _today.Day + 1;

                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_1MonthAgo, _policyMonthly));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToBackupMonthlyAndLastBackupWasEarlierThisMonth()
            {
                // Arrange
                _policyMonthly.ScheduleDayOfMonth = _today.Day - 1;

                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_today.AddDays( - 1), _policyMonthly));
            }

            [TestMethod]
            public void ReturnFalse_WhenPolicyIsSetToBackupMonthlyAndLastBackupWasEarlierToday()
            {
                // Arrange
                _policyMonthly.ScheduleDayOfMonth = _today.Day;

                // Assert
                Assert.IsFalse(SUT.IsPstFileNeedtoBeSaved(_today, _policyMonthly));
            }
        }
    }
}
