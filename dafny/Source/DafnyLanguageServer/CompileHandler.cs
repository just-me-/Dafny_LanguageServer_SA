using System.Threading;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace DafnyLanguageServer
{
    
    public class CompilerParams : IRequest<CompilerResults>
    {
        public ILanguageServerDocument Document { get; set; }   //noch nciht sicher was ich hier brauch. die anderen haben die "URI" mitgegeben.
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
            return await Task.Run(() =>
            {
                // Do someting to get a return value 
                return new CompilerResults
                {
                    Error = false,
                    Message = "Geiles Teil",
                    Executable = true
                };

            });
        }
    }
}


/* Commands.ts, Zeile 143:
 *     public compile(document: vscode.TextDocument | undefined, run: boolean = false): void {
        if (!document) {
            return; // Skip if user closed everything in the meantime
        }
        document.save();
        vscode.window.showInformationMessage(InfoMsg.CompilationStarted);

        this.languageServer.sendRequest<ICompilerResult>(LanguageServerRequest.Compile, document.uri)
        .then((result) => {
            vscode.window.showInformationMessage(InfoMsg.CompilationFinished);
            if (run && result.executable) {
                this.runner.run(document.fileName);
            } else if (run) {
                vscode.window.showErrorMessage(ErrorMsg.NoMainMethod);
            }
            return true;
        }, (error: any) => {
            vscode.window.showErrorMessage("Can't compile: " + error.message);
        });
    }


    */


/* server -> compile teil

     return new Promise<ICompilerResult>((resolve, reject) => {
            let executable = false;
const environment: Environment = new Environment(this.context.rootPath, this.notificationService, this.settings);
const dafnyCommand: Command = environment.getDafnyExe();

            const args = dafnyCommand.args;
args.push("/compile:1");
            args.push("/nologo");
            args.push(uri.fsPath);
            console.log(dafnyCommand.command + " " + args);
            const process = cp.spawn(dafnyCommand.command, args, environment.getStandardSpawnOptions());
process.on("exit", () => {
                resolve({ error: false, executable });
            });
            process.stdout.on("error", (data: Buffer) => {
                reject({ error: true, message: data.toString() });
            });
            process.stdout.on("data", (data: Buffer) => {
                const str = data.toString();

                if (str.toLowerCase().includes("error") && !str.toLowerCase().includes("0 errors")) {
                    reject({ error: true, message: str });
                }
                if (str.toLowerCase().indexOf(".exe") !== -1) {
                    executable = true;
                }

            });
        });


    */