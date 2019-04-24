using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RandomnessChecker
{
    class SQLDatabase : IDatabaseConnection
    {
        private static Object connectionLock = new Object();

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
        private static String connectionString; // = "server=localhost;port=3306;user=User;password=ZZIzhRD$0*@u3ZWs;database=redditrandomnesschecker";

        public SQLDatabase(String connectionString="")
        {

        }

        public bool CanConnect(Dictionary<DatabaseParameter, String> databaseParams)
        {
            if (connectionString == null || connectionString == "")
            {
                Console.WriteLine("No connection string provided");
                throw new Exception();
            }

            bool isConnectionSuccessful = false;
            try
            {
                connection = new MySqlConnection(connectionString);
                isConnectionSuccessful = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection failed");
                Console.WriteLine(e.Message);
            }

            return isConnectionSuccessful;
        }

        public void SetConnectionString(Dictionary<DatabaseParameter, String> databaseParams)
        {
            connectionString = "";

            var databaseParamValues = Enum.GetValues(typeof(DatabaseParameter));
            foreach (DatabaseParameter databaseParam in databaseParamValues)
            {
                String value = databaseParams[databaseParam];
                if (value != null)
                {
                    connectionString += databaseToConnectionString[databaseParam] + "=" + value + ";";
                }
            }
        }

        public void AddToDatabase(DateTime dateTime, String label)
        {
            lock (connectionLock)
            {
                connection.Open();
                String sqlDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                String query = "INSERT INTO subredditstats (name, time) VALUES ('" + label + "', '" + sqlDateTime + "')";
                var cmd = new MySqlCommand(query, connection);
                int reader = cmd.ExecuteNonQuery();
                connection.Close();
            }
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
            int result = 0;
            lock (connectionLock)
            {
                connection.Open();
                String query = "SELECT COUNT(DISTINCT(name)) FROM subredditstats;";
                var cmd = new MySqlCommand(query, connection);
                long resultAsLong = (long) cmd.ExecuteScalar();
                result = (int) resultAsLong;
                connection.Close();
            }
            return result;
        }

        public int GetNumberTotalRecords()
        {
            int result = 0;
            lock (connectionLock)
            {
                connection.Open();
                String query = "SELECT COUNT(name) FROM subredditstats;";
                var cmd = new MySqlCommand(query, connection);
                long resultAsLong = (long)cmd.ExecuteScalar();
                result = (int)resultAsLong;
                connection.Close();
            }
            return result;
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
