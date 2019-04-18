using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface IRunInfo
    {
        int GetNumberOfRequests();

        int GetRequestsPerSecond();

        IGetRandomUnit GetRequestType();

        IDatabaseConnection GetDatabaseType();

        String GetRequestString();
    }
}
