using System;
using System.IO;
using System.Security.AccessControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SUT = SmartSingularity.PstBackupFileSystem.FileSystem;

namespace UnitTest_PstBackupFileSystem
{
    [TestClass]
    public class FileSystem
    {
        [TestClass]
        public class CreateDestinationFolder_Should
        {
            [TestMethod]
            public void CreateTheFolder_WhenParentFolderExists()
            {
                // Arrange
                DirectoryInfo folderToCreate = new DirectoryInfo( @"E:\Pst Backup\Test Files\FolderToCreate");

                // Act
                if(folderToCreate.Exists)
                {
                    folderToCreate.Delete(true);
                    folderToCreate.Refresh();
                    Assert.IsFalse(folderToCreate.Exists);
                }
                SUT.CreateDestinationFolder(folderToCreate.FullName, false, String.Empty, String.Empty);
                folderToCreate.Refresh();

                // Assert
                Assert.IsTrue(folderToCreate.Exists);
            }

            [TestMethod]
            public void CreateTheFolder_WhenParentFolderDoesNotExists()
            {
                // Arrange
                DirectoryInfo folderToCreate = new DirectoryInfo(@"E:\Pst Backup\Test Files\ParentFolder\FolderToCreate");
                DirectoryInfo parentFolder = new DirectoryInfo(@"E:\Pst Backup\Test Files\ParentFolder");

                // Act
                if (parentFolder.Exists)
                {
                    parentFolder.Delete(true);
                    parentFolder.Refresh();
                    Assert.IsFalse(parentFolder.Exists);
                }
                SUT.CreateDestinationFolder(folderToCreate.FullName, false, String.Empty, String.Empty);
                folderToCreate.Refresh();

                // Assert
                Assert.IsTrue(folderToCreate.Exists);
            }

            [TestMethod]
            public void DoNotChangeNTFSPermissions_WhenSetExclusivePermissionsIsNotAsk()
            {
                // Arrange
                DirectoryInfo folderToCreate = new DirectoryInfo(@"E:\Pst Backup\Test Files\FolderToCreate");
                DirectorySecurity parentSecurity;
                DirectorySecurity folderSecurity;                

                // Act
                if (folderToCreate.Exists)
                {
                    folderToCreate.Delete(true);
                    folderToCreate.Refresh();
                    Assert.IsFalse(folderToCreate.Exists);
                }
                SUT.CreateDestinationFolder(folderToCreate.FullName, false, String.Empty, String.Empty);
                parentSecurity = new DirectorySecurity(folderToCreate.Parent.FullName, AccessControlSections.All);
                folderSecurity = new DirectorySecurity(folderToCreate.FullName, AccessControlSections.All);
                AuthorizationRuleCollection childSecCollection = folderSecurity.GetAccessRules(true, true, typeof( System.Security.Principal.SecurityIdentifier));
                AuthorizationRuleCollection parentSecCollection = parentSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

                // Assert
                Assert.IsFalse(folderSecurity.AreAccessRulesProtected);
                Assert.AreEqual(parentSecCollection.Count, childSecCollection.Count);
                foreach (AuthorizationRule rule in childSecCollection)
                {
                    Assert.IsTrue(rule.IsInherited);                    
                }
            }

            [TestMethod]
            public void CreateTheFolderWithPersonnalizedNtfsPermissions_WhenSetExclusivePermissionsIsAsk()
            {
                // Arrange
                DirectoryInfo folderToCreate = new DirectoryInfo(@"E:\Pst Backup\Test Files\FolderToCreate");
                DirectorySecurity folderSecurity;
                string securityDescriptor;

                // Act
                if (folderToCreate.Exists)
                {
                    folderToCreate.Delete(true);
                    folderToCreate.Refresh();
                    Assert.IsFalse(folderToCreate.Exists);
                }
                SUT.CreateDestinationFolder(folderToCreate.FullName, true, @"MCSA\SConnor" , @"MCSA\SVaughan");
                folderSecurity = new DirectorySecurity(folderToCreate.FullName, AccessControlSections.All);
                securityDescriptor = folderSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);

                // Assert
                Assert.IsTrue(folderSecurity.AreAccessRulesProtected);
                Assert.IsTrue(securityDescriptor.Contains("(A;OICI;FA;;;SY)"));
                Assert.IsTrue(securityDescriptor.Contains("(A;OICI;0x1301bf;;;S-1-5-21-2569095476-1252395535-2594552870-1105)"));
                Assert.IsTrue(securityDescriptor.Contains("(A;OICI;FA;;;S-1-5-21-2569095476-1252395535-2594552870-1608)"));
                Assert.IsTrue(securityDescriptor.Contains("(A;OICI;0x1301bf;;;S-1-5-21-2569095476-1252395535-2594552870-1609)"));
            }
        }

        [TestClass]
        public class ExpandeDestinationFolder_Should
        {
            [TestMethod]
            public void ReturnsAnExpandedPath_WhenPathContainsUserLoginVariable()
            {
                // Arrange
                string collapsedPath = @"\\192.168.0.250\Share\Transit\PstFiles\%UserLogin%";
                string expectedPath = @"\\192.168.0.250\Share\Transit\PstFiles\courtel";
                string actualPath = String.Empty;

                // Act
                actualPath = SUT.ExpandDestinationFolder(collapsedPath);

                // Assert
                Assert.AreEqual(expectedPath, actualPath, true);
            }

            [TestMethod]
            public void ReturnsAnExpandedPath_WhenPathContainsComputerNameVariable()
            {
                // Arrange
                string collapsedPath = @"\\192.168.0.250\Share\Transit\PstFiles\%ComputerName%";
                string expectedPath = @"\\192.168.0.250\Share\Transit\PstFiles\DEVPSTBACKUP201";
                string actualPath = String.Empty;

                // Act
                actualPath = SUT.ExpandDestinationFolder(collapsedPath);

                // Assert
                Assert.AreEqual(expectedPath, actualPath, true);
            }

            [TestMethod]
            public void ReturnsAnExpandedPath_WhenPathContainsComputerNameAndUserNameVariables()
            {
                // Arrange
                string collapsedPath = @"\\192.168.0.250\Share\Transit\PstFiles\%ComputerName%\%UserLogin%";
                string expectedPath = @"\\192.168.0.250\Share\Transit\PstFiles\DEVPSTBACKUP201\Courtel";
                string actualPath = String.Empty;

                // Act
                actualPath = SUT.ExpandDestinationFolder(collapsedPath);

                // Assert
                Assert.AreEqual(expectedPath, actualPath, true);
            }

            [TestMethod]
            public void ReturnsAnExpandedPath_WhenPathContainsComputerNameVariableInUpperCase()
            {
                // Arrange
                string collapsedPath = @"\\192.168.0.250\Share\Transit\PstFiles\%COMPUTERNAME%";
                string expectedPath = @"\\192.168.0.250\Share\Transit\PstFiles\DEVPSTBACKUP201";
                string actualPath = String.Empty;

                // Act
                actualPath = SUT.ExpandDestinationFolder(collapsedPath);

                // Assert
                Assert.AreEqual(expectedPath, actualPath, true);
            }

            [TestMethod]
            public void ReturnsAnExpandedPath_WhenPathContainsUserLoginVariableInUpperCase()
            {
                // Arrange
                string collapsedPath = @"\\192.168.0.250\Share\Transit\PstFiles\%USERLOGIN%";
                string expectedPath = @"\\192.168.0.250\Share\Transit\PstFiles\Courtel";
                string actualPath = String.Empty;

                // Act
                actualPath = SUT.ExpandDestinationFolder(collapsedPath);

                // Assert
                Assert.AreEqual(expectedPath, actualPath, true);
            }

            [TestMethod]
            public void ReturnsTheSamePath_WhenPathDoesNotContainsVariable()
            {
                // Arrange
                string collapsedPath = @"\\192.168.0.250\Share\Transit\PstFiles\Courtel";
                string expectedPath = @"\\192.168.0.250\Share\Transit\PstFiles\Courtel";
                string actualPath = String.Empty;

                // Act
                actualPath = SUT.ExpandDestinationFolder(collapsedPath);

                // Assert
                Assert.AreEqual(expectedPath, actualPath, true);
            }
        }

        [TestClass]
        public class ResizeFile_Should
        {
            [TestMethod]
            public void CreateTheFile_WhenItDoesNotExists()
            {
                // Arrange
                System.IO.FileInfo fileToResize = new System.IO.FileInfo(@"E:\Pst Backup\Test Files\FileToResize.pst");

                // Act
                if (fileToResize.Exists)
                {
                    fileToResize.Delete();
                    fileToResize.Refresh();
                    Assert.IsFalse(fileToResize.Exists);
                }
                SUT.ResizeFile(fileToResize.FullName, 4096L);
                fileToResize.Refresh();

                // Assert
                Assert.IsTrue(fileToResize.Exists);
                Assert.AreEqual(4096L, fileToResize.Length);
            }

            [TestMethod]
            public void ExpandTheFile_WhenItIsTooSmall()
            {
                // Arrange
                System.IO.FileInfo fileToResize = new System.IO.FileInfo(@"E:\Pst Backup\Test Files\FileToResize.pst");

                // Act
                if (fileToResize.Exists)
                {
                    fileToResize.Delete();
                    fileToResize.Refresh();
                    Assert.IsFalse(fileToResize.Exists);
                }
                SUT.ResizeFile(fileToResize.FullName, 4096L);
                fileToResize.Refresh();
                Assert.AreEqual(4096L, fileToResize.Length);
                SUT.ResizeFile(fileToResize.FullName, 20000L);
                fileToResize.Refresh();

                // Assert
                Assert.AreEqual(20000L, fileToResize.Length);
            }

            [TestMethod]
            public void ShrinkTheFile_WhenItIsTooLarge()
            {
                // Arrange
                System.IO.FileInfo fileToResize = new System.IO.FileInfo(@"E:\Pst Backup\Test Files\FileToResize.pst");

                // Act
                if (fileToResize.Exists)
                {
                    fileToResize.Delete();
                    fileToResize.Refresh();
                    Assert.IsFalse(fileToResize.Exists);
                }
                SUT.ResizeFile(fileToResize.FullName, 20000L);
                fileToResize.Refresh();
                Assert.AreEqual(20000L, fileToResize.Length);
                SUT.ResizeFile(fileToResize.FullName, 4096L);
                fileToResize.Refresh();

                // Assert
                Assert.AreEqual(4096L, fileToResize.Length);
            }
        }

        [TestClass]
        public class RenameFile_Should
        {
            [TestMethod]
            public void RenameTheFile_WhenTheFirstFileExists()
            {
                // Arrange
                FileInfo firstFile = new FileInfo(@"E:\Pst Backup\Test Files\FileToRename.pst.partial");
                FileInfo secondFile = new FileInfo(@"E:\Pst Backup\Test Files\FileToRename.pst");

                // Act
                if (!firstFile.Exists)
                {
                    firstFile.CreateText().Close();
                    firstFile.Refresh();
                    Assert.IsTrue(firstFile.Exists);
                }
                if (secondFile.Exists)
                {
                    secondFile.Delete();
                    secondFile.Refresh();
                    Assert.IsFalse(secondFile.Exists);
                }
                SUT.RenameFile(firstFile.FullName, secondFile.FullName);
                firstFile.Refresh();
                secondFile.Refresh();

                // Assert
                Assert.IsTrue(secondFile.Exists);
                Assert.IsFalse(firstFile.Exists);
            }
        }

        [TestClass]
        public class GetNewName_Should
        {
            [TestMethod]
            public void ReturnTheSameFullPathOfTheFile_WhenTheFilenameEndsWithDotPartial()
            {
                // Arrange
                string partialFile = @"E:\Pst Backup\Test Files\FileToRename.pst.partial";
                string pstFile = @"E:\Pst Backup\Test Files\FileToRename.pst";

                // Act
                string newName = SUT.GetNewName(partialFile);

                // Assert
                Assert.AreEqual(pstFile, newName);
            }
        }

        [TestClass]
        public class GetTemporaryFolder_Should
        {
            [TestMethod]
            public void CreateTheFolder_WhenItDoesNotExists()
            {
                // Arrange
                DirectoryInfo parentFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pst Backup"));
                if(parentFolder.Exists)
                {
                    parentFolder.Delete(true);
                    parentFolder.Refresh();
                    Assert.IsFalse(parentFolder.Exists);
                }
                DirectoryInfo tempFolder;

                // Act
                tempFolder = new DirectoryInfo(SUT.GetTemporaryFolder());

                // Assert
                Assert.IsTrue(tempFolder.Exists);
            }

        }
    }
}
