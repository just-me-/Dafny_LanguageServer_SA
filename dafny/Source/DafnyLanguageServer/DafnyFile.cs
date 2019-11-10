using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public class DafnyFile
    {
        public Uri Uri { get; set; }
        public string Filepath => Uri.ToString();
        public string Sourcecode { get; set; }
        public FileSymboltable Symboltable { get; set; }
        // 2Do: auch noch die aktuellen Error Informationen hinzutun 
    }
}
