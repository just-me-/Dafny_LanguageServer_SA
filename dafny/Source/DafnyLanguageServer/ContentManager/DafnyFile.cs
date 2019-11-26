using DafnyLanguageServer.DafnyAdapter;
using System;

namespace DafnyLanguageServer
{
    public class DafnyFile
    {
        public Uri Uri { get; set; }
        public string Filepath => Uri.LocalPath;
        public string Sourcecode { get; set; }
        public FileSymboltable Symboltable { get; set; }
        public IDafnyHelper DafnyHelper { get; set; }
    }
}
