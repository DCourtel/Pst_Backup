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
            _dbConnection = new SqlConnection($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PstBackup;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;AttachDbFileName = {_dbPath};");
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
            _sqlCommand.Parameters.AddWithValue("@clientId", client.Id);

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
                _sqlCommand.Parameters.AddWithValue("@clientId", client.Id);
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
            _sqlCommand.Parameters.AddWithValue("@clientId", clientId);
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
                $"'{justNow.ToString("yyyyMMdd HH:mm:ss")}');", _dbConnection);
            _sqlCommand.Parameters.AddWithValue("@clientId", client.Id);
            _sqlCommand.Parameters.AddWithValue("@version", client.Version.ToString());
            _sqlCommand.Parameters.AddWithValue("@computerName", client.ComputerName);
            _sqlCommand.Parameters.AddWithValue("@userName", client.Username);

            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdateClientInfo(Client client)
        {
            DateTime justNow = DateTime.UtcNow;
            var _sqlCommand = new SqlCommand("UPDATE tbClients SET " +
                "Version=@version, " +
                "ComputerName=@computerName, " +
                "UserName=@userName, " +
                $"LastContactDate='{justNow.ToString("yyyyMMdd HH:mm:ss")}' " +
                $"WHERE Id LIKE @clientId;", _dbConnection);
            _sqlCommand.Parameters.AddWithValue("@version", client.Version.ToString());
            _sqlCommand.Parameters.AddWithValue("@computerName", client.ComputerName);
            _sqlCommand.Parameters.AddWithValue(@"userName", client.Username);
            _sqlCommand.Parameters.AddWithValue("@clientId", client.Id);

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
                _sqlCommand.Parameters.AddWithValue("@clientId", clientId);
                _sqlCommand.Parameters.AddWithValue("@localPath", localPath);
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
                _sqlCommand.Parameters.AddWithValue("@clientId", clientId);
                _sqlCommand.Parameters.AddWithValue("@localPath", localPath);
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
            _sqlCommand.Parameters.AddWithValue("@clientId", clientId);
            _sqlCommand.Parameters.AddWithValue("@localPath", pstFile.LocalPath);
            _sqlCommand.Parameters.AddWithValue("@isSetToBackup", pstFile.IsSetToBackup);
            _sqlCommand.Parameters.AddWithValue("@size", pstFile.Size);
            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdatePstFileInfo(string clientId, PstFile pstFile)
        {
            var _sqlCommand = new SqlCommand("UPDATE tbPstFiles SET IsSetToBackup=@isSetToBackup,Size=@size,LastSuccessfulBackup=@lastSuccessfulBackup;", _dbConnection);
            _sqlCommand.Parameters.AddWithValue("@isSetToBackup", pstFile.IsSetToBackup);
            _sqlCommand.Parameters.AddWithValue("@size", pstFile.Size);
            _sqlCommand.Parameters.AddWithValue("@lastSuccessfulBackup", ((object)pstFile.LastSuccessfulBackup?.ToString("yyyyMMdd HH:mm:ss") ?? DBNull.Value));
            _sqlCommand.ExecuteNonQuery();
        }

        #endregion Pst Files
    }
}
