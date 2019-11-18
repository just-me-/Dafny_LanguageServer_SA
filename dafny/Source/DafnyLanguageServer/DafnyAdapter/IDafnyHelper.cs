using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Boogie;

namespace DafnyLanguageServer.DafnyAdapter
{
    public interface IDafnyHelper
    {
        List<ErrorInformation> Errors { get; }

        bool Verify();
        List<SymbolTable.SymbolInformation> Symbols();
        List<CounterExampleProvider.CounterExample> CounterExample();
        void DotGraph();
    }
}
