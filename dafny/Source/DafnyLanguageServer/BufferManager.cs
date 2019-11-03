using System;
using System.Collections.Concurrent;

namespace DafnyLanguageServer
{
    class BufferObject
    {
        public string Code { get; set; }
        public FileSymboltable symboltable { get; set; }
    }
    class BufferManager
    {
        // DafnyFile statt String? 
        private ConcurrentDictionary<Uri, BufferObject> _fileBuffers = new ConcurrentDictionary<Uri, BufferObject>();

        public void UpdateBuffer(Uri documentPath, string code)
        {
            // Ist das File valide? Symboltabelle updaten... ebenfalls im Buffer 'halten' 
            _fileBuffers.AddOrUpdate(documentPath, code, (k, v) => BufferObject.Code);
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
