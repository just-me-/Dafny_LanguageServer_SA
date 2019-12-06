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
        DafnyFile UpdateBuffer(Uri documentPath, string sourceCodeOfFile);
        DafnyFile GetFile(Uri documentPath);
        DafnyFile GetFile(string documentPath);
        string GetSourceCodeAsText(Uri documentPath);
        FileSymboltable GetSymboltable(Uri documentPath);
        ConcurrentDictionary<Uri, DafnyFile> GetAllFiles();
    }
}
