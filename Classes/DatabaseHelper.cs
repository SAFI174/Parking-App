using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace ParkingApp.Classes
{
    public class DatabaseHelper
    {
        public static string databaseFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ParkingApp\\database.txt";

        public static string encryptionPassword = "80X9q!Sq";
        public string connectionString;
        private SqlConnection sqlConnection;
        private SqlDataAdapter sqlDataAdapter;
        public DatabaseHelper()
        {
            connectionString = ReadConnectionStringFromFile();
            sqlConnection = new SqlConnection(connectionString);
            sqlDataAdapter = new SqlDataAdapter();
            OpenConnection();
            CloseConnection();
        }
        // Read ecrypted connection string from database.txt file 
        private string ReadConnectionStringFromFile()
        {
            // check if file exist
            if (!File.Exists(databaseFilePath))
            {
                // create the file if not exist
                StreamWriter writer = new StreamWriter(databaseFilePath);
                writer.Close();
            }
            // otherwise read the first line of it
            StreamReader reader = new StreamReader(databaseFilePath);
            string secureText = reader.ReadLine();
            reader.Close();
            // return decrypt connection string
            return Encryption.DecryptString(secureText, encryptionPassword);
        }

        // run custom sql query and return table
        public DataTable ExecuteQueryWithTable(string queryText)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = queryText;
            sqlCommand.Connection = sqlConnection;
            DataTable dataTable = new DataTable();
            sqlDataAdapter.SelectCommand = sqlCommand;
            OpenConnection();
            sqlDataAdapter.Fill(dataTable);
            CloseConnection();
            return dataTable;
        }
        // run custom sql query
        public void ExecuteQuery(string queryText)
        {
            OpenConnection();
            SqlCommand sqlCommand = new SqlCommand(queryText, sqlConnection);
            sqlCommand.ExecuteNonQuery();
            CloseConnection();
        }
        // run stored proc from database with parameters and return table
        public DataTable ExecuteWithTable(string procedureName, SqlParameter[] parameters = null)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = procedureName;
            sqlCommand.Connection = sqlConnection;
            if (parameters != null)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            DataTable dataTable = new DataTable();
            sqlDataAdapter.SelectCommand = sqlCommand;
            OpenConnection();
            sqlDataAdapter.Fill(dataTable);
            CloseConnection();
            return dataTable;
        }
        // run stored proc from database with parameters
        public void ExecuteWithoutTable(string procedureName, SqlParameter[] parameters)
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = procedureName;
            sqlCommand.Connection = sqlConnection;
            if (parameters != null)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            OpenConnection();
            sqlCommand.ExecuteNonQuery();
            CloseConnection();
        }

        private void OpenConnection()
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        private void CloseConnection()
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
    }
}
