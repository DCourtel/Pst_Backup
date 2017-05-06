using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSingularity.PstBackupLogger;
using SmartSingularity.PstBackupSettings;

namespace SmartSingularity.PstBackupEngine
{
    public class Scheduler
    {
        private static ApplicationSettings _localSettings;

        public static bool IsPstFileNeedtoBeSaved(DateTime lastSuccessfulBackup, ApplicationSettings appSettings)
        {
            _localSettings = appSettings;
            int result = GetNewBackupDate(lastSuccessfulBackup).CompareTo(DateTime.Now);
            Logger.Write(30000, "Pst need to be saved (" + lastSuccessfulBackup.ToString() + "): " + (result <= 0), Logger.MessageSeverity.Debug);
            return (result <= 0);
        }

        private static DateTime GetNewBackupDate(DateTime lastSuccessfulBackup)
        {
            DateTime newBackupDate = DateTime.Now;

            switch (_localSettings.SchedulePolicy)
            {
                case ApplicationSettings.BackupPolicy.EveryX:
                    newBackupDate = GetBackupDateEvery(lastSuccessfulBackup);
                    break;
                case ApplicationSettings.BackupPolicy.Weekly:
                    newBackupDate = GetBackupDateWeekly(lastSuccessfulBackup);
                    break;
                case ApplicationSettings.BackupPolicy.Monthly:
                    newBackupDate = GetBackupDateMonthly(lastSuccessfulBackup);
                    break;
                default:
                    Logger.Write(10004, "Unknown backup method", Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                    break;
            }
            Logger.Write(30000, "Get new backup date for " + lastSuccessfulBackup.ToString() + " : " + newBackupDate.ToString(), Logger.MessageSeverity.Debug);
            return newBackupDate;
        }

        private static DateTime GetBackupDateEvery(DateTime lastsuccessfulBackup)
        {
            DateTime newBackupDate = DateTime.Now;

            try
            {
                switch (_localSettings.ScheduleUnit)
                {
                    case ApplicationSettings.BackupUnit.Days:
                        Logger.Write(30000, "Backup Policy is 'Every x Days'", Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Information);
                        newBackupDate = lastsuccessfulBackup.AddDays((double)_localSettings.ScheduleInterval);
                        break;
                    case ApplicationSettings.BackupUnit.Weeks:
                        Logger.Write(30000, "Backup Policy is 'Every x Weeks'", Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Information);
                        newBackupDate = lastsuccessfulBackup.AddDays((double)_localSettings.ScheduleInterval * 7);
                        break;
                    case ApplicationSettings.BackupUnit.Months:
                        Logger.Write(30000, "Backup Policy is 'Every x Months'", Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Information);
                        newBackupDate = lastsuccessfulBackup.AddMonths(_localSettings.ScheduleInterval);
                        break;
                    default:
                        Logger.Write(10005, "Unknown backup interval", Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                        break;
                }
            }
            catch (Exception ex) { Logger.Write(30000, ex.Message, Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Error); }

            return newBackupDate;
        }

        private static DateTime GetBackupDateWeekly(DateTime lastsuccessfulBackup)
        {
            DateTime newBackupDate = DateTime.Now;

            try
            {
                Logger.Write(30000, "Backup Policy is 'Weekly' on " + _localSettings.ScheduleDayOfWeek, Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Information);
                List<DateTime> thisWeek = GetWeekDays(lastsuccessfulBackup);
                List<DateTime> nextWeek = GetWeekDays(thisWeek[0].AddDays(7));
                newBackupDate = nextWeek[(int)_localSettings.ScheduleDayOfWeek];
            }
            catch (Exception ex) { Logger.Write(30000, ex.Message, Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Error); }

            return newBackupDate;
        }

        private static DateTime GetBackupDateMonthly(DateTime lastsuccessfulBackup)
        {
            DateTime nextBackupDate = DateTime.Now;

            try
            {
                Logger.Write(30000, "Backup Policy is 'Monthly' on " + _localSettings.ScheduleDayOfMonth, Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Information);
                DateTime nextMonth = new DateTime(lastsuccessfulBackup.Year, lastsuccessfulBackup.Month, 1).AddMonths(1);

                if (DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month) < _localSettings.ScheduleDayOfMonth) // Not enought days in the next month.
                    nextBackupDate = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month)); // Schedule backup at the last day of the month.
                else
                    nextBackupDate = new DateTime(nextMonth.Year, nextMonth.Month, _localSettings.ScheduleDayOfMonth); // Schedule backup for the next month.
            }
            catch (Exception ex) { Logger.Write(30000, ex.Message, Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Error); }

            return nextBackupDate;
        }

        private static List<DateTime> GetWeekDays(DateTime dayOfWeek)
        {
            List<DateTime> thisWeek = new List<DateTime>();

            int days = (int)dayOfWeek.DayOfWeek;
            DateTime dt = dayOfWeek.AddDays(-days);
            thisWeek.Add(dt);
            thisWeek.AddRange(new DateTime[] { dt.AddDays(1), dt.AddDays(2), dt.AddDays(3), dt.AddDays(4), dt.AddDays(5), dt.AddDays(6) });

            return thisWeek;
        }

    }
}
