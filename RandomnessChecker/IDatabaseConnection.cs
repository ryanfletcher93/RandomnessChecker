﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface IDatabaseConnection
    {
        // Table name is set separately to connection string
        String TableName { set; get; }

        void SetConnectionString(Dictionary<DatabaseParameter, String> databaseParams);
        void SetConnectionString(String connectionString);

        // Set 
        void ConnectToDatabase();
        bool CanConnect(Dictionary<DatabaseParameter, String> databaseParams, String tableName);

        void AddToDatabase(DateTime dateTime, String label);
        bool IsInDatabase(String label);

        void PrintDatabase();

        /** Gets number of unique items in database
         * e.g. if collecting url, this returns number of unique urls
         */
        int GetNumberUniqueItems();
        int GetNumberTotalRecords();

        Dictionary<String, List<DateTime>> GetDataBetweenDates(DateTime dateTime1, DateTime dateTime2);
        Dictionary<String, List<DateTime>> GetAllData();
    }
}
