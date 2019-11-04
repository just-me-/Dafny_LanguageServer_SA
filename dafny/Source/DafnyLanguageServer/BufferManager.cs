using DafnyServer;
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
        // hmm auch noch error information hinzutun?
        // und dann die 3 arrays auslagern in eins vom typ "dafnyfile" und infos ins dafny file reinpacken? 

        public void UpdateBuffer(Uri documentPath, string content)
        {
            _fileBuffers.AddOrUpdate(documentPath, content, (k, v) => content);

            var symboltable = new FileSymboltable(documentPath.ToString(), content);
            if(symboltable.HasEntries)
            {
                _symboltableBuffers.AddOrUpdate(documentPath, symboltable, (k, v) => symboltable);
            }
        }

        public void UpdateBuffer(DafnyFile file)
        {
            UpdateBuffer(file.Uri, file.Sourcecode);
        }

        public string GetTextFromBuffer(Uri documentPath)
        {
            return _fileBuffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }

        public FileSymboltable GetSymboltableForFile(Uri documentPath)
        {
            return _symboltableBuffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }
        public ConcurrentDictionary<Uri, FileSymboltable> GetAllSymboltabls()
        {
            return _symboltableBuffers;
        }
    }
}
