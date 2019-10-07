using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;


namespace Dafny_Server_Redesign.Server
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
        private readonly ILogger _logger;

        public TestHandler(ILoggerFactory factory)
        {
            // _logger = factory.CreateLogger<ExpandAliasHandler>();
        }

        public async Task<TestResult> Handle(TestParams request, CancellationToken cancellationToken)
        {
            // Do someting to get a return value 
            return new TestResult
            {
                Text = "SomeResult" 
            };
        }
    }
}
