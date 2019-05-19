using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RandomnessChecker
{
    class SQLDatabase : IDatabaseConnection
    {
        private static readonly bool debug = false;

        private static Object connectionLock = new Object();

        public String TableName { get; set; }
        public static readonly Dictionary<DatabaseParameter, String> databaseToConnectionString
            = new Dictionary<DatabaseParameter, String>()
            {
                { DatabaseParameter.Host, "server" },
                { DatabaseParameter.Port, "port" },
                { DatabaseParameter.Username, "user" },
                { DatabaseParameter.Password, "password" },
                { DatabaseParameter.Database, "database" }
            };

        private MySqlConnection connection = null;
        private String connectionString;

        public SQLDatabase(String connectionString="")
        {

        }

        /**
         * @return 
         */
        public bool CanConnect(Dictionary<DatabaseParameter, String> databaseParams, String tableName)
        {
            String tempConnectionString = CreateConnectionString(databaseParams);
            MySqlConnection tempConnection;
            tempConnection = new MySqlConnection(tempConnectionString);

            bool isConnectionSuccessful = true;
            try
            {
                // Try opening connection so if fails will not set isConnectionSuccessful to true
                //tempConnection.Open();


                String queryTableString = "SELECT * FROM " + tableName;
                ExecuteNonQueryWithConnection(queryTableString, tempConnection);
            }
            catch (Exception e)
            {
                isConnectionSuccessful = false;
                if (debug)
                {
                    Console.WriteLine("Connection failed: ");
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }
            }
            
            return isConnectionSuccessful;
        }

        public void SetConnectionString(Dictionary<DatabaseParameter, String> databaseParams)
        {
            this.connectionString = CreateConnectionString(databaseParams);
        }

        public void SetConnectionString(String connectionString)
        {
            this.connectionString = connectionString;
        }

        private String CreateConnectionString(Dictionary<DatabaseParameter, String> databaseParams)
        {
            String tempConnectionString = "";

            var databaseParamValues = Enum.GetValues(typeof(DatabaseParameter));
            foreach (DatabaseParameter databaseParam in databaseParamValues)
            {
                String value = databaseParams[databaseParam];
                if (value != null)
                {
                    tempConnectionString += databaseToConnectionString[databaseParam] + "=" + value + ";";
                }
            }

            return tempConnectionString;
        }

        public void ConnectToDatabase()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to connect to database");
                if (debug)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public void AddToDatabase(DateTime dateTime, String label)
        {
            String sqlDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            String query = "INSERT INTO subredditstats (name, time) VALUES ('" + label + "', '" + sqlDateTime + "')";
            ExecuteNonQuery(query);
        }

        public bool IsInDatabase(String label)
        {
            return true;
        }

        public void PrintDatabase()
        {

        }

        public int GetNumberUniqueItems()
        {
            String query = "SELECT COUNT(DISTINCT(name)) FROM subredditstats;";
            int numberUniqueItems = ExecuteScalarQuery(query);
            return numberUniqueItems;
        }

        public int GetNumberTotalRecords()
        {
            String query = "SELECT COUNT(name) FROM subredditstats;";
            int numberTotalRecords = ExecuteScalarQuery(query);
            return numberTotalRecords;
        }

        public Dictionary<String, List<DateTime>> GetDataBetweenDates(DateTime dateTime1, DateTime dateTime2)
        {
            Dictionary<String, List<DateTime>> resultDict = new Dictionary<String, List<DateTime>>();

            connection.Open();
            String query = "SELECT name, time FROM subredditstats";
            var cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AddToDictionary(resultDict, (string) reader.GetValue(0), (DateTime) reader.GetValue(1));
            }
            connection.Close();

            return resultDict;
        }

        public Dictionary<String, List<DateTime>> GetAllData()
        {
            return GetDataBetweenDates(DateTime.Now.AddYears(-10), DateTime.Now);
        }

        private void ExecuteNonQuery(String query)
        {
            ExecuteNonQueryWithConnection(query, this.connection);
        }

        private int ExecuteScalarQuery(String query)
        {
            int result = 0;
            lock (connectionLock)
            {
                connection.Open();
                var cmd = new MySqlCommand(query, connection);
                long resultAsLong = (long)cmd.ExecuteScalar();
                result = (int)resultAsLong;
                connection.Close();
            }
            return result;
        }

        private void ExecuteNonQueryWithConnection(String query, MySqlConnection connection)
        {
            lock (connectionLock)
            {
                connection.Open();
                var cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void AddToDictionary(Dictionary<String, List<DateTime>> res, String key, DateTime value)
        {
            // Check if already in returnResults and add to existing or add new key
            if (res.ContainsKey(key))
            {
                res[key].Add(value);
            }
            else
            {
                List<DateTime> tempDateTime = new List<DateTime>();
                tempDateTime.Add(value);
                res.Add(key, tempDateTime);
            }
        }
    }
}
