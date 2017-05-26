using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using SmartSingularity.PstBackupReportServer;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupReportServerDb
{
    public class PstBackupReportServerDb
    {
        private string _dbPath = String.Empty;
        private SqlConnection _dbConnection = null;
        private SqlCommand _sqlCommand = null;

        public PstBackupReportServerDb(string dbPath)
        {
            this._dbPath = dbPath;
        }

        /// <summary>
        /// Open a connection to the database
        /// </summary>
        public void Connect()
        {
            Disconnect();
            _dbConnection = new SqlConnection($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PstBackup;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;AttachDbFileName = {_dbPath};");
            _sqlCommand = new SqlCommand(String.Empty, _dbConnection);
            _dbConnection.Open();
        }

        /// <summary>
        /// Close the connection to the database
        /// </summary>
        public void Disconnect()
        {
            if (_sqlCommand != null)
            {
                _sqlCommand.Dispose();
            }
            if (_dbConnection != null)
            {
                if (_dbConnection.State != System.Data.ConnectionState.Closed)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                }
            }
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
            DateTime justNow = DateTime.Now.ToUniversalTime();

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


    }
}
