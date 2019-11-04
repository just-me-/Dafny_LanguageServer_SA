using DafnyServer;
using Microsoft.Dafny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    class FileSymboltable
    {
        private List<SymbolTable.SymbolInformation> _symbolTable; 
        public bool HasEntries { get; }

        public FileSymboltable(string uri, string content)
        {
            var symboltable = getSymbolList(uri, content);
            if(symboltable.Count > 0)
            {
                _symbolTable = symboltable;
            } else
            {
                _symbolTable = new List<SymbolTable.SymbolInformation>(); 
            }
            HasEntries = (symboltable.Count > 0); 
        }

        public List<SymbolTable.SymbolInformation> getTmpList()
        {
            return _symbolTable; 
        }

        private List<SymbolTable.SymbolInformation> getSymbolList(String documentPath, String code)
        {
            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, documentPath, code);
            return helper.Symbols();
        }

    }
}
