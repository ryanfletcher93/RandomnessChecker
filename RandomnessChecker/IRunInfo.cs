﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface IRunInfo
    {
        int NumberOfRequests { get; set; }
        String SqlTableName { get; set; }
        String RequestString { get; set; }
        String RegexString { get; set; }

        IGetRandomUnit GetRequestType();

        String FormatResponse(String str);
    }
}
