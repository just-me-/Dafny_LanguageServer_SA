using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Boogie;

namespace DafnyLanguageServer.DafnyAdapter
{
    class DafnyHelperMock : IDafnyHelper
    {
        public bool IsNice = true;

        public DafnyHelperMock(bool isnice)
        {
            IsNice = isnice;
        }

        public List<ErrorInformation> Errors { get; } = new List<ErrorInformation>();


        public bool Verify()
        {
            return IsNice;
        }

        public List<SymbolTable.SymbolInformation> Symbols()
        {
            List<SymbolTable.SymbolInformation> symbols = new List<SymbolTable.SymbolInformation>();
            symbols.Add(new SymbolTable.SymbolInformation { Name = "myFunction", Position = 0 });
            symbols.Add(new SymbolTable.SymbolInformation { Name = "myOtherFunction", Position = 1 });
            return symbols;
        }

        public List<CounterExampleProvider.CounterExample> CounterExample()
        {
            List<CounterExampleProvider.CounterExample> ces = new List<CounterExampleProvider.CounterExample>();
            CounterExampleProvider.CounterExample ce = new CounterExampleProvider.CounterExample();
            ces.Add(ce);

            return ces;
        }

        public void DotGraph()
        {
            throw new NotImplementedException();
        }

        public List<ErrorInformation> GetErrors()
        {
            return null;
        }
    }
}
