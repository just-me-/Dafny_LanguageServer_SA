using System;

namespace DafnyLanguageServer
{
    public class DafnyFile
    {
        public Uri Uri { get; set; }
        public string Filepath => Uri.ToString();
        public string Sourcecode { get; set; }
        public FileSymboltable Symboltable { get; set; }
        // 2Do: eventuell auch noch die aktuellen Error Informationen hinzutun 
    }
}
