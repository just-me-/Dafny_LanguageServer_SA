﻿using System.Collections.Generic;
using System.Linq;
using DafnyHelper = DafnyLanguageServer.DafnyAdapter.DafnyHelper;
using SymbolTable = DafnyLanguageServer.DafnyAdapter.SymbolTable;

namespace DafnyLanguageServer
{
    public class FileSymboltable
    {
        private List<SymbolTable.SymbolInformation> _symbolTable;
        public bool HasEntries => (_symbolTable.Count > 0);

        public FileSymboltable(string uri, string content)
        {
            _symbolTable = GetSymbolList(uri, content);
        }

        public List<SymbolTable.SymbolInformation> GetFullList()
        {
            return RemoveConstructorSymbols(_symbolTable);

        }
        public List<SymbolTable.SymbolInformation> GetList()
        {
            return RemoveDuplicates(_symbolTable);
        }

        public List<SymbolTable.SymbolInformation> GetList(string specificWord) //identifier
        {
            if (specificWord is null)
            {
                return GetList();
            }
            var parentSymbol = GetSymbolByName(specificWord);
            return RemoveDuplicates(_symbolTable.Where(x => (x.ParentClass == specificWord  && SymbolIsInRangeOf(x, parentSymbol))).ToList());
        }

        private SymbolTable.SymbolInformation GetSymbolByName(string name)
        {
            return _symbolTable.FirstOrDefault(x => (x.Name == name));
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
            return word is null ? null : _symbolTable.FirstOrDefault(x => x.Name == word)?.ParentClass;
        }

        private List<SymbolTable.SymbolInformation> GetSymbolList(string documentPath, string code)
        {
            DafnyHelper helper = new DafnyHelper(documentPath, code);
            return helper.Symbols();
        }

        private List<SymbolTable.SymbolInformation> RemoveDuplicates(List<SymbolTable.SymbolInformation> list)
        {
            return RemoveConstructorSymbols(list).GroupBy(x => x.Name).Select(x => x.First()).ToList();
        }

        private List<SymbolTable.SymbolInformation> RemoveConstructorSymbols(List<SymbolTable.SymbolInformation> list)
        {
            var ignoredSymbols = new[] { "_ctor", "_default" };
            list?.RemoveAll(x => ignoredSymbols.Any(x.Name.Contains));
            return list;
        }

    }
}
