using DafnyLanguageServer;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        private readonly string filename = "<none>";

        [Test]
        public void Fail1()
        {
            string source = @"method MultipleReturns(inp1: int, inp2: int) returns (more: int, less: int)
   ensures less < inp1 < more
{
   more := inp1 + inp2;
   less := inp1 - inp2;
}";
            var service = new CounterExampleService(filename, source);
            var results = service.ProvideCounterExamples().Result;


            Assert.Pass();
        }
    }
}