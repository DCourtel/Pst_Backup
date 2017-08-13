using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SmartSingularity.PstBackupReportServer
{
    public class ReportServerDb
    {
        private string _dbPath = String.Empty;
        private SqlConnection _dbConnection = null;

        public ReportServerDb(string dbPath)
        {
            this._dbPath = dbPath;
            if (!System.IO.File.Exists(dbPath))
            {
                DropDatabase();
                CreateDatabase(dbPath);
                CreateTables(dbPath);
            }
            _dbConnection = new SqlConnection($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PstBackup;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;AttachDbFileName = {_dbPath};");
        }

        private void DropDatabase()
        {
            using (SqlConnection db = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"))
            {
                db.Open();
                SqlCommand cmd = db.CreateCommand();
                cmd.CommandText = $"DROP DATABASE PstBackup";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception) { }
                db.Close();
            }
        }

        private void CreateDatabase(string dbPath)
        {
            System.IO.FileInfo dbFile = new System.IO.FileInfo(dbPath);
            if (!dbFile.Directory.Exists)
            {
                dbFile.Directory.Create();
            }
            using (SqlConnection db = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"))
            {
                db.Open();
                SqlCommand cmd = db.CreateCommand();
                cmd.CommandText = $"CREATE DATABASE PstBackup ON (NAME = 'PstBackup', FILENAME = '{dbPath}')";
                cmd.ExecuteNonQuery();
                db.Close();
            }
        }

        private void CreateTables(string dbPath)
        {
            using (SqlConnection db = new SqlConnection($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PstBackup;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;AttachDbFileName = {_dbPath};"))
            {
                db.Open();
                SqlCommand cmd = db.CreateCommand();

                // Table tbClients
                cmd.CommandText = "CREATE TABLE [dbo].[tbClients](" + 
                                    "[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, " +
                                    "[Version] VARCHAR(15) NOT NULL, " +
                                    "[ComputerName] NVARCHAR(64) NOT NULL, " +
                                    "[UserName] NVARCHAR(32) NOT NULL, " +
                                    "[LastContactDate] DATETIME NOT NULL);";
                cmd.ExecuteNonQuery();

                // Table tbPstFiles
                cmd.CommandText = "CREATE TABLE [dbo].[tbPstFiles](" + 
                                    "[ClientId] UNIQUEIDENTIFIER NOT NULL, " +
                                    "[LocalPath] NVARCHAR(300) NOT NULL, " +
                                    "[FileId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, " +
                                    "[IsSetToBackup] BIT NOT NULL, " +
                                    "[Size] BIGINT NOT NULL, " +
                                    "[LastSuccessfulBackup] DATETIME NULL);";
                cmd.ExecuteNonQuery();

                // Table tbBackupSessions
                cmd.CommandText = "CREATE TABLE [dbo].[tbBackupSessions](" + 
                                    "[FileId] UNIQUEIDENTIFIER NOT NULL, " +
                                    "[RemotePath] NVARCHAR(300) NOT NULL, " +
                                    "[CompressedSize]  BIGINT NOT NULL, " +
                                    "[IsCompressed] BIT NOT NULL, " +
                                    "[BackupMethod] INT NOT NULL, " +
                                    "[IsSchedule] BIT NOT NULL, " +
                                    "[StartTime] DATETIME NOT NULL, " +
                                    "[EndTime] DATETIME NOT NULL, " +
                                    "[ChunkCount] INT NOT NULL, " +
                                    "[ErrorCode] INT NOT NULL, " +
                                    "[ErrorMessage] NVARCHAR(300) NOT NULL," + 
                                    "PRIMARY KEY([FileId], [StartTime]));";
                cmd.ExecuteNonQuery();
                db.Close();
            }
        }

        private void AddParameter(SqlCommand command, string parameterName, System.Data.SqlDbType parameterType, object value)
        {
            command.Parameters.Add(parameterName, parameterType);
            command.Parameters[parameterName].Value = value;
        }

        /// <summary>
        /// Gets the state of the connection
        /// </summary>
        public System.Data.ConnectionState ConnectionState
        {
            get { return _dbConnection.State; }
        }

        /// <summary>
        /// Open a connection to the database
        /// </summary>
        public void Connect()
        {
            if (ConnectionState == System.Data.ConnectionState.Open)
                Disconnect();
            _dbConnection.Open();
        }

        /// <summary>
        /// Close the connection to the database
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_dbConnection != null && _dbConnection.State != System.Data.ConnectionState.Closed)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                }
            }
            catch (Exception) { }
        }

        #region PstBackup Client

        /// <summary>
        /// Insert the client into the database if the client do not already exists, otherwise, update informations
        /// </summary>
        /// <param name="client">Client to insert into the database</param>
        public void RegisterClient(Client client)
        {
            if (IsClientExists(client))
                UpdateClientInfo(client);
            else
                RegisterNewClient(client);
        }

        /// <summary>
        /// Delete the client from the database.
        /// </summary>
        /// <param name="client">Client to delete.</param>
        public void DeleteClient(Client client)
        {
            var _sqlCommand = new SqlCommand("DELETE FROM tbClients WHERE Id LIKE @clientId;", _dbConnection);
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(client.Id));

            _sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks whether or not the client is registered in the database.
        /// </summary>
        /// <param name="client">Client to search.</param>
        /// <returns>Returns true if the client have been found in the database.</returns>
        public bool IsClientExists(Client client)
        {
            bool result = false;

            using (var _sqlCommand = new SqlCommand("SELECT * FROM tbClients WHERE Id LIKE @clientId;", _dbConnection))
            {
                AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(client.Id));
                using (var reader = _sqlCommand.ExecuteReader())
                {
                    result = reader.HasRows;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves a client in the database from its ClientID.
        /// </summary>
        /// <param name="clientId">Id of the client to retrieve.</param>
        /// <returns>Returns an instance of the client.</returns>
        public Client GetClient(string clientId)
        {
            var _sqlCommand = new SqlCommand("SELECT * FROM tbClients WHERE Id LIKE @clientId;", _dbConnection);
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, clientId);
            Client client = new Client();

            using (SqlDataReader reader = _sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        client.Id = reader.GetGuid(0).ToString();
                        client.Version = new Version(reader.GetString(1));
                        client.ComputerName = reader.GetString(2);
                        client.Username = reader.GetString(3);
                        client.LastContactDate = reader.GetDateTime(4);
                    }
                }
            }

            return client;
        }

        private void RegisterNewClient(Client client)
        {
            DateTime justNow = DateTime.UtcNow;
            var _sqlCommand = new SqlCommand("INSERT INTO tbClients VALUES (@clientId," +
                "@version," +
                "@computerName," +
                "@userName," +
                "@lastContactDate);", _dbConnection);
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(client.Id));
            AddParameter(_sqlCommand, "@version", System.Data.SqlDbType.NVarChar, client.Version.ToString().Substring(0, System.Math.Min(15, client.Version.ToString().Length)));
            AddParameter(_sqlCommand, "@computerName", System.Data.SqlDbType.NVarChar, client.ComputerName.Substring(0, System.Math.Min(64, client.ComputerName.Length)));
            AddParameter(_sqlCommand, "@userName", System.Data.SqlDbType.NVarChar, client.Username.Substring(0, System.Math.Min(32, client.Username.Length)));
            AddParameter(_sqlCommand, "@lastContactDate", System.Data.SqlDbType.DateTime, justNow);

            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdateClientInfo(Client client)
        {
            DateTime justNow = DateTime.UtcNow;
            var _sqlCommand = new SqlCommand("UPDATE tbClients SET " +
                "Version=@version, " +
                "ComputerName=@computerName, " +
                "UserName=@userName, " +
                $"LastContactDate=@lastContactDate " +
                $"WHERE Id LIKE @clientId;", _dbConnection);
            AddParameter(_sqlCommand, "@version", System.Data.SqlDbType.NVarChar, client.Version.ToString().Substring(0, System.Math.Min(15, client.Version.ToString().Length)));
            AddParameter(_sqlCommand, "@computerName", System.Data.SqlDbType.NVarChar, client.ComputerName.Substring(0, System.Math.Min(64, client.ComputerName.Length)));
            AddParameter(_sqlCommand, @"userName", System.Data.SqlDbType.NVarChar, client.Username.Substring(0, System.Math.Min(32, client.Username.Length)));
            AddParameter(_sqlCommand, @"lastContactDate", System.Data.SqlDbType.DateTime, justNow);
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(client.Id));

            _sqlCommand.ExecuteNonQuery();
        }

        #endregion PstBackup Client

        #region Pst Files

        /// <summary>
        /// Insert the PST file into the database if the PST file do not already exists, otherwise, update informations
        /// </summary>
        /// <param name="clientId">Unique ID of the client that own the PST file.</param>
        /// <param name="pstFile">Informations on the PST file.</param>
        /// <param name="bckSession">Informations on the backup session.</param>
        public void RegisterPstFile(string clientId, PstFile pstFile)
        {
            if (IsPstFileExists(clientId, pstFile.LocalPath))
                UpdatePstFileInfo(clientId, pstFile);
            else
                RegisterNewPstFile(clientId, pstFile);
        }

        /// <summary>
        /// Checks whether or not the PST file is registered in the database.
        /// </summary>
        /// <param name="clientId">Unique ID of the client that own the PST file.</param>
        /// <param name="localPath">Path to the PST file on the client computer.</param>
        /// <returns>Returns true if the PST file have been found in the database.</returns>
        public bool IsPstFileExists(string clientId, string localPath)
        {
            bool result = false;
            using (var _sqlCommand = new SqlCommand("SELECT * FROM tbPstFiles WHERE ClientID LIKE @clientId AND LocalPath LIKE @localPath;", _dbConnection))
            {
                AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(clientId));
                AddParameter(_sqlCommand, "@localPath", System.Data.SqlDbType.NVarChar, localPath.Substring(0, System.Math.Min(300, localPath.Length)));
                using (SqlDataReader reader = _sqlCommand.ExecuteReader())
                {
                    result = reader.HasRows;
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves informations on a PST file from the clientId and the localPath.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public PstFile GetPstFile(string clientId, string localPath)
        {
            PstFile pstFile = new PstFile();

            using (var _sqlCommand = new SqlCommand("SELECT * FROM tbPstFiles WHERE ClientId LIKE @clientId AND LocalPath LIKE @localPath;", _dbConnection))
            {
                AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(clientId));
                AddParameter(_sqlCommand, "@localPath", System.Data.SqlDbType.NVarChar, localPath.Substring(0, System.Math.Min(300, localPath.Length)));
                using (SqlDataReader reader = _sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            pstFile.LocalPath = reader.GetString(1);
                            pstFile.Id = reader.GetGuid(2);
                            pstFile.IsSetToBackup = reader.GetBoolean(3);
                            pstFile.Size = reader.GetInt64(4);
                            object lastSuccessfulBackup = reader[5];
                            pstFile.LastSuccessfulBackup = (lastSuccessfulBackup == System.DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(lastSuccessfulBackup);
                        }
                    }
                }
            }
            return pstFile;
        }

        private void RegisterNewPstFile(string clientId, PstFile pstFile)
        {
            var _sqlCommand = new SqlCommand($"INSERT INTO tbPstFiles VALUES (@clientId,@localPath,'{Guid.NewGuid()}',@isSetToBackup,@size,null);", _dbConnection);
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(clientId));
            AddParameter(_sqlCommand, "@localPath", System.Data.SqlDbType.NVarChar, pstFile.LocalPath.Substring(0, System.Math.Min(300, pstFile.LocalPath.Length)));
            AddParameter(_sqlCommand, "@isSetToBackup", System.Data.SqlDbType.Bit, pstFile.IsSetToBackup);
            AddParameter(_sqlCommand, "@size", System.Data.SqlDbType.BigInt, pstFile.Size);
            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdatePstFileInfo(string clientId, PstFile pstFile)
        {
            var _sqlCommand = new SqlCommand("UPDATE tbPstFiles SET " +
                "IsSetToBackup=@isSetToBackup," +
                "Size=@size," +
                "LastSuccessfulBackup=@lastSuccessfulBackup " +
                "WHERE ClientId LIKE @clientId AND LocalPath LIKE @localPath;", _dbConnection);
            AddParameter(_sqlCommand, "@isSetToBackup", System.Data.SqlDbType.Bit, pstFile.IsSetToBackup);
            AddParameter(_sqlCommand, "@size", System.Data.SqlDbType.BigInt, pstFile.Size);
            AddParameter(_sqlCommand, "@lastSuccessfulBackup", System.Data.SqlDbType.DateTime, ((object)pstFile.LastSuccessfulBackup ?? DBNull.Value));
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(clientId));
            AddParameter(_sqlCommand, "@localPath", System.Data.SqlDbType.NVarChar, pstFile.LocalPath.Substring(0, System.Math.Min(300, pstFile.LocalPath.Length)));
            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdateBackupSuccessDate(string clientId, string fileId, DateTime backupDate)
        {
            var _sqlCommand = new SqlCommand($"UPDATE tbPstFiles SET " +
                $"LastSuccessfulBackup=@backupDate " +
                $"WHERE ClientId LIKE @clientId AND fileId LIKE @fileId;", _dbConnection);
            AddParameter(_sqlCommand, "@backupDate", System.Data.SqlDbType.DateTime, backupDate);
            AddParameter(_sqlCommand, "@clientId", System.Data.SqlDbType.UniqueIdentifier, new Guid(clientId));
            AddParameter(_sqlCommand, "@fileId", System.Data.SqlDbType.UniqueIdentifier, new Guid(fileId));
            _sqlCommand.ExecuteNonQuery();
        }

        #endregion Pst Files

        #region BackupSession

        /// <summary>
        /// Log the result of the backup session in the database.
        /// </summary>
        /// <param name="clientId">Unique Id of the client that have perform the backup.</param>
        /// <param name="bckSession">Informations on the backup.</param>
        public void LogBackupResult(string clientId, BackupSession bckSession)
        {
            string fileId = GetPstFile(clientId, bckSession.LocalPath).Id.ToString();
            var _sqlCommand = new SqlCommand($"INSERT INTO tbBackupSessions VALUES(" +
                $"@fileId," +
                $"@remotePath," +
                $"@compressedSize," +
                $"@isCompressed," +
                $"@backupMethod," +
                $"@isSchedule," +
                $"@startTime," +
                $"@endTime," +
                $"@chunkCount," +
                $"@errorCode," +
                $"@errorMessage);", _dbConnection);

            AddParameter(_sqlCommand, "@fileId", System.Data.SqlDbType.UniqueIdentifier, new Guid(fileId));
            AddParameter(_sqlCommand, "@remotePath", System.Data.SqlDbType.NVarChar, bckSession.RemotePath.Substring(0, System.Math.Min(300, bckSession.RemotePath.Length)));
            AddParameter(_sqlCommand, "@isCompressed", System.Data.SqlDbType.Bit, bckSession.IsCompressed);
            AddParameter(_sqlCommand, "@compressedSize", System.Data.SqlDbType.BigInt, bckSession.CompressedSize);
            AddParameter(_sqlCommand, "@backupMethod", System.Data.SqlDbType.Int, bckSession.BackupMethod);
            AddParameter(_sqlCommand, "@isSchedule", System.Data.SqlDbType.Bit, bckSession.IsSchedule);
            AddParameter(_sqlCommand, "@startTime", System.Data.SqlDbType.DateTime, bckSession.StartTime);
            AddParameter(_sqlCommand, "@endTime", System.Data.SqlDbType.DateTime, bckSession.EndTime);
            AddParameter(_sqlCommand, "@chunkCount", System.Data.SqlDbType.Int, bckSession.ChunkCount);
            AddParameter(_sqlCommand, "@errorCode", System.Data.SqlDbType.Int, bckSession.ErrorCode);
            AddParameter(_sqlCommand, "@errorMessage", System.Data.SqlDbType.NVarChar, bckSession.ErrorMessage.Substring(0, System.Math.Min( 300, bckSession.ErrorMessage.Length)));
            _sqlCommand.ExecuteNonQuery();

            if (bckSession.ErrorCode == PstBackupEngine.BackupResultInfo.BackupResult.Success)
                UpdateBackupSuccessDate(clientId, fileId, bckSession.EndTime);
        }


        #endregion BackupSession
    }
}
