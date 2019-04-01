using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            CRUDOperations app = new CRUDOperations();
            //var app = new QueryOperation();

            app.Run();
        }
    }
}
