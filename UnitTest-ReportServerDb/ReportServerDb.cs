using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSingularity.PstBackupReportServer;
using SUT = SmartSingularity.PstBackupReportServerDb.PstBackupReportServerDb;

namespace UnitTest_ReportServerDb
{
    public class ReportServerDb
    {
        [TestClass]
        public class RegisterClient_Should
        {
            private static string _dbPath = @"E:\Pst Backup\Test Files\PstBackup.mdf";
            private static SqlConnection _dbConnection = null;
            private static SqlCommand _sqlCommand = null;

            [ClassInitialize]
            public static void ClassInitialize(TestContext context)
            {
                _dbConnection = new SqlConnection($"Server=(localdb)\v11.0;Integrated Security=true;AttachDbFileName = {_dbPath};");
                _sqlCommand = new SqlCommand(String.Empty, _dbConnection);
                _dbConnection.Open();

                _sqlCommand.CommandText = "DELETE * FROM tbClients;";
                _sqlCommand.ExecuteNonQuery();
            }

            [ClassCleanup]
            public static void ClassCleanUp(TestContext context)
            {
                try
                {
                    _dbConnection.Close();
                }
                catch (Exception)                {                }
            }

            [TestMethod]
            public void RegisterTheClient_WhenClientDoNotExists()
            {
                // Arrange
                SUT sut = new SUT(_dbPath);
                sut.Connect();
                Client clientToRegister = new Client()
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = new Version("1.0.1705.26"),
                    Username = "Courtel",
                    ComputerName = "Céos",
                    LastContactDate = DateTime.Now.ToUniversalTime()
                };

                // Act
                Assert.IsFalse(sut.IsClientExists(clientToRegister));
                sut.RegisterClient(clientToRegister);

                // Assert
                Assert.IsTrue(sut.IsClientExists(clientToRegister));
            }
        }
    }
}
