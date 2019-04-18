using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface IGetRandomUnit
    {
        String GetData(IRunInfo runInfo);
    }
}
