using System;
using System.Collections.Concurrent;

namespace DafnyLanguageServer
{
    class BufferManager
    {
        // DafnyFile statt String? 
        private ConcurrentDictionary<Uri, string> _buffers = new ConcurrentDictionary<Uri, string>();

        public void UpdateBuffer(Uri documentPath, string content)
        {
            _buffers.AddOrUpdate(documentPath, content, (k, v) => content);
        }
        public void UpdateBuffer(DafnyFile file)
        {
            UpdateBuffer(file.Uri, file.Sourcecode);
        }

        public string GetTextFromBuffer(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }
    }
}
