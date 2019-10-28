using System;
using System.Diagnostics;
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
        public string DafnyFilePath { get; set; }
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

                string dafnyExe = @"G:\Dokumente\VisualStudio\SA\dafny-server-redesign\dafny\Binaries\Dafny.exe";
                string dafnyFile = request.DafnyFilePath;

                //To support spaces in path:
                dafnyFile = '\"' + request.DafnyFilePath + '\"';

                //string dafnyFile = "\"D:\\Eigene Dokumente\\VisualStudio\\SA\\dafny-server-redesign\\anExmapleDafnyFile.dfy\"";

                System.Diagnostics.Process process = new Process();
                process.StartInfo.FileName = dafnyExe;
                process.StartInfo.Arguments = "/compile:1 /nologo " + dafnyFile;
                process.EnableRaisingEvents = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                string processOut = "";
                process.OutputDataReceived += (sender, args) => processOut += args.Data;

                try
                {
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    return new CompilerResults
                    {
                        Error = true,
                        Message = "Internal Server Error: " + e.Message,
                        Executable = false
                    };
                }


                

                if (processOut.Contains("Compiled assembly into"))
                {
                    return new CompilerResults
                    {
                        Error = false,
                        Message = "Hat geklappt",
                        Executable = true
                    };
                }
                else
                {
                    return new CompilerResults
                    {
                        Error = true,
                        Message = "Das Programm hat noch Fehler",
                        Executable = false
                    };
                }


            });
        }
    }
}

/*
 * Konsolenout
 * 
 * 
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
