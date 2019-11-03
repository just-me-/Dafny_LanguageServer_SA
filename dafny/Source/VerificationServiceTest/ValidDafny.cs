using Microsoft.VisualStudio.TestTools.UnitTesting;
using DafnyLanguageServer;
using System;

namespace VerificationServiceTest
{
    [TestClass]
    public class ValidDafny
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
        static readonly DafnyFile file = new DafnyLanguageServer.DafnyFile
        {
            Uri = new Uri("C://none"),
            Sourcecode = dafnyCode
        };

        [TestMethod]
        public void TestDafnyVerify()
        {
            /*
            DafnyHelper helper = new DafnyHelper(new string[] { }, "none", dafnyCode);
            if (!helper.Verify())
            {
                throw new ArgumentException("Failed to verify document."); //TODO: Während des schreibens ist das doc immer wieder invalid... exception ist etwas zu krass imho
            }
            */
            var verificationService = new VerificationService(); 
            var helper = verificationService.DafnyVerify(file);
            Assert.IsTrue(helper.Errors.Count == 0);
        }
    }
}
