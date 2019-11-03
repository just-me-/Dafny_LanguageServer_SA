using System;
using DafnyLanguageServer;
using Microsoft.Boogie;
using Microsoft.Dafny;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VerificationServiceTest
{
    [TestClass]
    public class CreateDiagnosticTest
    {
        static readonly String dafnyCode = @" 
            method Test(x: int, y: int) returns (more: int, less: int)
                requires 0 < y
                ensures less < x < more
            {
                more := x + y;
                less := x - y;
            }
        ";
        static readonly DafnyLanguageServer.DafnyFile file = new DafnyLanguageServer.DafnyFile
        {
            Uri = null,
            Sourcecode = dafnyCode
        };

        [TestMethod]
        public void TestDiagnosticNoErrors()
        {
            var verificationService = new VerificationService();

            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, file.Filepath, file.Sourcecode);

            // faka data... 
            var token = new Token();
            token.filename = "FakedFile";
            token.val = "Dies ist eigentlich kein Fehler";
            token.kind = token.pos = token.line = token.col = token.line = 3;
            

            // Dieses Mist hat keinen public Konstruktor ... ausserhhalb von Boogie. Will der micht den total v___-.. 
            var info = new ErrorInformation(token, "Msg"); 
            helper.Errors.Add(info); 

            //  foreach (ErrorInformation e in helper.Errors)
            // for (int i = 0; i < e.Aux.Count - 1; i++) //ignore last element (trace)

            var diagnostics = verificationService.CreateDafnyDiagnostics(helper, file);
            // now compare 


        }
    }
}
