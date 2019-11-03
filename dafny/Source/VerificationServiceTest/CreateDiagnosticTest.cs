using System;
using DafnyLanguageServer;
using Microsoft.Boogie;
using Microsoft.Dafny;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VerificationServiceTest
{

    public class FakeErrorObject : ErrorInformation
    {
        public FakeErrorObject(IToken tok, string msg) : base(tok, msg)
        {
        }
    }

    [TestClass]
    public class CreateDiagnosticTest
    {

        [TestMethod]
        public void TestDiagnosticOneError()
        {
            var verificationService = new VerificationService(null);

            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, null, null);

            // faka data... 
            var token = new Token();
            token.filename = "FakedFile";
            token.val = "Dies ist eigentlich kein Fehler";
            token.kind = token.pos = token.line = token.col = token.line = 3;
            

            // Dieses Mist hat keinen public Konstruktor ... ausserhhalb von Boogie. Will der micht den total v___-.. 
            var info = new FakeErrorObject(token, "Msg"); 
            helper.Errors.Add(info); 

            //  foreach (ErrorInformation e in helper.Errors)
            // for (int i = 0; i < e.Aux.Count - 1; i++) //ignore last element (trace)

            var diagnostics = verificationService.CreateDafnyDiagnostics(helper.Errors, token.filename);
            // now compare 

            Assert.AreEqual(diagnostics.Count, 1);
            Assert.AreEqual(diagnostics[0].Source, token.filename);
        }
    }
}
