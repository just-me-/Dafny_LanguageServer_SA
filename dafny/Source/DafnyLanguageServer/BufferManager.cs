using System.Collections.Concurrent;

namespace DafnyLanguageServer
{
    class BufferManager
    {
        private ConcurrentDictionary<string, string> _buffers = new ConcurrentDictionary<string, string>();

        public void UpdateBuffer(string documentPath, string content)
        {
            _buffers.AddOrUpdate(documentPath, content, (k, v) => content);
        }

        public string GetTextFromBuffer(string documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }
    }
}
