using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    class ConfigManager
    {
        /**
         * 
         */
        public IRunInfo GetConfigFromFile(String filePath)
        {
            IRunInfo runInfo = new DefaultRunInfo();

            // Attempt to find config file if not provided
            if (filePath == "" || filePath == null)
            {

            }

            return runInfo;
        }
    }
}
