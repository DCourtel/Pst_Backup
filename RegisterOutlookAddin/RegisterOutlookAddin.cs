using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;

namespace RegisterOutlookAddin
{
    public class CustomActions
    {
        public enum OsBitness
        {
            OS32,
            OS64
        }

        public enum OfficeRelease
        {
            Office2010,
            Office2013,
            Office2016,
            Unknown
        }

        public enum OfficeBitness
        {
            Office32,
            Office64,
            Unknown
        }

        [CustomAction]
        public static ActionResult RegisterOutlookAddin(Session session)
        {
            session.Log("Begin RegisterOutlookAddin");

            try
            {
                OsBitness osBitness = GetOsBitness();
                session.Log("OS bitness is : " + osBitness.ToString());
                OfficeBitness officeBitness = GetOfficeBitness(osBitness);
                session.Log("Office bitness is : " + officeBitness.ToString());
                OfficeRelease officeRelease = GetOfficeRelease(osBitness);
                session.Log("Office release is : " + officeRelease.ToString());

                session.Log("Registering Outlook Addin…");
                RegisterAddin(osBitness, officeBitness);
                session.Log("Whitelisting Addin…");
                WhitelistAddin(osBitness, officeBitness, officeRelease);
            }
            catch (Exception ex)
            {
                session.Log("An error occurs while registering Outlook Addin in the Registry : " + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UnRegisterOutlookAddin(Session session)
        {
            session.Log("Begin UnRegisterOutlookAddin");

            try
            {
                OsBitness osBitness = GetOsBitness();
                session.Log("OS bitness is : " + osBitness.ToString());
                OfficeBitness officeBitness = GetOfficeBitness(osBitness);
                session.Log("Office bitness is : " + officeBitness.ToString());
                OfficeRelease officeRelease = GetOfficeRelease(osBitness);
                session.Log("Office release is : " + officeRelease.ToString());

                session.Log("Unregistering Outlook Addin…");
                UnRegisterAddin(osBitness, officeBitness);
                session.Log("Unwhitelisting Addin…");
                UnWhitelistAddin(osBitness, officeBitness, officeRelease);

            }
            catch (Exception ex)
            {
                session.Log("An error occurs while unregistering Outlook Addin : " + ex.Message);
            }

            return ActionResult.Success;
        }

        private static void RegisterAddin(OsBitness osBitness, OfficeBitness officeBitness)
        {
            string manifestLocation = osBitness == OsBitness.OS64 ? @"file:///C:/Program Files (x86)/Pst Backup/SmartSingularity.PstBackupAddin.vsto|vstolocal" : @"file:///C:/Program Files/Pst Backup/SmartSingularity.PstBackupAddin.vsto|vstolocal";
            RegistryKey HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (officeBitness == OfficeBitness.Office32 ? RegistryView.Registry32 : RegistryView.Registry64));
            RegistryKey officeKey = HKLM.CreateSubKey(@"SOFTWARE\Microsoft\Office\Outlook\Addins", true);
            RegistryKey addinKey = officeKey.CreateSubKey("SmartSingularity.PstBackupAddin", true);

            addinKey.SetValue("Description", (object)"Automatically backup your PST files at closing.", RegistryValueKind.String);
            addinKey.SetValue("FriendlyName", (object)"Pst Backup", RegistryValueKind.String);
            addinKey.SetValue("LoadBehavior", (object)3, RegistryValueKind.DWord);
            addinKey.SetValue("Manifest", (object)manifestLocation, RegistryValueKind.String);

            addinKey.Close();
            officeKey.Close();
            HKLM.Close();
        }

        private static void UnRegisterAddin(OsBitness osBitness, OfficeBitness officeBitness)
        {
            try
            {
                RegistryKey HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (officeBitness == OfficeBitness.Office32 ? RegistryView.Registry32 : RegistryView.Registry64));
                RegistryKey officeKey = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\Outlook\Addins", true);

                if (officeKey != null)
                {
                    officeKey.DeleteSubKey("SmartSingularity.PstBackupAddin", true);
                    officeKey.Close();
                }

                HKLM.Close();
            }
            catch (Exception) { }
        }

        private static void WhitelistAddin(OsBitness osBitness, OfficeBitness officeBitness, OfficeRelease officeRelease)
        {
            RegistryKey HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (officeBitness == OfficeBitness.Office32 ? RegistryView.Registry32 : RegistryView.Registry64));
            RegistryKey officeKey = null;

            switch (officeRelease)
            {
                case OfficeRelease.Office2010:
                    officeKey = HKLM.CreateSubKey(@"SOFTWARE\Microsoft\Office\14.0\Outlook\Resiliency\DoNotDisableAddinList", true);
                    break;
                case OfficeRelease.Office2013:
                    officeKey = HKLM.CreateSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\Resiliency\DoNotDisableAddinList", true);
                    break;
                case OfficeRelease.Office2016:
                    officeKey = HKLM.CreateSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\Resiliency\DoNotDisableAddinList", true);
                    break;
            }
            officeKey.SetValue("SmartSingularity.PstBackupAddin", (object)1, RegistryValueKind.DWord);

            officeKey.Close();
            HKLM.Close();
        }

        private static void UnWhitelistAddin(OsBitness osBitness, OfficeBitness officeBitness, OfficeRelease officeRelease)
        {
            try
            {
                RegistryKey HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (officeBitness == OfficeBitness.Office32 ? RegistryView.Registry32 : RegistryView.Registry64));
                RegistryKey officeKey = null;

                switch (officeRelease)
                {
                    case OfficeRelease.Office2013:
                        officeKey = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\Resiliency\DoNotDisableAddinList", true);
                        break;
                    case OfficeRelease.Office2016:
                        officeKey = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\Resiliency\DoNotDisableAddinList", true);
                        break;
                }
                if (officeKey != null)
                {
                    officeKey.DeleteValue("SmartSingularity.PstBackupAddin", false);
                    officeKey.Close();
                }
                HKLM.Close();
            }
            catch (Exception) { }
        }

        private static OsBitness GetOsBitness()
        {
            return Environment.Is64BitOperatingSystem ? OsBitness.OS64 : OsBitness.OS32;
        }

        private static OfficeBitness GetOfficeBitness(OsBitness osBitness)
        {
            RegistryKey HKLM = Registry.LocalMachine;
            RegistryKey officeKey14 = null;
            RegistryKey officeKey15 = null;
            RegistryKey officeKey16 = null;

            switch (osBitness)
            {
                case OsBitness.OS32:
                    return OfficeBitness.Office32;
                case OsBitness.OS64:
                    // Search Office 32Bit
                    HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    officeKey14 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Outlook\InstallRoot", false);
                    officeKey15 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\InstallRoot", false);
                    officeKey16 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\InstallRoot", false);
                    if (officeKey14 != null || officeKey15 != null || officeKey16 != null)
                        return OfficeBitness.Office32;

                    // Search Office 64Bit
                    HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    officeKey14 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Outlook\InstallRoot", false);
                    officeKey15 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\InstallRoot", false);
                    officeKey16 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\InstallRoot", false);
                    if (officeKey14 != null || officeKey15 != null || officeKey16 != null)
                        return OfficeBitness.Office64;
                    break;
            }
            throw new Exception("Unable to determine Office bitness");
        }

        private static OfficeRelease GetOfficeRelease(OsBitness osBitness)
        {
            RegistryKey HKLM = Registry.LocalMachine;
            RegistryKey officeKey14 = null;
            RegistryKey officeKey15 = null;
            RegistryKey officeKey16 = null;

            switch (osBitness)
            {
                case OsBitness.OS32:
                    HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    officeKey14 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Outlook\InstallRoot", false);
                    officeKey15 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\InstallRoot", false);
                    officeKey16 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\InstallRoot", false);
                    if (officeKey14 != null)
                        return OfficeRelease.Office2010;
                    if (officeKey15 != null)
                        return OfficeRelease.Office2013;
                    if (officeKey16 != null)
                        return OfficeRelease.Office2016;
                    break;
                case OsBitness.OS64:
                    HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    officeKey14 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Outlook\InstallRoot", false);
                    officeKey15 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\InstallRoot", false);
                    officeKey16 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\InstallRoot", false);
                    if (officeKey14 != null)
                        return OfficeRelease.Office2010;
                    if (officeKey15 != null)
                        return OfficeRelease.Office2013;
                    if (officeKey16 != null)
                        return OfficeRelease.Office2016;
                    HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    officeKey14 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Outlook\InstallRoot", false);
                    officeKey15 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\15.0\Outlook\InstallRoot", false);
                    officeKey16 = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Outlook\InstallRoot", false);
                    if (officeKey14 != null)
                        return OfficeRelease.Office2010;
                    if (officeKey15 != null)
                        return OfficeRelease.Office2013;
                    if (officeKey16 != null)
                        return OfficeRelease.Office2016;
                    break;
            }
            throw new Exception("Unable to determine Office release");
        }
    }
}
