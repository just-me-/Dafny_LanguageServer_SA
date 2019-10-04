using System.Collections.Concurrent;
using Microsoft.Language.Xml;

namespace Dafny_Server_Redesign.Server
{
    class BufferManager
    {
        private ConcurrentDictionary<string, string> _buffers = new ConcurrentDictionary<string, string>();

        public void UpdateBuffer(string documentPath, string content)
        {
            _buffers.AddOrUpdate(documentPath, content, (k, v) => content);
        }

        public string GetBuffer(string documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }
    }
}
