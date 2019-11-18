using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public interface IBufferManager
    {
        void UpdateBuffer(Uri documentPath, string content);
        void UpdateBuffer(DafnyFile file);
        string GetTextFromBuffer(Uri documentPath);
        FileSymboltable GetSymboltableForFile(Uri documentPath);
        ConcurrentDictionary<Uri, DafnyFile> GetAllFiles();
    }
}
