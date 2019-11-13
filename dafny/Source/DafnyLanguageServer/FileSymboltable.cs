using DafnyServer;
using Microsoft.Dafny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public class FileSymboltable
    {
        private List<SymbolTable.SymbolInformation> _symbolTable;
        public bool HasEntries => (_symbolTable.Count > 0);

        public FileSymboltable(string uri, string content)
        {
            _symbolTable = RemoveDuplicates(GetSymbolList(uri, content));
        }

        public List<SymbolTable.SymbolInformation> GetList()
        {
            return _symbolTable;
        }

        public List<SymbolTable.SymbolInformation> GetList(string specificWord) //identifier 
        {
            if (specificWord is null)
            {
                return GetList();
            }
            var parentSymbol = GetSymbolByName(specificWord);
            return _symbolTable.Where(x => (x.ParentClass == specificWord && SymbolIsInRangeOf(x, parentSymbol))).ToList();
        }

        private SymbolTable.SymbolInformation GetSymbolByName(string name)
        {
            return _symbolTable.Where(x => (x.Name == name)).FirstOrDefault();
        }

        private bool SymbolIsInRangeOf(SymbolTable.SymbolInformation child, SymbolTable.SymbolInformation parent)
        {
            return FileHelper.ChildIsContainedByParent(
                child.Line, child.EndLine, child.Position, child.EndPosition,
                parent.Line, parent.EndLine, parent.Position, parent.EndPosition
            );
        }

        public string GetParentForWord(string word)
        {
            // 2do def programmieren 
            return word is null ? 
                null 
                : _symbolTable.Where(x => x.Name == word).FirstOrDefault().ParentClass;
        }

        private List<SymbolTable.SymbolInformation> GetSymbolList(String documentPath, String code)
        {
            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, documentPath, code);
            return helper.Symbols();
        }

        private List<SymbolTable.SymbolInformation> RemoveDuplicates(List<SymbolTable.SymbolInformation> list)
        {
            list = RemoveLeadingDashSymbols(list); // tmp? 2do 
            return list.GroupBy(x => x.Name).Select(x => x.First()).ToList();
        }
        // not sure if this is a good idea. Therefore its an isolated function 
        private List<SymbolTable.SymbolInformation> RemoveLeadingDashSymbols(List<SymbolTable.SymbolInformation> list)
        {
            var filteredList = list;
            var countRemoved = filteredList?.RemoveAll(x => x.Name.StartsWith("_"));
            return filteredList;
        }

    }
}
