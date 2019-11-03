﻿using DafnyServer;
using Microsoft.Dafny;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DafnyLanguageServer
{
    class BufferManager
    {
        // DafnyFile statt String? 
        private ConcurrentDictionary<Uri, string> _fileBuffers = new ConcurrentDictionary<Uri, string>();
        private ConcurrentDictionary<Uri, FileSymboltable> _symboltableBuffers = new ConcurrentDictionary<Uri, FileSymboltable>();

        public void UpdateBuffer(Uri documentPath, string content)
        {
            _fileBuffers.AddOrUpdate(documentPath, content, (k, v) => content);
            // if file valid, add or update buffer, else tue nichts
            // was für den fall dass das file valid ist, die tabelle aber leer? also man leert das file sozusagen tatsächlich => testen! 
            var symbols = getSymbolList(documentPath.ToString(), content);
            // symbols zu FileSymboltable casten... also alle Inhalte zu Einträgen pro File casten. Doppelte Infos kommen raus. etc. 
            _symboltableBuffers.AddOrUpdate(documentPath, symbols, (k, v) => symbols);
        }

        public void UpdateBuffer(DafnyFile file)
        {
            UpdateBuffer(file.Uri, file.Sourcecode);
        }

        public string GetTextFromBuffer(Uri documentPath)
        {
            return _fileBuffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }

        private List<SymbolTable.SymbolInformation> getSymbolList(String documentPath, String code)
        {
            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, documentPath, code);
            return helper.Symbols();
        }
    }
}
