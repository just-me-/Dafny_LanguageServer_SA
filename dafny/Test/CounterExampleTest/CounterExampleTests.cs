using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DafnyLanguageServer;
using DafnyLanguageServer.DafnyAdapter;
using Microsoft.Boogie;
using Microsoft.Dafny;
using NUnit.Framework;
using CounterExamples = System.Collections.Generic.List<DafnyLanguageServer.DafnyAdapter.CounterExampleProvider.CounterExample>;
using CounterExample = DafnyLanguageServer.DafnyAdapter.CounterExampleProvider.CounterExample;
using CounterExampleState = DafnyLanguageServer.DafnyAdapter.CounterExampleProvider.CounterExampleState;
using CounterExampleVariable = DafnyLanguageServer.DafnyAdapter.CounterExampleProvider.CounterExampleVariable;
using DafnyConsolePrinter = DafnyLanguageServer.DafnyAdapter.DafnyConsolePrinter;

namespace Tests
{

    class FakeDafnyTranslationUnitForCounterExamples : IDafnyTranslationUnit
    {
        private readonly CounterExamples counterExamples = new CounterExamples();
        public CounterExamples CounterExample() => counterExamples;

        public FakeDafnyTranslationUnitForCounterExamples()
        {
            counterExamples.Add(new CounterExample());
            var states = counterExamples[0].States;
            //first two states are ignored as-is by Dafny Provider
            states.Add(new CounterExampleState());
            states.Add(new CounterExampleState());
        }

        public void AddCounterExampleToFake(int col, int row, string[] vars, string[] vals)
        {
            var states = counterExamples[0].States;

            states.Add(new CounterExampleState());
            var state = counterExamples[0].States.Last();
            state.Column = col;
            state.Line = row;
            state.Variables = new List<CounterExampleVariable>();

            for (int i = 0; i < vars.Length; i++)
            {
                state.Variables.Add(new CounterExampleVariable
                {
                    //CanonicalName = "a",  //Field not used, add here if used in future tests
                    Name = vars[i],
                    //RealName = "c",       //Field not used, add here if used in future tests
                    Value = vals[i]
                });
            }

        }

        public bool Verify() { throw new NotImplementedException(); }
        public List<ErrorInformation> GetErrors() { throw new NotImplementedException(); }
        public List<SymbolTable.SymbolInformation> Symbols() { throw new NotImplementedException(); }
        public void DotGraph() { throw new NotImplementedException();}
    } 

    public class UnitTests
    {
        [Test]
        public void SingleCounterExample()
        {
            const int col = 2;
            const int row = 2;
            string[] vars = { "myVar" };
            string[] vals = { "myVal" };

            FakeDafnyTranslationUnitForCounterExamples fake = new FakeDafnyTranslationUnitForCounterExamples();
            fake.AddCounterExampleToFake(col, row, vars, vals);

            var service = new CounterExampleService(fake);
            CounterExampleResults results = service.ProvideCounterExamples().Result;

            Assert.AreEqual(1, results.CounterExamples.Count, $"Counter Example should only contain 1 counter examples.");

            CompareCounterExampleWithExpectation(results.CounterExamples[0], col, row, vars, vals);
        }


        [Test]
        public void TwoCounterExample()
        {
            const int col1 = 2;
            const int row1 = 2;
            string[] vars1 = { "myVar" };
            string[] vals1 = { "myVal" };


            const int col2 = 2;
            const int row2 = 2;
            string[] vars2 = { "myVar" };
            string[] vals2 = { "myVal" };

            FakeDafnyTranslationUnitForCounterExamples fake = new FakeDafnyTranslationUnitForCounterExamples();
            fake.AddCounterExampleToFake(col1, row1, vars1, vals1);
            fake.AddCounterExampleToFake(col2, row2, vars2, vals2);

            var service = new CounterExampleService(fake);
            CounterExampleResults results = service.ProvideCounterExamples().Result;

            Assert.AreEqual(2, results.CounterExamples.Count, $"Counter Example should only contain 2 counter examples.");

            CompareCounterExampleWithExpectation(results.CounterExamples[0], col1, row1, vars1, vals1);
            CompareCounterExampleWithExpectation(results.CounterExamples[1], col2, row2, vars2, vals2);
        }

        private static void CompareCounterExampleWithExpectation(CounterExampleResult r, int col, int row, string[] vars, string[] vals)
        {
            Assert.AreEqual(col, r.Col, "A column index is wrong in the provided counter example");
            Assert.AreEqual(row, r.Line, "A line (row) index is wrong in the provided counter example");

            foreach (string var in vars)
            {
                Assert.Contains(var, r.Variables.Keys, $"The key {var} is not provided in the counter examples.");
            }

            //TODO Dict order is non deterministic. Cant test like this.
            var resultKeys = r.Variables.Keys.ToList();
            for (int i = 0; i < r.Variables.Count; i++)
            {
                Assert.AreEqual(vars[i], resultKeys[i], $"The provided counter example is not containing key {vars[i]} at the expected position.");
                Assert.AreEqual(vals[i], r.Variables[vars[i]], $"Value to Key {vars[i]} is not as expected.");
            }
        }
    }
    public class IntegrationTests
    {
        private static readonly string assemblyPath = Path.GetDirectoryName(typeof(IntegrationTests).Assembly.Location);
        internal static readonly string testPath = Path.GetFullPath(Path.Combine(assemblyPath, "../Test/CounterExampleFiles"));
        
        private CounterExampleResults ProvideCounterExamples(string filename)
        {
            ExecutionEngine.printer = new DafnyConsolePrinter();
            string fullFilePath = Path.Combine(testPath, filename);
            string source = File.ReadAllText(fullFilePath);
            DafnyTranslationUnit h = new DafnyTranslationUnit(fullFilePath, source);
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

        [Test]
        public void InvalidFileName()
        {
            var ex = Assert.Throws<AggregateException>(() =>
            {
                ExecutionEngine.printer = new DafnyConsolePrinter();
                string fullFilePath = Path.Combine(testPath, "idonotexist.dfy");
                string source = "method a(){}";
                DafnyTranslationUnit h = new DafnyTranslationUnit(fullFilePath, source);
                var service = new CounterExampleService(h);
                var _ = service.ProvideCounterExamples().Result;
            });
            Assert.That(ex.InnerException, Is.TypeOf(typeof(FileNotFoundException)));
        }
    }
}