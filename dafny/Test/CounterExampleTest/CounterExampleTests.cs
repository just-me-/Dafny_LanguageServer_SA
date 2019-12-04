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
using DafnyConsolePrinter = DafnyLanguageServer.DafnyAdapter.DafnyConsolePrinter;

namespace Tests
{

    class FakeTUForCounterExamples : IDafnyTranslationUnit
    {
        private CounterExamples counterExamples;
        public FakeTUForCounterExamples(CounterExamples counterExamples) => this.counterExamples = counterExamples;
        public FakeTUForCounterExamples(int col, int row, string[] vars, string[] vals)
        {
            CounterExamples fakeData = new CounterExamples();
            fakeData.Add(new CounterExampleProvider.CounterExample());
            var states = fakeData[0].States;
            //first two states are ignored
            states.Add(new CounterExampleProvider.CounterExampleState());
            states.Add(new CounterExampleProvider.CounterExampleState());


            states.Add(new CounterExampleProvider.CounterExampleState()); //Option: Support multiple states. Statt im CTOR ne methode machen "add CE mti diesen params"
            var state = fakeData[0].States[2];
            state.Column = col;
            state.Line = row;
            state.Variables = new List<CounterExampleProvider.CounterExampleVariable>();

            for (int i = 0; i < vars.Length; i++)
            {
                state.Variables.Add(new CounterExampleProvider.CounterExampleVariable
                {
                    //CanonicalName = "a",
                    Name = vars[i],
                    //RealName = "c",
                    Value = vals[i]
                });
            }

            counterExamples = fakeData;
        }

        public CounterExamples CounterExample() => counterExamples;

        public bool Verify() { throw new NotImplementedException(); }
        public List<ErrorInformation> GetErrors() { throw new NotImplementedException(); }
        public List<SymbolTable.SymbolInformation> Symbols() { throw new NotImplementedException(); }
        public void DotGraph() { throw new NotImplementedException();}
    } 

    public class UnitTests
    {


        [Test]
        public void FakeDataTestYaaay()
        {
            int col = 2;
            int row = 2;
            string[] vars = {"myVar"};
            string[] vals = { "myVal" };

            FakeTUForCounterExamples fakeTU = new FakeTUForCounterExamples(col, row, vars, vals);

            var service = new CounterExampleService(fakeTU);
            CounterExampleResults results = service.ProvideCounterExamples().Result;

            Assert.AreEqual(1, results.CounterExamples.Count);

            foreach (CounterExampleResult r in results.CounterExamples)
            {
                Assert.AreEqual(col, r.Col);
                Assert.AreEqual(row, r.Line);


                foreach (string var in vars)
                {
                    Assert.Contains(var, r.Variables.Keys);
                }

                for (int i = 0; i < r.Variables.Count; i++)
                {
                    Assert.AreEqual(vars[i], r.Variables.Keys.ToList()[i]);
                    Assert.AreEqual(vals[i], r.Variables[vars[i]]);
                }

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