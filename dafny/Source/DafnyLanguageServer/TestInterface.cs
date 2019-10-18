using System.Threading;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace DafnyLanguageServer
{
    [Serial, Method("sayhello")]
    public interface ITestInterface : IJsonRpcRequestHandler<TestParams, TestResult> { }

    public class TestParams : IRequest<TestResult>
    {
        public string Text { get; set; }
    }

    public class TestResult
    {
        public string Text { get; set; }
    }

    public class TestHandler : ITestInterface
    {

        public async Task<TestResult> Handle(TestParams request, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                // Do someting to get a return value 
                return new TestResult
                {
                    Text = "SomeResult Amumumuh"
                };

            });
        }
    }
}
