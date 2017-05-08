using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using SmartSingularity.PstBackupLogger;
using System.Threading.Tasks;

namespace SmartSingularity.PstBackupSettings
{
    /// <summary>
    /// A class to store informations on a PST file mount in Outlook
    /// </summary>
    public class PSTRegistryEntry : IComparable
    {
        public PSTRegistryEntry()
        {
            RegistryPath = String.Empty;
            SourcePath = String.Empty;
            ToBackup = true;
            LastSuccessfulBackup = new DateTime();
        }

        public PSTRegistryEntry(string sourcePath)
        {
            RegistryPath = String.Empty;
            SourcePath = sourcePath.ToLower();
            ToBackup = true;
            LastSuccessfulBackup = new DateTime();
        }

        public string RegistryPath { get; set; }
        public string SourcePath { get; set; }
        public bool ToBackup { get; set; }
        public DateTime LastSuccessfulBackup { get; set; }

        /// <summary>
        /// Save informations into the registry
        /// </summary>
        public void Save()
        {
            try
            {
                Registry.SetValue(RegistryPath, "SourcePath", SourcePath);
                Registry.SetValue(RegistryPath, "Backup", ToBackup ? 1 : 0);
                Registry.SetValue(RegistryPath, "LastSuccessfulBackup", LastSuccessfulBackup.ToString());
            }
            catch (Exception ex)
            {
                Logger.Write(30000, "Error when writting pst file settings into registry : " + ex.Message, Logger.MessageSeverity.Debug, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public override string ToString() { return SourcePath; }

        public int CompareTo(object obj)
        {
            return DateTime.Compare(this.LastSuccessfulBackup, (obj as PSTRegistryEntry).LastSuccessfulBackup);
        }

    }
}
