using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RandomnessChecker
{
    public enum InputType
    {
        File, CommandLine
    }

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
        private IDatabaseConnection database;
        
        private static int maxConcurrentRequests = 5;

        /**
         * Setup config for program in constructor
         */
        public Runner(String configFilePath)
        {

        }

        /**
         * Start execution of program
         * 
         * TODO: Move some of this to functions or another class
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
                String filePath;
                Dictionary<DatabaseParameter, String> databaseParams;
                String tableName;
                do
                {
                    databaseParams = new Dictionary<DatabaseParameter, String>();
                    tableName = "";
                    //databaseParams = cmdInterface.GetDatabaseParams();
                    filePath = cmdInterface.GetFilePath();
                    
                    using (StreamReader sr = File.OpenText(filePath))
                    {
                        String s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            String[] splitString = s.Split(':');
                            
                            switch (splitString[0])
                            {
                                case "Host":
                                    databaseParams.Add(DatabaseParameter.Host, splitString[1]);
                                    break;
                                case "Port":
                                    databaseParams.Add(DatabaseParameter.Port, splitString[1]);
                                    break;
                                case "Database":
                                    databaseParams.Add(DatabaseParameter.Database, splitString[1]);
                                    break;
                                case "Username":
                                    databaseParams.Add(DatabaseParameter.Username, splitString[1]);
                                    break;
                                case "Password":
                                    databaseParams.Add(DatabaseParameter.Password, splitString[1]);
                                    break;
                                case "Table":
                                    tableName = splitString[1];
                                    break;
                            }
                        }
                    }
                    
                }
                while (!CanConnectToDatabase(databaseParams, tableName));

                database.SetConnectionString(databaseParams);
                database.TableName = tableName;

                database.ConnectToDatabase();
            }

            // Run operation specified by user
            Operation currOperation;
            while (true)
            {
                // Get action
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

        private void GetDatabaseParamsFromFile(String filePath)
        {

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

            PrintActualAndExpectedResultsPerItem(dataBetweenDates);

            DrawChartOfData(dataBetweenDates);
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

        private void PrintActualAndExpectedResultsPerItem(Dictionary<String, List<DateTime>> dataBetweenDates)
        {
            float aveRequestsPerItem = GetAverageRequestsPerItem(dataBetweenDates);
            int numberRecords = 0;
            foreach (var res in dataBetweenDates)
            {
                numberRecords += res.Value.Count;
            }
            float expectedAveRequestsPerItemIfRandom = 
                (numberRecords) / (float)dataBetweenDates.Keys.Count;

            Console.WriteLine("Actual requests per item: " + aveRequestsPerItem);
            Console.WriteLine("Expected requests per item: " + expectedAveRequestsPerItemIfRandom);
        }

        private float GetAverageRequestsPerItem(Dictionary<String, List<DateTime>> data)
        {
            int numberRecords = database.GetNumberTotalRecords();
            int numberDistinctItems = database.GetNumberUniqueItems();
            float ratioOfTotalToDistinct = ((float)numberRecords / (float)numberDistinctItems);

            return ratioOfTotalToDistinct;
        }

        private void PrintNumberDistinctSubreddits(Dictionary<String, List<DateTime>> data)
        {

        }

        private void DrawChartOfData(Dictionary<String, List<DateTime>> dataBetweenPoints)
        {
            Application.Run(new Form1(dataBetweenPoints));
        }
    }
}
