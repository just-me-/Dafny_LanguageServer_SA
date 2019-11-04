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

        public void UpdateBuffer(Uri documentPath, string content)
        {
            _fileBuffers.AddOrUpdate(documentPath, content, (k, v) => content);
            // if file valid, add or update buffer, else tue nichts
            // was für den fall dass das file valid ist, die tabelle aber leer? also man leert das file sozusagen tatsächlich => testen! 
            var symboltable = new FileSymboltable(documentPath.ToString(), content);
            // symbols zu FileSymboltable casten... also alle Inhalte zu Einträgen pro File casten. Doppelte Infos kommen raus. etc. 
            _symboltableBuffers.AddOrUpdate(documentPath, symboltable, (k, v) => symboltable);
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
