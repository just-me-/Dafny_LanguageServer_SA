using Microsoft.Boogie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationServiceTest
{
    public class FakeErrorObject : ErrorInformation
    {
        public FakeErrorObject(IToken tok, string msg) : base(tok, msg)
        {
        }
    }
}
