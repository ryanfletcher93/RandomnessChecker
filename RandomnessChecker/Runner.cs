using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace RandomnessChecker
{
    public enum DatabaseType
    {
        Invalid, NonPersistent, MySql
    }

    public enum DatabaseParameter
    {
        Host, Port, Database, Username, Password
    }

    public enum Operation
    {
        Invalid, Quit, GatherData, ReportOnDataIntegrity, GenerateReport
    }

    class Runner
    {
        private static bool Debug = false;

        private CommandLineInterface cmdInterface = new CommandLineInterface();
        //private ConfigManager configManager;
        private IRunInfo runInfo = new DefaultRunInfo();
        //private IGetRandomUnit getRandomUnit;
        private ISqlDatabaseConnection database;
        
        private static int maxConcurrentRequests = 5;

        /**
         * Setup config for program in constructor
         */
        public Runner(String configFilePath)
        {

        }

        /**
         * Start execution of program
         */
        public void run()
        {
            // TODO: Include other options besides MySQL
            DatabaseType databaseType = DatabaseType.Invalid;
            while (databaseType == DatabaseType.Invalid)
            {
                databaseType = cmdInterface.GetDatabaseType();
            }

            if (databaseType == DatabaseType.MySql)
            {
                database = new SQLDatabase();
            }
            else
            {
                throw new Exception();
            }

            // Check that can connect to database if not non persistent
            if (databaseType != DatabaseType.NonPersistent)
            {
                Dictionary<DatabaseParameter, String> databaseParams;
                String tableName;
                do
                {
                    databaseParams = cmdInterface.GetDatabaseParams();
                    tableName = cmdInterface.GetTableName();
                }
                while (!CanConnectToDatabase(databaseParams, tableName));

                database.SetConnectionString(databaseParams);
                database.TableName = tableName;
            }

            Operation currOperation;
            while (true)
            {
                currOperation = cmdInterface.GetAction();

                if (currOperation == Operation.GatherData)
                {
                    String requestString = cmdInterface.GetRandomiserUrl();
                    String regexString = cmdInterface.GetRegexForReturnUrl();

                    DisplayExampleRequestAndResponse();

                    if (cmdInterface.ConfirmSelection())
                    {
                        runInfo.RequestString = requestString;
                        runInfo.RegexString = regexString;

                        runInfo.NumberOfRequests = cmdInterface.GetNumberOfRequests();

                        GetData();
                        Console.WriteLine("All results collected\n");
                    }
                }
                else if (currOperation == Operation.ReportOnDataIntegrity)
                {
                    ReportOnRandomness();
                }
                else if (currOperation == Operation.GenerateReport)
                {
                    AnalyseAllData();
                }
                // If not standard operation, quit program
                else
                {
                    break;
                }
            }
        }

        private bool CanConnectToDatabase(Dictionary<DatabaseParameter, String> databaseParams, String tableName)
        {
            bool canConnect =  database.CanConnect(databaseParams, tableName);
            if (canConnect == false)
            {
                Console.WriteLine("Could not connect");
            }
            return canConnect;
        }

        public void DisplayExampleRequestAndResponse()
        {

        }

        private void GetData()
        {
            //TODO: Implement actual number or requested tatus

            List<Task> taskList = new List<Task>();
            for (int i=0; i< runInfo.NumberOfRequests; i++)
            {
                Task tempTask = Task.Factory.StartNew(() => GetRandomUnit());
                taskList.Add(tempTask);
                
                // Wait for maxConcurrentRequests to complete before creating more threads
                if (i % maxConcurrentRequests == 0 && i!=0)
                {
                    Task.WaitAll(taskList.ToArray());
                }
                
                if (Debug)
                {
                    Console.WriteLine("i: " + i);
                }

            }

            Task.WaitAll(taskList.ToArray());
            
        }

        private void GetRandomUnit()
        {
            String resStr = (new GetWebPage()).GetData(runInfo);

            if (resStr != null)
            {
                database.AddToDatabase(DateTime.Now, resStr);
            }
        }

        private void ReportOnRandomness()
        {
            int numberDistinctItems = database.GetNumberUniqueItems();
            int numberRecords = database.GetNumberTotalRecords();
            Console.WriteLine("Number of distinct values in database: " + numberDistinctItems);
            Console.WriteLine("Number of records in database: " + numberRecords);

            float ratioOfTotalToDistinct = ((float)numberRecords / (float)numberDistinctItems);
            Console.WriteLine("Number or records for each distinct item: " + ratioOfTotalToDistinct);

            Console.WriteLine();
        }

        private void AnalyseDataBetweenDates(DateTime dt1, DateTime dt2)
        {
            Dictionary<String, List<DateTime>> dataBetweenDates;
            dataBetweenDates = database.GetDataBetweenDates(dt1, dt2);

            if (Debug)
            {
                Console.WriteLine("Data between " + dt1 + " and " + dt2);
                Console.WriteLine("---------------------------------------------");
                PrintData(dataBetweenDates);
                Console.WriteLine();
            }

            PrintCountOfData(dataBetweenDates);

            PrintFrequency(dataBetweenDates);

            float aveRequestsPerItem = GetAverageRequestsPerItem(dataBetweenDates);
            Dictionary<String, float> squaredDiff = 
                GetSquaredDiffToAverageRequestsPerItem(dataBetweenDates, aveRequestsPerItem);

            if (Debug)
            {
                PrintAllSquaredDiffs(squaredDiff);
            }

            PrintAllSquaredDiffsAboveThreshold(squaredDiff, 100);

        }

        private void AnalyseAllData()
        {
            DateTime dt1 = DateTime.Now.AddYears(-1000);
            DateTime dt2 = DateTime.Now;
            AnalyseDataBetweenDates(dt1, dt2);
        }

        private void PrintData(Dictionary<String, List<DateTime>> data)
        {
            foreach (KeyValuePair<String, List<DateTime>> kvp in data)
            {
                Console.Write(kvp.Key + ": ");
                foreach (DateTime time in kvp.Value)
                {
                    Console.Write(time + ", ");
                }
                Console.WriteLine();
            }
        }

        private void PrintCountOfData(Dictionary<String, List<DateTime>> data)
        {
            foreach (KeyValuePair<String, List<DateTime>> kvp in data)
            {
                Console.WriteLine(kvp.Value.Count + ": " + kvp.Key);
            }
            Console.WriteLine();
        }

        private void PrintFrequency(Dictionary<String, List<DateTime>> data)
        {
            SortedDictionary<int, int> frequencyDict = new SortedDictionary<int, int>();
            foreach (KeyValuePair<String, List<DateTime>> value in data)
            {
                int count = value.Value.Count;
                if (frequencyDict.ContainsKey(count))
                {
                    frequencyDict[count]++;
                }
                else
                {
                    frequencyDict.Add(count, 1);
                }
            }

            Console.WriteLine("Frequency distribution");
            foreach (KeyValuePair<int, int> frequencyRow in frequencyDict)
            {
                Console.WriteLine(frequencyRow.Key + ": " + frequencyRow.Value);
            }
        }

        private float GetAverageRequestsPerItem(Dictionary<String, List<DateTime>> data)
        {
            int numberRecords = database.GetNumberTotalRecords();
            int numberDistinctItems = database.GetNumberUniqueItems();
            float ratioOfTotalToDistinct = ((float)numberRecords / (float)numberDistinctItems);

            return ratioOfTotalToDistinct;
        }

        private Dictionary<String, float> GetSquaredDiffToAverageRequestsPerItem(Dictionary<String, List<DateTime>> data, 
            float aveRequestsPerItem)
        {
            Dictionary<String, float> itemToSquaredDiffMap = new Dictionary<string, float>();

            foreach (KeyValuePair<String, List<DateTime>> dataLine in data)
            {
                float diff = Math.Abs(dataLine.Value.Count - aveRequestsPerItem);
                float diffSquared = diff * diff;

                itemToSquaredDiffMap.Add(dataLine.Key, diffSquared);
            }

            return itemToSquaredDiffMap;
        }

        private void PrintAllSquaredDiffs(Dictionary<String, float> squaredDiffsMap)
        {

        }

        private void PrintAllSquaredDiffsAboveThreshold(Dictionary<String, float> squaredDiffsMap, int threshold)
        {
            Dictionary<String, float> thresholdedDiffsMap = new Dictionary<string, float>();

            foreach (KeyValuePair<String, float> squaredDiffsLine in squaredDiffsMap)
            {
                if (squaredDiffsLine.Value >= threshold)
                {
                    thresholdedDiffsMap.Add(squaredDiffsLine.Key, squaredDiffsLine.Value);
                }
            }

            PrintAllSquaredDiffs(thresholdedDiffsMap);
        }

        private void PrintNumberDistinctSubreddits(Dictionary<String, List<DateTime>> data)
        {

        }
    }
}
