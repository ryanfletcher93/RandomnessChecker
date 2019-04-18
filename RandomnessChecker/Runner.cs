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
            getRandomUnit = new GetWebPage();
            getRandomUnit.GetData(runInfo);
        }
    }
}
