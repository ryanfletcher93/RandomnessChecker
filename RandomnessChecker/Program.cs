using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    class Program
    {
        static void Main(String[] args)
        {
            // Check if specified config file in command line
            String configFilePath = "";
            if (args.Length == 2)
            {
                configFilePath = args[1];
            }

            Runner runner = new Runner(configFilePath);
            runner.run();


            Console.ReadKey();
        }
    }
}
