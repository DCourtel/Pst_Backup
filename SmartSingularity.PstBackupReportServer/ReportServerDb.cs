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
        private SqlCommand _sqlCommand = null;

        public ReportServerDb(string dbPath)
        {
            this._dbPath = dbPath;
            _dbConnection = new SqlConnection($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PstBackup;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;AttachDbFileName = {_dbPath};");
            _sqlCommand = new SqlCommand(String.Empty, _dbConnection);
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
                if (_sqlCommand != null)
                {
                    _sqlCommand.Dispose();
                }
                if (_dbConnection != null && _dbConnection.State != System.Data.ConnectionState.Closed)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                }
            }
            catch (Exception) { }
        }

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
            _sqlCommand.CommandText = $"DELETE FROM tbClients WHERE ClientID LIKE '{client.Id}';";

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
            _sqlCommand.CommandText = $"SELECT * FROM tbClients WHERE ClientId LIKE '{client.Id}';";

            using (SqlDataReader reader = _sqlCommand.ExecuteReader())
            {
                result = reader.HasRows;
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
            _sqlCommand.CommandText = $"SELECT * FROM tbClients WHERE ClientId LIKE '{clientId}';";
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

            _sqlCommand.CommandText = $"INSERT INTO tbClients VALUES ('{client.Id}'," +
                $"'{client.Version.ToString()}'," +
                $"'{client.ComputerName}'," +
                $"'{client.Username}'," +
                $"'{justNow.ToString("yyyyMMdd HH:mm:ss")}');";

            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdateClientInfo(Client client)
        {
            _sqlCommand.CommandText = $"UPDATE tbClients SET " +
                $"ClientVersion='{client.Version.ToString()}', " +
                $"ComputerName='{client.ComputerName}', " +
                $"UserName='{client.Username}', " +
                $"LastContactDate='{client.LastContactDate.ToString("yyyyMMdd HH:mm:ss")}' " +
                $"WHERE ClientID LIKE '{client.Id}';";

            _sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Insert the PST file into the database if the PST file do not already exists, otherwise, update informations
        /// </summary>
        /// <param name="clientId">Unique ID of the client that own the PST file.</param>
        /// <param name="pstFile">Informations on the PST file.</param>
        /// <param name="bckSession">Informations on the backup session.</param>
        public void RegisterPstFile(string clientId, PstFile pstFile)
        {
            if (IsPstFileExists(clientId, pstFile.LocaPath))
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
            _sqlCommand.CommandText = $"SELECT * FROM tbPstFiles WHERE ClientID LIKE '{clientId}' AND LocalPath LIKE '{localPath}';";

            using (SqlDataReader reader = _sqlCommand.ExecuteReader())
            {
                result = reader.HasRows;
            }

            return result;
        }
        
        /// <summary>
        /// Retrieves the unique Id of a PST file from the clientId and the localPath.
        /// </summary>
        /// <param name="clientId">Unique Id of the client that own the PST file.</param>
        /// <param name="localPath">Path to the PST file on the client computer.</param>
        /// <returns></returns>
        public string GetPstFileId(string clientId, string localPath)
        {
            _sqlCommand.CommandText = $"SELECT * FROM tbPstFiles WHERE ClientId LIKE '{clientId}' AND LocalPath LIKE '{localPath}';";
            string pstFileId = String.Empty;

            using (SqlDataReader reader = _sqlCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        pstFileId = reader.GetString(2);
                    }
                }
            }

            return pstFileId;
        }

        private void RegisterNewPstFile(string clientId, PstFile pstFile)
        {
            _sqlCommand.CommandText = $"INSERT INTO tbPstFiles VALUES ('{clientId}','{pstFile.LocaPath}',{Guid.NewGuid()},{(pstFile.IsSetToBackup?'1':'0')},'{pstFile.Size}',{DBNull.Value});";
            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdatePstFileInfo(string clientId, PstFile pstFile)
        {
            string pstFileId = GetPstFileId(clientId, pstFile.LocaPath);

            _sqlCommand.CommandText = $"UPDATE tbPstFiles SET IsSetToBackup={(pstFile.IsSetToBackup ? '1' : '0')},Size='{pstFile.Size}';";
            _sqlCommand.ExecuteNonQuery();
        }
    }
}
