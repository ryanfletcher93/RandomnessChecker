using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface ISqlDatabaseConnection : IDatabaseConnection
    {
        String TableName { set; get; }

        void SetConnectionString(Dictionary<DatabaseParameter, String> databaseParams);
        void SetConnectionString(String connectionString);
        void ConnectToDatabase();

        bool CanConnect(Dictionary<DatabaseParameter, String> databaseParams, String tableName);
    }
}
