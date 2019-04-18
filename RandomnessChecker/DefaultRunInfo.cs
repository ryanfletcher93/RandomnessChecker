using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    class DefaultRunInfo : IRunInfo
    {
        public int GetNumberOfRequests()
        {
            return 1;
        }


        public int GetRequestsPerSecond()
        {
            return 1;
        }


        public String GetRequestString()
        {
            return "http://www.reddit.com/r/random/";
        }


        public IGetRandomUnit GetRequestType()
        {
            return new GetWebPage();
        }

        public IDatabaseConnection GetDatabaseType()
        {
            return new NonPersistentDatabase();
        }
    }
}
