using System;
using System.Collections.Generic;
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

            var token = new Token();
            token.filename = "FakedFile";
            token.val = "Dies ist eigentlich kein Fehler";
            token.kind = token.pos = token.line = token.col = token.line = 3;

            var errors = new List<FakeErrorObject>();
            var info = new FakeErrorObject(token, "Msg");
            errors.Add(info); 

            //  foreach (ErrorInformation e in helper.Errors)
            // for (int i = 0; i < e.Aux.Count - 1; i++) //ignore last element (trace)

            var diagnostics = verificationService.CreateDafnyDiagnostics(errors, token.filename);

            Assert.AreEqual(diagnostics.Count, 1);
            Assert.AreEqual(diagnostics[0].Source, token.filename);
        }
    }
}
