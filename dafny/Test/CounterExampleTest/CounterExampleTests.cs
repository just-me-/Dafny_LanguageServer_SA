using System.IO;
using DafnyLanguageServer;
using Microsoft.Boogie;
using Microsoft.Dafny;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        private static readonly string assemblyPath = Path.GetDirectoryName(typeof(Tests).Assembly.Location);
        internal static readonly string testPath = Path.GetFullPath(Path.Combine(assemblyPath, "../CounterExampleFiles"));

        [Test]
        public void Fail1()
        {
            ExecutionEngine.printer = new DafnyConsolePrinter();
            string filename = Path.Combine(testPath, "fail.dfy");

            string source = File.ReadAllText(filename);


            var service = new CounterExampleService(filename, source);
            var results = service.ProvideCounterExamples().Result;


            Assert.AreEqual(2, results.CounterExamples.Count);
        }
    }
}