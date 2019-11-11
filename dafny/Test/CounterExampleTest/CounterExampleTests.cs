using System.IO;
using DafnyLanguageServer;
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
            //string filename = Path.Combine(testPath, "fail.dfy");

            //string source = File.ReadAllText(filename);


            string filename = 
                "d:\\Eigene Dokumente\\VisualStudio\\SA\\dafny - server - redesign\\VSCodePlugin\\counterExampleExampleFile.dfy";
            string source =
                "method MultipleReturns(inp1: int, inp2: int) returns (more: int, less: int)\r\n   ensures less < inp1 < more\r\n{\r\n   more := inp1 + inp2;\r\n   less := inp1 - inp2;\r\n}";
            var service = new CounterExampleService(filename, source);
            var results = service.ProvideCounterExamples().Result;


            Assert.AreEqual(1, results.CounterExamples.Count);
        }
    }
}