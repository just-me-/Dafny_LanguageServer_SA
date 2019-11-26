using DafnyLanguageServer.DafnyAdapter;
using System;
using System.IO;

namespace DafnyLanguageServer
{
    public class DafnyFile
    {
        public Uri Uri { get; set; }
        public string Filepath
        {
            get
            {
                string s = Uri.LocalPath;
                s = s.Substring(1);
                return s.Replace('/', '\\');
            }

        }
        public string Sourcecode { get; set; }
        public FileSymboltable Symboltable { get; set; }
        public IDafnyHelper DafnyHelper { get; set; }
    }
}
