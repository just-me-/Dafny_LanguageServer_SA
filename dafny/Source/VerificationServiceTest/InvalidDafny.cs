using DafnyLanguageServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationServiceTest
{
    [TestClass]
    public class InvalidDafny
    {
        static readonly String dafnyCode = @" 
            method Test(x: int, y: int) returns (more: int, less: int)
                ensures less < x < more
            {
                more := x + y;
                less := x - y;
            }
        ";
        static readonly DafnyFile file = new DafnyLanguageServer.DafnyFile
        {
            Uri = null,
            Sourcecode = dafnyCode
        };

        [TestMethod]
        public void TestMethod1()
        {

            var verificationService = new VerificationService();
            var helper = verificationService.DafnyVerify(file);
            Assert.IsFalse(helper.Errors.Count == 0);
        }
    }
}
