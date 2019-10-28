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
            var helper = VerificationService.DafnyVerify(file);
            Assert.IsTrue(helper.Errors.Count == 0);
        }
    }
}
