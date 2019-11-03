using System;
using System.Collections.Concurrent;

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

        }

        public void UpdateBuffer(DafnyFile file)
        {
            UpdateBuffer(file.Uri, file.Sourcecode);
        }

        public string GetTextFromBuffer(Uri documentPath)
        {
            return _fileBuffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }
    }
}
