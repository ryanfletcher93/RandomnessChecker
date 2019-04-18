using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace RandomnessChecker
{
    class Runner
    {
        private ConfigManager configManager;
        private IRunInfo runInfo;
        private IGetRandomUnit getRandomUnit;
        private IDatabaseConnection database;

        /**
         * Setup config for program in constructor
         */
        public Runner(String configFilePath)
        {
            configManager = new ConfigManager();
            runInfo = configManager.GetConfigFromFile(configFilePath);

            getRandomUnit = runInfo.GetRequestType();
            database = runInfo.GetDatabaseType();
        }

        /**
         * Start execution of program
         */
        public void run()
        {
            GetData();

            int actionId = GetNextAction();

            if (actionId == 1)
            {
                // Look at database and analyse results
                database.PrintDatabase();
                AnalyseData();
            }
        }

        private void GetData()
        {
            List<Task> taskList = new List<Task>();
            for (int i=0; i<10; i++)
            {
                Task tempTask = Task.Factory.StartNew(() => GetRandomUnit());
                taskList.Add(tempTask);

                Thread.Sleep(100);
                Console.WriteLine("i: " + i);
            }

            Task.WaitAll(taskList.ToArray());
            
        }

        private void GetRandomUnit()
        {
            String resStr = (new GetWebPage()).GetData(runInfo);

            database.AddToDatabase(DateTime.Now, resStr);
        }

        private int GetNextAction()
        {
            return 1;
        }

        private void AnalyseData()
        {

        }
    }
}
