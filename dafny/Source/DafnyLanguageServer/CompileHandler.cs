using System;
using System.Diagnostics;
using System.IO.Packaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

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

            string dafnyExe = request.DafnyExePath;
            string dafnyFile = request.DafnyFilePath;

            return await Compile(dafnyExe, dafnyFile);
        }





        public static async Task<CompilerResults> Compile(string dafnyExe, string dafnyFile)
        {
            return await Task.Run(() =>
            {

                //To support spaces in path:
                dafnyFile = '\"' + dafnyFile + '\"';

                System.Diagnostics.Process process = new Process();
                process.StartInfo.FileName = dafnyExe;
                process.StartInfo.Arguments = "/compile:1 /nologo " + dafnyFile;
                process.EnableRaisingEvents = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                string processOut = "";
                process.OutputDataReceived += (sender, args) => processOut += args.Data + "\n";

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

                if (processOut.Contains("Compiled assembly into") && processOut.Contains(".exe"))
                {
                    return new CompilerResults
                    {
                        Error = false,
                        Message = "Compilation successful",
                        Executable = true
                    };
                }
                else if (processOut.Contains("Compiled assembly into"))
                {
                    return new CompilerResults
                    {
                        Error = false,
                        Message = "Compilation successful",
                        Executable = false
                    };
                }
                else
                {
                    string pattern = "Error:? .*\n";
                    Match m = Regex.Match(processOut, pattern);

                    return new CompilerResults
                    {
                        Error = true,
                        Message = "Compilation failed: " + m.Value,
                        Executable = false
                    };
                }
            });
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
