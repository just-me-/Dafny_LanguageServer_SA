using DafnyLanguageServer.DafnyAdapter;
using System;
using System.Collections.Concurrent;

namespace DafnyLanguageServer
{
    public class BufferManager : IBufferManager
    {
        private ConcurrentDictionary<Uri, DafnyFile> _buffers = new ConcurrentDictionary<Uri, DafnyFile>();

        public void UpdateBuffer(Uri documentPath, string content)
        {
            DafnyFile file = GetOrCreateFileBuffer(documentPath);
            file.Sourcecode = content;
            file.DafnyHelper = new DafnyHelper(file.Filepath, file.Sourcecode);

            // do not update symboltable if current file state is invalid 
            var symboltable = new FileSymboltable(file.DafnyHelper);
            if(symboltable.HasEntries)
            {
                file.Symboltable = symboltable;
            }

            _buffers.AddOrUpdate(documentPath, file, (k, v) => file);
        }

        public void UpdateBuffer(DafnyFile file)
        {
            UpdateBuffer(file.Uri, file.Sourcecode);
        }

        private DafnyFile GetOrCreateFileBuffer(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : new DafnyFile { Uri = documentPath };
        }

        public DafnyFile GetFileFromBuffer(Uri documentPath)
        {
            return GetOrCreateFileBuffer(documentPath); 
        }
        public DafnyFile GetFileFromBuffer(string documentPath)
        {
            return GetFileFromBuffer(new Uri(documentPath));
        }

        public string GetTextFromBuffer(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer.Sourcecode : null;
        }

        public FileSymboltable GetSymboltableForFile(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer.Symboltable : null;
        }

        public ConcurrentDictionary<Uri, DafnyFile> GetAllFiles()
        {
            return _buffers; 
        }
    }
}
