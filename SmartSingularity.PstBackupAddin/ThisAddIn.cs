using System;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;
using Logger = SmartSingularity.PstBackupLogger.Logger;
using SmartSingularity.PstBackupSettings;

namespace SmartSingularity.PstBackupAddin
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                ((Outlook.ApplicationEvents_11_Event)Application).Quit += ThisAddIn_Quit;
            }
            catch (Exception ex)
            {
                Logger.Write(20017, "An error occurs while linking to Application.Quit in Outlook.exe.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void ThisAddIn_Quit()
        {
            try
            {
                Logger.Write(30001, "Outlook is closing.", Logger.MessageSeverity.Debug);
                UpdateRegistryEntries();

                System.IO.FileInfo currentAssembly = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string currentPath = currentAssembly.DirectoryName;

#if (DEBUG)
            string bckAgentPath = @"C:\Users\Courtel\Documents\Visual Studio 2017\Projects\SmartSingularity.PstBackup\SmartSingularity.PstBackupAgent\bin\Debug\SmartSingularity.PstBackupAgent.exe";
#else

                string bckAgentPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Pst Backup", "SmartSingularity.PstBackupAgent.exe");
                Logger.Write(30000, "Launching Backup-Agent at " + bckAgentPath, Logger.MessageSeverity.Debug);
#endif
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(bckAgentPath));
            }
            catch (Exception ex) { Logger.Write(20000, "Unable to start Backup-Agent." + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
        }

        /// <summary>
        /// Reregister all PST files in the registry
        /// </summary>
        internal static void UpdateRegistryEntries()
        {
            List<string> pstFiles = GetPstFileList();
            List<PSTRegistryEntry> registryEntries = ApplicationSettings.GetPstRegistryEntries();

            RemoveOutdatedRegistryEntries(registryEntries, pstFiles);
            RegisterNewPstFilesInRegistry(registryEntries, pstFiles);
        }

        /// <summary>
        /// Gets the list of all Pst files mount in Outlook.
        /// </summary>
        /// <returns>A list of file path to each pst file mount in Outlook</returns>
        public static List<string> GetPstFileList()
        {
            List<string> pstFiles = new List<string>();

            try
            {
                Outlook.Stores stores = new Outlook.Application().Session.Stores;
                foreach (Outlook.Store store in stores)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(store.FilePath) && store.IsDataFileStore && !store.IsCachedExchange)
                        {
                            Logger.Write(30002, "Adding " + store.FilePath.ToLower() + " to the PST list.", Logger.MessageSeverity.Debug);
                            pstFiles.Add(store.FilePath.ToLower());
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(20001, "Unable to enumerate PST files.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error);
            }

            return pstFiles;
        }

        /// <summary>
        /// Remove items from registryEntries that or not present in the pstFiles list
        /// </summary>
        /// <param name="registryEntries">A list of registry entry</param>
        /// <param name="pstFiles">A list of pst files</param>
        public static void RemoveOutdatedRegistryEntries(List<PSTRegistryEntry> registryEntries, List<string> pstFiles)
        {
            try
            {
                List<PSTRegistryEntry> outdatedEntries = new List<PSTRegistryEntry>();

                foreach (var registryEntry in registryEntries)
                {
                    if (!pstFiles.Contains(registryEntry.SourcePath.ToLower()))
                    { outdatedEntries.Add(registryEntry); }
                }

                foreach (var outdatedEntry in outdatedEntries)
                {
                    registryEntries.Remove(outdatedEntry);
                }
            }
            catch (Exception ex) { Logger.Write(20018, "An error occurs while removing outdated registry entries.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
        }

        /// <summary>
        /// Add into Registry, new PST files that or not already registred in the registry
        /// </summary>
        /// <param name="registryEntries">List of all registry entries.</param>
        /// <param name="pstFileList">List of all PST files mounted in Outlook.</param>
        public static void RegisterNewPstFilesInRegistry(List<PSTRegistryEntry> registryEntries, List<string> pstFileList)
        {
            try
            {
                foreach (string pstFile in pstFileList)
                {
                    bool found = false;

                    foreach (PSTRegistryEntry regEntry in registryEntries)
                    {
                        if (regEntry.SourcePath.ToLower() == pstFile.ToLower())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        PSTRegistryEntry regEntry = new PSTRegistryEntry(pstFile.ToLower());
                        registryEntries.Add(regEntry);
                    }
                }
            }
            catch (Exception ex) { Logger.Write(20019, "An error occurs while registering new PST files.\r\n" + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }

            DeletePstFileRegistryEntries();
            for (int i = 0; i < registryEntries.Count; i++)
            {
                var regEntry = registryEntries[i];
                regEntry.RegistryPath = @"HKEY_CURRENT_USER\Software\PST Backup\PST Files\" + i.ToString();
                regEntry.Save();
            }
        }

        /// <summary>
        /// Delete all PST file entries in the registry
        /// </summary>
        private static void DeletePstFileRegistryEntries()
        {
            try
            {
                Microsoft.Win32.RegistryKey HKCU = Microsoft.Win32.Registry.CurrentUser;
                HKCU.DeleteSubKeyTree(@"Software\PST Backup\PST Files", false);
            }
            catch (Exception) { }
        }

        #region Code généré par VSTO

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
        }

        #endregion
    }
}
