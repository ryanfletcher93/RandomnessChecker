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
        public String TableName { get; set; }

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

        public Dictionary<String, List<DateTime>> GetDataBetweenDates(DateTime dt1, DateTime dt2)
        {
            Dictionary<String, List<DateTime>> returnResults = new Dictionary<String, List<DateTime>>();
            foreach (KeyValuePair<String, List<DateTime>> keyValue in data)
            {
                foreach (DateTime time in keyValue.Value)
                {
                    if (time.CompareTo(dt1) > 0 && time.CompareTo(dt2) < 0)
                    {
                        AddToDictionary(returnResults, keyValue.Key, time);
                    }
                }
            }

            return returnResults;
        }

        public int GetNumberUniqueItems()
        {
            int result = 0;
            return result;
        }

        public int GetNumberTotalRecords()
        {
            int result = 0;
            return result;
        }

        public Dictionary<String, List<DateTime>> GetAllData()
        {
            return GetDataBetweenDates(DateTime.Now.AddYears(-10), DateTime.Now);
        }

        private void AddToDictionary(Dictionary<String, List<DateTime>> res, String key, DateTime value)
        {
            // Check if already in returnResults and add to existing or add new key
            if (res.ContainsKey(key))
            {
                res[key].Add(value);
            }
            else
            {
                List<DateTime> tempDateTime = new List<DateTime>();
                tempDateTime.Add(value);
                res.Add(key, tempDateTime);
            }
        }
    }
}
