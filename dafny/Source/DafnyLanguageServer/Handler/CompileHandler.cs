using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace DafnyLanguageServer
{
    public class CompilerParams : IRequest<CompilerResults>
    {
        public string DafnyFilePath { get; set; }
        public string DafnyExePath { get; set; }
    }

    public class CompilerResults
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public bool? Executable { get; set; }
    }

    [Serial, Method("compile")]
    public interface ICompile : IJsonRpcRequestHandler<CompilerParams, CompilerResults> { }

    public class CompileHandler : ICompile
    {
        public async Task<CompilerResults> Handle(CompilerParams request, CancellationToken cancellationToken)
        {
            CompilationService cs = new CompilationService(request.DafnyExePath, request.DafnyFilePath);
            return await cs.Compile();
        }
    }
}

/*
 * Example Konsolenout
 * 
    Dafny program verifier finished with 1 verified, 0 errors
Compiled assembly into _dfy.dll

********************************************************
_dfy_error.dfy(1,24): Error: assertion violation
Execution trace:
    (0,0): anon0

Dafny program verifier finished with 0 verified, 1 error

G:\Dokumente\VisualStudio\SA\dafny-server-redesign\dafny\Binaries>
    */
