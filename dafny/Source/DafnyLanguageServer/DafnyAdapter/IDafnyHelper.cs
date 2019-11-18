using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafnyLanguageServer.DafnyAdapter
{
    public interface IDafnyHelper
    {
        bool Verify();
        List<SymbolTable.SymbolInformation> Symbols();
        List<CounterExampleProvider.CounterExample> CounterExample();
        void DotGraph();
    }
}
