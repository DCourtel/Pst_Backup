using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupClientDb
{
    public class ClientDb
    {
        private string _dbPath = String.Empty;
        private SQLiteConnection _dbConnection = null;
        private SQLiteCommand _sqlCommand = null;


        /// <summary>
        /// Create a new instance of ClientDb
        /// </summary>
        /// <param name="dbPath">Full path to the database file</param>
        public ClientDb(string dbPath)
        {
            _dbPath = dbPath;
        }

        /// <summary>
        /// Create a well formated database file if the file does not exists or is malformed
        /// </summary>
        public void Initialize()
        {
            Logger.Write(30004, "Initializing Database", Logger.MessageSeverity.Debug);
            if (!ClientDb.IsDbExists(_dbPath))
            {
                CreateDb();
            }
            else if (!IsDbWellFormated())
            {
                System.IO.File.Delete(_dbPath);
                CreateDb();
            }
        }

        /// <summary>
        /// Gets the full path to the database file
        /// </summary>
        public string GetDbPath { get { return _dbPath; } }

        public void Connect()
        {
            Disconnect();
            _dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath));
            _sqlCommand = new SQLiteCommand(_dbConnection);
            _dbConnection.Open();
        }

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
        /// Check whether or not the database file exists
        /// </summary>
        /// <param name="dbPath">Full path to the database file</param>
        /// <returns>Return true if the database file exists</returns>
        public static bool IsDbExists(string dbPath)
        {
            bool result = false;

            try
            {
                System.IO.FileInfo dbFile = new System.IO.FileInfo(dbPath);
                result = dbFile.Exists;
            }
            catch (Exception) { }

            return result;
        }

        /// <summary>
        /// Check whether or not the database file have the right tables and each tables have the right columns
        /// </summary>
        /// <returns>Return true if the database have the right tables and columns</returns>
        public bool IsDbWellFormated()
        {
            List<string> dbColumns = new List<string>();

            try
            {
                SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath));
                dbConnection.Open();
                System.Data.DataTable tables = dbConnection.GetSchema("Columns");

                string tableColumns = String.Empty;

                foreach (System.Data.DataRow row in tables.Rows)
                {
                    foreach (System.Data.DataColumn col in tables.Columns)
                    {
                        tableColumns += row[col] + ";";
                    }
                    tableColumns += "\r\n";
                }
                dbColumns = GetDbColumns(tableColumns);
                dbConnection.Close();
            }
            catch (Exception) { }

            return dbColumns.Contains("tbFiles;fileId") && dbColumns.Contains("tbFiles;sourceFile") && dbColumns.Contains("tbFiles;destinationFile") &&
                dbColumns.Contains("tbHashes;fileId") && dbColumns.Contains("tbHashes;chunkId") && dbColumns.Contains("tbHashes;hash");
        }

        /// <summary>
        /// Create the file to host the SQLite database and associated tables
        /// </summary>
        public void CreateDb()
        {
            Logger.Write(30005, "Creating a new database\r\n" + _dbPath, Logger.MessageSeverity.Debug);
            string sqlCommand;

            // Create Db file
            SQLiteConnection.CreateFile(_dbPath);

            // DDL command to create Files table
            sqlCommand = "CREATE TABLE tbFiles(fileId INTEGER PRIMARY KEY, sourceFile NVARCHAR(260) UNIQUE, destinationFile NVARCHAR(260) NOT NULL);";
            ExecuteDbNonQuery(sqlCommand);

            // DDL command to create Hashes table
            sqlCommand = "CREATE TABLE tbHashes(fileId INTEGER NOT NULL, chunkId INTEGER NOT NULL, hash CHARACTER(32) NOT NULL, PRIMARY KEY (fileID,chunkID));";
            ExecuteDbNonQuery(sqlCommand);
        }

        /// <summary>
        /// Check whether or not the sourceFile and the destinationFile are register together in the database
        /// </summary>
        /// <param name="pstFile">Full path of the source PST file to search</param>
        /// <returns>Returns true if at least one row is found in the database</returns>
        public bool IsPstFileRegistered(string pstFile)
        {
            bool result = false;

            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();

                string sqlCommandText = String.Format("SELECT * FROM tbFiles WHERE SourceFile LIKE '{0}';", pstFile.ToLower());
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommandText, dbConnection))
                {
                    SQLiteDataReader reader = sqlCommand.ExecuteReader();
                    result = reader.HasRows;

                    reader.Close();
                    dbConnection.Close();   // Déconnexion de la base de données.
                    Logger.Write(30006, pstFile + (!result ? " does not" : "") + " exists in the database", Logger.MessageSeverity.Debug);
                }
            }
            return result;
        }

        /// <summary>
        /// Insert a new row in the tbFiles table. And create "blank" hashes in the tbHashes table.
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database. Enable to make the link with the tbHashes table</param>
        /// <param name="pstFile">Full path to the PST file</param>
        /// <param name="backupFile">Full path to the backup file</param>
        public void RegisterNewPstFile(int fileId, string pstFile, string backupFile)
        {
            // Create Pst File in the tbFiles
            string dbCommand = String.Format("INSERT INTO tbFiles VALUES ({0},'{1}','{2}');", fileId, pstFile.ToLower(), backupFile.ToLower());
            ExecuteDbNonQuery(dbCommand);
            Logger.Write(30007, "Registering a new PST file in the database\r\n" + pstFile + "\r\n" + backupFile, Logger.MessageSeverity.Debug);
        }

        /// <summary>
        /// Delete all rows in the tbFiles table, for which "pstFile" and "backupFile" match the fields "SourceFile" and "DestinationFile"
        /// </summary>
        /// <param name="pstFile">Full path of the PST file</param>
        public void DeletePstFile(string pstFile)
        {
            Logger.Write(30008, "Deleting " + pstFile + " from the database", Logger.MessageSeverity.Debug);
            // Get FileId of the record
            int fileId = GetFileID(pstFile);

            // Delete Pst File from the tbPstFiles table
            string dbCommand = String.Format("DELETE FROM tbFiles WHERE SourceFile LIKE '{0}';", pstFile.ToLower());
            ExecuteDbNonQuery(dbCommand);

            // Delete hashes from the tbHashes table
            dbCommand = String.Format("DELETE FROM tbHashes WHERE FileId={0};", fileId);
            ExecuteDbNonQuery(dbCommand);
        }

        /// <summary>
        /// Get the FileId of a record in the database
        /// </summary>
        /// <param name="pstFile">Full name of the PST file</param>
        /// <returns>Returns the FileId in the database of the record</returns>
        public int GetFileID(string pstFile)
        {
            int result = 0;

            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();

                string sqlCommandText = String.Format("SELECT * FROM tbFiles WHERE SourceFile LIKE '{0}';", pstFile.ToLower());
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommandText, dbConnection))
                {
                    SQLiteDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }

                    reader.Close();
                    dbConnection.Close();   // Déconnexion de la base de données.
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a fileId that is not used in the database
        /// </summary>
        /// <returns>Returns a fileId not in use in the database</returns>
        public int GetAvailableFileId()
        {
            int availableFileId = 0;
            List<int> usedFileId = new List<int>();

            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();

                string sqlCommandText = "SELECT * FROM tbFiles;";
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommandText, dbConnection))
                {
                    SQLiteDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            usedFileId.Add(reader.GetInt32(0));
                        }
                    }
                    reader.Close();
                    dbConnection.Close();   // Déconnexion de la base de données.
                }
            }
            for (int i = 1; i < 10000; i++)
            {
                if (!usedFileId.Contains(i))
                {
                    availableFileId = i;
                    break;
                }
            }
            return availableFileId;
        }

        /// <summary>
        /// Gets the full path to the backup file
        /// </summary>
        /// <param name="pstFile">Full path to the PST file</param>
        /// <returns>Return the full path to the backup file</returns>
        public string GetBackupFilePath(string pstFile)
        {
            string result = String.Empty;

            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();

                string sqlCommandText = String.Format("SELECT * FROM tbFiles WHERE SourceFile LIKE '{0}';", pstFile.ToLower());
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommandText, dbConnection))
                {
                    SQLiteDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            result = reader.GetString(2);
                        }
                    }
                    reader.Close();
                    dbConnection.Close();   // Déconnexion de la base de données.
                }
            }
            Logger.Write(30009, "The backup filepath for " + pstFile + " is " + result, Logger.MessageSeverity.Debug);
            return result;
        }

        /// <summary>
        /// Replace the full path to the backup file by this provided
        /// </summary>
        /// <param name="pstFile">Full path to the PST file</param>
        /// <param name="backupFile">Full path to the backup file</param>
        public void RenameBackupFile(string pstFile, string backupFile)
        {
            string dbCommand = String.Format("UPDATE tbFiles SET DestinationFile='{0}'  WHERE SourceFile LIKE '{1}';", backupFile, pstFile);
            ExecuteDbNonQuery(dbCommand);
        }

        /// <summary>
        /// Returns the hash for the specific chunk of the specific file
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunkId">Unique ID of the chunk in the database</param>
        /// <returns>If the couple fileId/chunkId exists, returns the corresponding hash, otherwise, returns String.Emptty</returns>
        public string GetHash(int fileId, int chunkId)
        {
            string hash = String.Empty;

            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();

                string sqlCommandText = String.Format("SELECT * FROM tbHashes WHERE FileId={0} AND ChunkId={1};", fileId, chunkId);
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommandText, dbConnection))
                {
                    SQLiteDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                            hash = reader.GetString(2);
                    }
                    reader.Close();
                    dbConnection.Close();   // Déconnexion de la base de données.
                }
            }
            return hash;
        }

        /// <summary>
        /// Gets a list of Hashes for the fileId specified
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <returns>Returns a list of hashes</returns>
        public List<string> GetHashes(int fileId)
        {
            List<string> hashes = new List<string>();

            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();

                string sqlCommandText = String.Format("SELECT * FROM tbHashes WHERE FileId={0} ORDER BY ChunkId;", fileId);
                using (SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommandText, dbConnection))
                {
                    SQLiteDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            hashes.Add(reader.GetString(2));
                        }
                    }
                    reader.Close();
                    dbConnection.Close();   // Déconnexion de la base de données.
                }
            }
            return hashes;
        }

        /// <summary>
        /// Insert the provided hash into the database
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunkId">Unique ID of the chunk in the database</param>
        /// <param name="hash">Hash to insert into the database</param>
        public void InsertHash(int fileId, int chunkId, string hash)
        {
            string dbCommand = String.Format("INSERT INTO tbHashes VALUES ({0}, {1}, '{2}');", fileId, chunkId, hash);
            ExecuteDbNonQuery(dbCommand);
        }

        /// <summary>
        /// Insert the provided hash into the database. The connection to the database must be already open. The connection is live open at the end.
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunkId">Unique ID of the chunk in the database</param>
        /// <param name="hash">Hash to insert into the database</param>
        public void InsertHashToConnectedDb(int fileId, int chunkId, string hash)
        {
            ExecuteDbNonQueryToConnectedDb(String.Format("INSERT INTO tbHashes VALUES ({0}, {1}, '{2}');", fileId, chunkId, hash));
        }

        /// <summary>
        /// Update an existing record in the database with the provided hash
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunkId">Unique ID of the chunk in the database</param>
        /// <param name="hash">Hash to write into the database</param>
        public void UpdateHash(int fileId, int chunkId, string hash)
        {
            string dbCommand = String.Format("UPDATE tbHashes SET Hash='{2}'  WHERE FileId={0} AND ChunkId={1};", fileId, chunkId, hash);
            ExecuteDbNonQuery(dbCommand);
        }

        /// <summary>
        /// Update an existing record in the database with the provided hash. The connection to the database must be already open. The connection is live open at the end.
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunkId">Unique ID of the chunk in the database</param>
        /// <param name="hash">Hash to write into the database</param>
        public void UpdateHashInConnectedDb(int fileId, int chunkId, int hash)
        {
            ExecuteDbNonQueryToConnectedDb(String.Format("UPDATE tbHashes SET Hash='{2}'  WHERE FileId={0} AND ChunkId={1};", fileId, chunkId, hash));
        }

        /// <summary>
        /// Delete the corresponding row in tbHashes table
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunckId">Unique ID of the chunk in the database</param>
        public void DeleteHash(int fileId, int chunckId)
        {
            string dbCommand = String.Format("DELETE FROM tbHashes WHERE FileId={0} AND ChunkId={1};", fileId, chunckId);
            ExecuteDbNonQuery(dbCommand);
        }

        /// <summary>
        /// Delete the rows in tbHashes table
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="chunckId">Unique ID of the chunk from which to start the deletion. Zero to delete all hashes</param>
        public void DeleteHashes(int fileId, int chunckId)
        {
            string dbCommand = String.Format("DELETE FROM tbHashes WHERE FileId={0} AND ChunkId>={1};", fileId, chunckId);
            ExecuteDbNonQuery(dbCommand);
        }

        /// <summary>
        /// Shrink the chunk list of a previously registred PST file.
        /// </summary>
        /// <param name="fileId">Unique ID of the file in the database</param>
        /// <param name="fileSize">The size of the backup file</param>
        public void ShrinkChunkList(int fileId, long fileSize, int chunkSize)
        {
            int targetedChunkCount = (int)System.Math.Ceiling(fileSize / (double)chunkSize);
            int actualChunkCount = GetHashes(fileId).Count;

            if (actualChunkCount > targetedChunkCount)
            {
                DeleteHashes(fileId, targetedChunkCount);
            }
        }

        /// <summary>
        /// Execute a non-selective SQL command against the database
        /// </summary>
        /// <param name="dbCommand">SQL command to run</param>
        private void ExecuteDbNonQuery(string dbCommand)
        {
            using (SQLiteConnection dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _dbPath)))
            {
                dbConnection.Open();    // Connect to the Database

                using (SQLiteCommand sqlCommand = new SQLiteCommand(dbCommand, dbConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                    
                    dbConnection.Close();   // Disconnect from the Db
                }
            }
        }

        /// <summary>
        /// Execute a non-selective SQL command against the database. A connection to the database must be open before calling this method. The connection is live open at the end.
        /// </summary>
        /// <param name="dbCommand">SQL command to run</param>
        private void ExecuteDbNonQueryToConnectedDb(string dbCommand)
        {
            _sqlCommand.CommandText = dbCommand;
            _sqlCommand.ExecuteNonQuery();
        }

        private static List<string> GetDbColumns(string dataColumns)
        {
            List<string> dbColumns = new List<string>();
            string[] lines = dataColumns.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] items = line.Split(new char[] { ';' });
                dbColumns.Add(items[2] + ";" + items[3]);
            }

            return dbColumns;
        }
    }
}
