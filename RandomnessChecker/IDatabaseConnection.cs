using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface IDatabaseConnection
    {
        void AddToDatabase(DateTime dateTime, String label);

        bool IsInDatabase(String label);

        void PrintDatabase();
    }
}
