using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;

namespace TestFramework
{
    class Program
    {
        static void Main(string[] args)
        {

            ExecutionEngine.printer = new DafnyConsolePrinter();

            var filename = "<none>";
            var sourceIsFile = false;
            var args2 = new string[] { };
            var source = "method selftest() { assert 1==3; }";

            DafnyHelper helper = new DafnyHelper(args2, filename, source);

            bool isValid = helper.Verify();
            Console.WriteLine("uga uga *********************************************************: " + isValid);

            ErrorReporter reporter = helper.GetReporter;
            var errors = reporter.AllMessages;
            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
