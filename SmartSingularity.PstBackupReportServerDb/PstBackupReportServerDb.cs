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
            _dbConnection = new SqlConnection($"Server=(localdb)\v11.0;Integrated Security=true;AttachDbFileName = {_dbPath};");
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

        public void RegisterClient(Client client)
        {
            if (IsClientExists(client))
                UpdateClientInfo(client);
            else
                RegisterNewClient(client);
        }

        public bool IsClientExists(Client client)
        {
            _sqlCommand.CommandText = $"SELECT * FROM tbClients WHERE ClientId={client.Id};";

            SqlDataReader reader = _sqlCommand.ExecuteReader();
            return reader.HasRows;
        }

        public Client GetClient(string clientId)
        {
            _sqlCommand.CommandText = $"SELECT * FROM tbClients WHERE ClientId={clientId};";
            Client client = new Client();

            SqlDataReader reader = _sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    client.Id = reader.GetString(0);
                    client.Version = new Version( reader.GetString(1));
                    client.ComputerName = reader.GetString(2);
                    client.Username = reader.GetString(3);
                    client.LastContactDate = reader.GetDateTime(4);
                }
            }

            return client;
        }

        private void RegisterNewClient(Client client)
        {
            _sqlCommand.CommandText = $"INSERT INTO tbClients VALUES ({client.Id}," +
                $"{client.Version.ToString()}," +
                $"{client.ComputerName}," +
                $"{client.Username}," +
                $"{DateTime.Now.ToUniversalTime()});";

            _sqlCommand.ExecuteNonQuery();
        }

        private void UpdateClientInfo(Client client)
        {
            _sqlCommand.CommandText = $"UPDATE tbClients SET " +
                $"ClientVersion='{client.Version.ToString()}', " +
                $"ComputerName='{client.ComputerName}', " +
                $"UserName='{client.Username}', " +
                $"LastContactDate={client.LastContactDate}' " +
                $"WHERE ClientID LIKE '{client.Id}';";

            _sqlCommand.ExecuteNonQuery();
        }
    }
}
