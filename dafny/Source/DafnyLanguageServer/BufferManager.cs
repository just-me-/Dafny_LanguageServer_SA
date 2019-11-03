using System;
using System.Collections.Concurrent;

namespace DafnyLanguageServer
{
    class BufferManager
    {
        // DafnyFile statt String? 
        private ConcurrentDictionary<Uri, string> _fileBuffers = new ConcurrentDictionary<Uri, string>();
        private ConcurrentDictionary<Uri, FileSymboltable> _SymboltableBuffers = new ConcurrentDictionary<Uri, FileSymboltable>();

        public void UpdateBuffer(Uri documentPath, string content)
        {
            // Ist das File valide? Symboltabelle updaten... ebenfalls im Buffer 'halten' 
            _fileBuffers.AddOrUpdate(documentPath, content, (k, v) => content);
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
