using System.IO;
using DafnyLanguageServer;
using DafnyLanguageServer.DafnyAdapter;
using Microsoft.Boogie;
using Microsoft.Dafny;
using NUnit.Framework;
using DafnyConsolePrinter = DafnyLanguageServer.DafnyAdapter.DafnyConsolePrinter;

namespace Tests
{
    public class Tests
    {


        private static readonly string assemblyPath = Path.GetDirectoryName(typeof(Tests).Assembly.Location);
        internal static readonly string testPath = Path.GetFullPath(Path.Combine(assemblyPath, "../Test/CounterExampleFiles"));
        
        private CounterExampleResults ProvideCounterExamples(string filename)
        {
            ExecutionEngine.printer = new DafnyConsolePrinter();
            string fullFilePath = Path.Combine(testPath, filename);
            string source = File.ReadAllText(fullFilePath);
            DafnyHelper h = new DafnyHelper(fullFilePath, source);
            var service = new CounterExampleService(h);
            return service.ProvideCounterExamples().Result;
        }

        [Test]
        public void Fail1()
        {
            var results = ProvideCounterExamples("fail1.dfy");
            Assert.AreEqual(1, results.CounterExamples.Count);
        }

        [Test]
        public void Fail2()
        {
            var results = ProvideCounterExamples("fail2.dfy");
            Assert.AreEqual(2, results.CounterExamples.Count);
        }


        [Test]
        public void TwoMethods()
        {
            var results = ProvideCounterExamples("twoMethods.dfy");
            Assert.AreEqual(1, results.CounterExamples.Count);
        }

        [Test]
        public void Ok()
        {
            var results = ProvideCounterExamples("ok.dfy");
            Assert.AreEqual(0, results.CounterExamples.Count);
        }
        
    }
}