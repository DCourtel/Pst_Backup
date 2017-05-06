using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupFileSystem
{
    public class FileSystem
    {
        #region Windows API

        const int NO_ERROR = 0;
        const int ERROR_INSUFFICIENT_BUFFER = 122;
        const int ERROR_INVALID_FLAGS = 1004;

        enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup,
            SidTypeDomain,
            SidTypeAlias,
            SidTypeWellKnownGroup,
            SidTypeDeletedAccount,
            SidTypeInvalid,
            SidTypeUnknown,
            SidTypeComputer
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool LookupAccountName(
             string lpSystemName,
             string lpAccountName,
             [MarshalAs(UnmanagedType.LPArray)] byte[] Sid,
             ref uint cbSid,
             StringBuilder ReferencedDomainName,
             ref uint cchReferencedDomainName,
             out SID_NAME_USE peUse);

        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ConvertSidToStringSid(
            [MarshalAs(UnmanagedType.LPArray)] byte[] pSID,
            out IntPtr ptrSid);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);

        #endregion Windows API

        /// <summary>
        /// Create a folder and set NTFS permissions accordingly to the parameters
        /// </summary>
        /// <param name="fullPath">Full path to the folder to create</param>
        /// <param name="setNtfsRights">True to personnalize NTFS permissions</param>
        /// <param name="fullControlMembers">Users and Groups that will have NTFS Full Control permission</param>
        /// <param name="readWriteMembers">Users and Groups that will have NTFS Read and Write permission</param>
        public static void CreateDestinationFolder(string fullPath, bool setNtfsRights, string fullControlMembers, string readWriteMembers)
        {
            DirectoryInfo destinationFolder = new DirectoryInfo(ExpandDestinationFolder(fullPath));

            if (setNtfsRights)
            {
                Logger.Write(30000, "Creating destination folder with personnalized NTFS permissions", Logger.MessageSeverity.Debug);
                destinationFolder.Create(GetDirectorySecurity(fullControlMembers, readWriteMembers));
            }
            else
            {
                Logger.Write(30000, "Creating destination folder with inherited NTFS permissions", Logger.MessageSeverity.Debug);
                destinationFolder.Create();
            }
        }

        /// <summary>
        /// Replace %userlogin% by the login of the current user, and %computename% by the name of the computer
        /// </summary>
        /// <param name="fullPath">A string that may contains %userlogin% or %computername%</param>
        /// <returns>Returns a string where %userlogin% and %computername% have been replace by their respective value</returns>
        public static string ExpandDestinationFolder(string fullPath)
        {
            Logger.Write(30000, "Expanding destination path : " + fullPath, Logger.MessageSeverity.Debug);
            fullPath = fullPath.ToLower();
            fullPath = fullPath.Replace("%userlogin%", Environment.UserName);
            fullPath = fullPath.Replace("%computername%", Environment.MachineName);
            Logger.Write(30000, "Expanded path is : " + fullPath, Logger.MessageSeverity.Debug);

            return fullPath;
        }

        /// <summary>
        /// Expand or Shrink a file to reached the specified lenght
        /// </summary>
        /// <param name="filename">Full path to the file. If the file doesn't exists, it is created.</param>
        /// <param name="lenght">New lenght of the file.</param>
        /// <returns>Returns -1 if the file have been shrinked, 1 if the file have been expanded and 0 if the file wasn't exists or have the same size</returns>
        public static int ResizeFile(string filename, long lenght)
        {
            FileInfo fileToResize = new FileInfo(filename);
            Logger.Write(30000, "Resizing " + filename + " to " + lenght, Logger.MessageSeverity.Debug);

            if (!fileToResize.Exists)                             // File does not exists, create an empty one
            {
                using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Logger.Write(30000, "Creating file : " + filename, Logger.MessageSeverity.Debug);
                    fs.SetLength(lenght);
                }
            }
            else
            {
                if (fileToResize.Length < lenght)        // The file is too small, expand it
                {
                    using (var fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        Logger.Write(30000, "Expanding file : " + filename, Logger.MessageSeverity.Debug);
                        fs.SetLength(lenght);
                    }
                    return 1;
                }
                else if (fileToResize.Length > lenght)   // The file is too large, shrink it
                {
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.None))
                    {
                        Logger.Write(30000, "Shrinking file : " + filename, Logger.MessageSeverity.Debug);
                        fs.SetLength(lenght);
                    }
                    return -1;
                }
            }
            return 0;
        }

        private static DirectorySecurity GetDirectorySecurity(string fullControlMembers, string readWriteMembers)
        {
            System.Security.AccessControl.DirectorySecurity dirSecturity = new System.Security.AccessControl.DirectorySecurity();
            dirSecturity.SetAccessRuleProtection(true, false);  // Break inheritance and discard parent's access rules

            AddReadWriteAccess(dirSecturity, Environment.UserName); // Set Read/Write access for the current user

            System.Security.Principal.SecurityIdentifier systemSID = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.LocalSystemSid, null);
            AddAccessRule(dirSecturity, GetFullControlAccessRule(systemSID));   // Set Full Access for "System" Account

            if (!String.IsNullOrEmpty(readWriteMembers))
            {
                try
                {
                    foreach (string trustee in readWriteMembers.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            AddReadWriteAccess(dirSecturity, trustee);
                        }
                        catch (Exception ex) { Logger.Write(20015, "Unable to set Read/Write permissions on destination folder. " + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
                    }
                }
                catch (Exception ex) { Logger.Write(20015, "Unable to set Read/Write permissions on destination folder. " + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
            }

            if (!String.IsNullOrEmpty(fullControlMembers))
            {
                try
                {
                    foreach (string trustee in fullControlMembers.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            AddFullControlAccess(dirSecturity, trustee);
                        }
                        catch (Exception ex) { Logger.Write(20016, "Unable to set Full Control permissions on destination folder. " + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
                    }
                }
                catch (Exception ex) { Logger.Write(20016, "Unable to set Full Control permissions on destination folder. " + ex.Message, Logger.MessageSeverity.Error, System.Diagnostics.EventLogEntryType.Error); }
            }
            return dirSecturity;
        }

        private static void AddReadWriteAccess(DirectorySecurity folderACL, string accountToAdd)
        {
            AddAccessRule(folderACL, GetReadWriteAccessRule(GetSID(accountToAdd)));
        }

        private static void AddFullControlAccess(DirectorySecurity folderACL, string accountToAdd)
        {
            AddAccessRule(folderACL, GetFullControlAccessRule(GetSID(accountToAdd)));
        }

        private static FileSystemAccessRule GetReadWriteAccessRule(SecurityIdentifier SID)
        {
            return new FileSystemAccessRule(SID,
                    FileSystemRights.Traverse |
                    FileSystemRights.ListDirectory |
                    FileSystemRights.ReadAttributes |
                    FileSystemRights.ReadExtendedAttributes |
                    FileSystemRights.CreateDirectories |
                    FileSystemRights.CreateFiles |
                    FileSystemRights.WriteAttributes |
                    FileSystemRights.WriteExtendedAttributes |
                    FileSystemRights.Delete |
                    FileSystemRights.ReadPermissions,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);
        }

        private static FileSystemAccessRule GetFullControlAccessRule(SecurityIdentifier SID)
        {
            return new FileSystemAccessRule(SID,
                FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);
        }

        private static void AddAccessRule(DirectorySecurity folder, FileSystemAccessRule rule)
        {
            folder.AddAccessRule(rule);
        }

        private static SecurityIdentifier GetSID(string accountName)
        {
            try
            {
                return TranslateAccountNameIntoSID(null, accountName);
            }
            catch (Exception) { }

            string systemName = null;

            if (accountName.Contains("\\"))
            {
                int index = accountName.IndexOf('\\');
                systemName = accountName.Substring(0, index);
                accountName = accountName.Substring(index + 1);
            }
            else if (accountName.Contains("@"))
            {
                int index = accountName.IndexOf('@');
                systemName = accountName.Substring(index + 1);
                accountName = accountName.Substring(0, index);
            }

            return TranslateAccountNameIntoSID(systemName, accountName);
        }

        private static SecurityIdentifier TranslateAccountNameIntoSID(string systemName, string accountName)
        {
            if (accountName == null)
                throw new ArgumentNullException();

            string sidString = String.Empty;
            byte[] Sid = null;
            uint cbSid = 0;
            StringBuilder referencedDomainName = new StringBuilder();
            uint cchReferencedDomainName = (uint)referencedDomainName.Capacity;
            SID_NAME_USE sidUse;

            int err = NO_ERROR;
            if (!LookupAccountName(systemName, accountName, Sid, ref cbSid, referencedDomainName, ref cchReferencedDomainName, out sidUse))
            {
                err = Marshal.GetLastWin32Error();
                if (err == ERROR_INSUFFICIENT_BUFFER || err == ERROR_INVALID_FLAGS)
                {
                    Sid = new byte[cbSid];
                    referencedDomainName.EnsureCapacity((int)cchReferencedDomainName);
                    err = NO_ERROR;
                    if (!LookupAccountName(systemName, accountName, Sid, ref cbSid, referencedDomainName, ref cchReferencedDomainName, out sidUse))
                        err = Marshal.GetLastWin32Error();
                }
            }
            else
            {
                throw new Exception("Unable to translate " + accountName + " into a SID.");
            }
            if (err == 0)
            {
                IntPtr ptrSid;
                if (!ConvertSidToStringSid(Sid, out ptrSid))
                {
                    err = Marshal.GetLastWin32Error();
                    throw new Exception("Could not convert sid to string. Error : " + err.ToString());
                }
                else
                {
                    sidString = Marshal.PtrToStringAuto(ptrSid);
                    LocalFree(ptrSid);
                }
            }
            else
                throw new Exception("Error : " + err.ToString());

            return new SecurityIdentifier(sidString);
        }

        /// <summary>
        /// Rename the first file into the second file
        /// </summary>
        /// <param name="oldFile">Full path to the file to rename</param>
        /// <param name="newFile">Full path to the file once renamed</param>
        public static void RenameFile(string oldFile, string newFile)
        {
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            File.Move(oldFile, newFile);
        }

        /// <summary>
        /// Returns the full path to the provided file without the extension ".partial" at the end
        /// </summary>
        /// <param name="oldName">Full path of a file that ends with ".partial"</param>
        /// <returns>Returns the full path of the same file without the ".partial" extension</returns>
        public static string GetNewName(string oldName)
        {
            FileInfo fileToRename = new FileInfo(oldName);
            FileInfo newName = new FileInfo(Path.Combine(fileToRename.DirectoryName, fileToRename.Name.Substring(0, fileToRename.Name.Length - ".partial".Length)));

            return newName.FullName;
        }

        /// <summary>
        /// Gets a FileStream link to the specified file. If the file is locked, try 5 times at 1 second interval
        /// </summary>
        /// <param name="filename">Full path to the file</param>
        /// <returns>Returns a FileStream link to the file or null if all tries to open the filestream have failed</returns>
        public static FileStream GetOutlookFile(string filename)
        {
            FileStream fs = null;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    break;
                }
                catch (Exception)
                {
                    Logger.Write(10006, filename + " lock by another process. Waiting 1 second before retrying.", Logger.MessageSeverity.Warning, System.Diagnostics.EventLogEntryType.Warning);
                    System.Threading.Thread.Sleep(1000);
                }
            }

            return fs;
        }

        /// <summary>
        /// Create a new temporary folder under %LocalAppData%\Pst Backup\Temp and returns the full path to that folder
        /// </summary>
        /// <returns>Returns the full path to the folder</returns>
        public static string GetTemporaryFolder()
        {
            DirectoryInfo newFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pst Backup\\Compression", Path.GetRandomFileName()));
            
            if(!newFolder.Exists)
            {
                newFolder.Create();
            }

            return newFolder.FullName;
        }
    }
}
