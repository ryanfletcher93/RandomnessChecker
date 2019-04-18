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

            // For the config number or times, request a page
            for (int i = 0; i < runInfo.GetNumberOfRequests(); i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(runInfo.GetRequestString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    responseUrlString = response.ResponseUri.ToString();

                    Console.WriteLine(responseUrlString);
                    Console.ReadKey();  
                }
                catch (WebException we)
                {
                    Console.WriteLine("Caught WebException" + we.Message);
                    responseUrlString = null;
                }
            }

            return responseUrlString;
        }
    }
}
