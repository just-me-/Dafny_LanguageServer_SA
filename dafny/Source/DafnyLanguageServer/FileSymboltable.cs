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
            _symbolTable = removeDuplicates(getSymbolList(uri, content));
            HasEntries = (_symbolTable.Count > 0); 
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

        private List<SymbolTable.SymbolInformation> removeDuplicates(List<SymbolTable.SymbolInformation> list)
        {
            list = removeLeadingDashSymbols(list); // tmp?
            return list.GroupBy(x => x.Name).Select(x => x.First()).ToList();
        }
        // not sure if this is a good idea. Therefore its an isolated function 
        private List<SymbolTable.SymbolInformation> removeLeadingDashSymbols(List<SymbolTable.SymbolInformation> list)
        {
            var filteredList = list;
            var countRemoved = filteredList.RemoveAll(x => x.Name.StartsWith("_"));
            return filteredList;
        }

    }
}
