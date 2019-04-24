using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    class DefaultRunInfo : IRunInfo
    {


        public int NumberOfRequests { get; set; }
        public String RequestString { get; set; }

        public IGetRandomUnit GetRequestType()
        {
            return new GetWebPage();
        }

        public IDatabaseConnection GetDatabaseType()
        {
            return new SQLDatabase();
        }

        public String FormatResponse(String response)
        {
            if (response == null)
            {
                return null;
            }

            String res = "";
            String[] splitResponse = response.Split('/');
            res = splitResponse[4];

            if (splitResponse.GetLength(0) < 5)
            {
                throw new Exception();
            }

            return res;
        }
    }
}
