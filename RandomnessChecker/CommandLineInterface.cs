using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RandomnessChecker
{
    class CommandLineInterface
    {
        private const String GatherDataOption = "Gather data";
        private const String AnalyseDataOption = "Analyse data";

        private static readonly String[] InitialOptionList = {
            GatherDataOption,
            AnalyseDataOption
        };

        public CommandLineInterface()
        {

        }

        public DatabaseType GetDatabaseType()
        {
            return DatabaseType.MySql;
        }

        public Dictionary<DatabaseParameter, String> GetDatabaseParams()
        {
            Dictionary<DatabaseParameter, String> paramDict = new Dictionary<DatabaseParameter, string>();

            Console.WriteLine("Please enter the database information below");
            Console.Write("Host: ");
            String hostVal = Console.ReadLine();
            paramDict.Add(DatabaseParameter.Host, hostVal);

            Console.Write("Port: ");
            String portVal = Console.ReadLine();
            paramDict.Add(DatabaseParameter.Port, portVal);

            Console.Write("Database: ");
            String databaseVal = Console.ReadLine();
            paramDict.Add(DatabaseParameter.Database, databaseVal);

            Console.Write("Username: ");
            String usernameVal = Console.ReadLine();
            paramDict.Add(DatabaseParameter.Username, usernameVal);

            Console.Write("Passowrd: ");
            String passwordVal = Console.ReadLine();
            paramDict.Add(DatabaseParameter.Password, passwordVal);

            return paramDict;
        }

        public String GetFilePath()
        {
            Console.WriteLine("Enter the Config.txt file path: ");
            String filePath = Console.ReadLine();
            return filePath;
        }

        public Operation GetAction()
        {
            Operation resultOperation = Operation.Invalid;

            Console.WriteLine("Select one of the following actions");
            Console.WriteLine("1. Gather data");
            Console.WriteLine("2. Report on current data integrity");
            Console.WriteLine("3. Generate randomness report");
            Console.WriteLine("4. Quit");

            while (resultOperation == Operation.Invalid)
            {
                String inputLine = Console.ReadLine();
                char enteredChar = '\0';
                if (inputLine.Length == 1)
                {
                    enteredChar = Convert.ToChar(inputLine);
                }

                if (enteredChar == '1')
                {
                    resultOperation = Operation.GatherData;
                }
                else if (enteredChar == '2')
                {
                    resultOperation = Operation.ReportOnDataIntegrity;
                }
                else if (enteredChar == '3')
                {
                    resultOperation = Operation.GenerateReport;
                }
                else if (enteredChar == '4')
                {
                    resultOperation = Operation.Quit;
                }
                else
                {
                    Console.WriteLine("Invalid entry, please enter again");
                    resultOperation = Operation.Invalid;
                }
            }

            return resultOperation;
        }

        public String GetTableName()
        {
            Console.Write("Table: ");
            String tableVal = Console.ReadLine();
            return tableVal;
        }

        public String GetRandomiserUrl()
        {
            Console.Write("Enter the randomizer url: ");
            String url = Console.ReadLine();
            return url;
        }

        public String GetRegexForReturnUrl()
        {
            Console.Write("Enter the regex to be applied to return url: ");
            String regex = Console.ReadLine();
            return regex;
        }

        public bool ConfirmSelection()
        {
            Console.WriteLine();
            Console.WriteLine("Are these the parameters you want to continue with? (Y/n)");

            String res;
            bool result = false;
            while (true)
            {
                res = Console.ReadLine();
                if (res == "Y")
                {
                    result = true;
                    break;
                }
                else if (res == "n")
                {
                    result = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid entry, please try again:");
                }
            }

            return result;
        }

        public int GetNumberOfRequests()
        {
            Console.Write("Enter the number of requests: ");
            int numRequests = -1;
            bool isNumber = false;
            bool isPositiveInt = false;

            while (true)
            {
                String responseStr = Console.ReadLine();
                isNumber = Int32.TryParse(responseStr, out numRequests);
                isPositiveInt = numRequests > 0;

                if (isPositiveInt && isNumber)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid number, please enter again");
                }
            }
            
            return numRequests;
        }
    }
}
