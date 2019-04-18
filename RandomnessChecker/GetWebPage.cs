using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public class GetWebPage : IGetRandomUnit
    {
        public String GetData(IRunInfo runInfo)
        {
            String responseUrlString = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(runInfo.GetRequestString());
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    responseUrlString = response.ResponseUri.ToString();

                    Console.WriteLine(responseUrlString);
                }
            }
            catch (WebException we)
            {
                Console.WriteLine("Caught WebException" + we.Message);
                responseUrlString = null;
            }

            String resStr = runInfo.FormatResponse(responseUrlString);

            return resStr;
        }
    }
}
