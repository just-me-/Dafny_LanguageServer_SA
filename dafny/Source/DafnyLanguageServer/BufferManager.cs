using DafnyServer;
using Microsoft.Dafny;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DafnyLanguageServer
{
    public class BufferManager
    {
        private ConcurrentDictionary<Uri, DafnyFile> _buffers = new ConcurrentDictionary<Uri, DafnyFile>();

        public void UpdateBuffer(Uri documentPath, string content)
        {
            DafnyFile file = getOrCreateFileBuffer(documentPath);
            file.Sourcecode = content; 

            // do not update symboltable if current file state is invalid 
            var symboltable = new FileSymboltable(documentPath.ToString(), content);
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

        private DafnyFile getOrCreateFileBuffer(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : new DafnyFile { Uri = documentPath };
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
