using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using SmartSingularity.PstBackupSettings;
using SUT = SmartSingularity.PstBackupAddin.ThisAddIn;

namespace UnitTest_PstBackupAddin
{
    public class Addin
    {
        internal static void DeletePstFilesEntries()
        {
            if (IsRegistryKeyExists(RegistryHive.CurrentUser, @"Software\PST Backup\PST Files"))
            {
                RegistryKey HKCU = Registry.CurrentUser;
                HKCU.DeleteSubKeyTree(@"Software\PST Backup\PST Files", false);
                RegistryKey pstFileEntries = HKCU.OpenSubKey(@"Software\PST Backup\PST Files", false);
                if (pstFileEntries != null)
                { throw new Exception("PST files entries not deleted"); }
            }
        }

        internal static bool IsRegistryKeyExists(RegistryHive hive, string keyName)
        {
            return GetBaseKey(hive).OpenSubKey(keyName, false) != null;
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
        public class GetPstFilesList_Should
        {
            [TestMethod]
            public void ReturnTheListOfAllPstFilesMountInOutlook_WhenCalled()
            {
                // Arrange
                List<string> pstFiles = new List<string>();

                // Act
                pstFiles = SUT.GetPstFileList();

                // Assert
                Assert.AreEqual(5, pstFiles.Count);

                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\Archive2009.pst".ToLower()));
                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\archive2010.pst".ToLower()));
                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\archive2011.pst".ToLower()));
                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\archive2012.pst".ToLower()));
            }
        }

        [TestClass]
        public class RemoveOutdatedRegistryEntries_Should
        {
            [TestMethod]
            public void RemoveRegistryEntries_WhenTheyAreNotInThePstFileList()
            {
                // Arrange
                List<PSTRegistryEntry> registryEntries = new List<PSTRegistryEntry>();
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2009.pst"));
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2010.pst"));
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2011.pst"));
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2006.pst"));  // Outdated
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2005.pst"));  // Outdated
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2004.pst"));  // Outdated
                List<string> pstFiles = new List<string>();
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2009.pst".ToLower());
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2010.pst".ToLower());
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2011.pst".ToLower());

                // Act
                SUT.RemoveOutdatedRegistryEntries(registryEntries, pstFiles);

                // Assert
                Assert.AreEqual(3, registryEntries.Count);
                Assert.IsFalse(registryEntries.Contains(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2006.pst")));
                Assert.IsFalse(registryEntries.Contains(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2005.pst")));
                Assert.IsFalse(registryEntries.Contains(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2004.pst")));
            }

            [TestMethod]
            public void DoNotRemoveRegistryEntries_WhenTheyAreInThePstFileList()
            {
                // Arrange
                List<PSTRegistryEntry> registryEntries = new List<PSTRegistryEntry>();
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2009.pst"));
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2010.pst"));
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2011.pst"));
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2006.pst"));  // Outdated
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2005.pst"));  // Outdated
                registryEntries.Add(new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2004.pst"));  // Outdated
                List<string> pstFiles = new List<string>();
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2009.pst".ToLower());
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2010.pst".ToLower());
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2011.pst".ToLower());

                // Act
                SUT.RemoveOutdatedRegistryEntries(registryEntries, pstFiles);

                // Assert
                Assert.AreEqual(3, registryEntries.Count);
                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\Archive2009.pst".ToLower()));
                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\Archive2010.pst".ToLower()));
                Assert.IsTrue(pstFiles.Contains(@"E:\Pst Backup\Pst Files\Archive2011.pst".ToLower()));
            }
        }

        [TestClass]
        public class RegisterNewPstFilesInRegistry_Should
        {
            [TestMethod]
            public void CreateNewEntryInRegistry_WhenThereIsNewPstFiles()
            {
                // Arrange
                List<PSTRegistryEntry> backupEntries = ApplicationSettings.GetPstRegistryEntries();
                DeletePstFilesEntries();
                int index = 0;
                List<PSTRegistryEntry> registryEntries = new List<PSTRegistryEntry>();
                PSTRegistryEntry regEntry;
                regEntry = new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2009.pst");
                regEntry.RegistryPath = @"HKEY_CURRENT_USER\Software\PST Backup\PST Files\" + index;
                registryEntries.Add(regEntry);
                index++;
                regEntry = new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2010.pst");
                regEntry.RegistryPath = @"HKEY_CURRENT_USER\Software\PST Backup\PST Files\" + index;
                registryEntries.Add(regEntry);
                index++;
                regEntry = new PSTRegistryEntry(@"E:\Pst Backup\Pst Files\Archive2011.pst");
                regEntry.RegistryPath = @"HKEY_CURRENT_USER\Software\PST Backup\PST Files\" + index;
                registryEntries.Add(regEntry);
                index++;
                foreach (var registryEntry in registryEntries)
                {
                    registryEntry.Save();
                }
                List<string> pstFiles = new List<string>();
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2005.pst".ToLower());
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2006.pst".ToLower());
                pstFiles.Add(@"E:\Pst Backup\Pst Files\Archive2007.pst".ToLower());
                
                // Act
                SUT.RegisterNewPstFilesInRegistry(registryEntries, pstFiles);
                registryEntries = ApplicationSettings.GetPstRegistryEntries();
                // Restore registry entries
                DeletePstFilesEntries();
                foreach (var registryEntry in backupEntries)
                {
                    registryEntry.Save();
                }

                // Assert
                Assert.AreEqual(6, registryEntries.Count);
                foreach (var pstFile in pstFiles)
                {
                    bool found = false;
                    foreach (var pstRegistryEntry in registryEntries)
                    {
                        if (pstRegistryEntry.SourcePath == pstFile)
                        { found = true;
                            break;
                        }
                    }
                    Assert.IsTrue(found);                    
                }
            }
        }
    }
}
