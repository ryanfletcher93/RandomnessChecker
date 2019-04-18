using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomnessChecker
{
    public interface IDatabaseConnection
    {
        void AddToDatabase();

        void IsInDatabase();
    }
}
