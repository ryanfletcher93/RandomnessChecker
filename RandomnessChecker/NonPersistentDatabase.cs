using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    class NonPersistentDatabase : IDatabaseConnection
    {
        private readonly object syncLock = new object();

        private Dictionary<String, List<DateTime>> data;

        public NonPersistentDatabase()
        {
            data = new Dictionary<string, List<DateTime>>();
        }

        public void AddToDatabase(DateTime dateTime, String name)
        {
            lock (syncLock)
            {
                if (data.ContainsKey(name))
                {
                    data[name].Add(dateTime);
                }
                else
                {
                    List<DateTime> tempList = new List<DateTime>();
                    tempList.Add(dateTime);
                    data.Add(name, tempList);
                }
            }
        }

        public bool IsInDatabase(String name)
        {
            return data.ContainsKey(name);
        }

        public void PrintDatabase()
        {
            Console.WriteLine();
            Console.WriteLine("Start Database data ouput");
            Console.WriteLine("-------------------------");

            foreach (KeyValuePair<String, List<DateTime>> keyValuePair in data) {
                Console.WriteLine(keyValuePair.Key);
                foreach (DateTime dateTime in keyValuePair.Value)
                {
                    Console.WriteLine(dateTime);
                }
                Console.WriteLine();
            }
        }
    }
}
