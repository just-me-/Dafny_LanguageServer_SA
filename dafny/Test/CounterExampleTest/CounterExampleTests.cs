using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Fail1()
        {
            string Source = """method MultipleReturns(inp1: int, inp2: int) returns (more: int, less: int)
            ensures less<inp1 < more
            {
                more:= inp1 + inp2;
                less:= inp1 - inp2;
            }
            """;
            Assert.Pass();
        }
    }
}