using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public class CompilationService
    {
        private string DafnyExe { get; }
        private string DafnyFile { get; set; }

        public CompilationService(string exe, string file)
        {
            DafnyExe = exe;
            DafnyFile = file;
        }

        public async Task<CompilerResults> Compile()
        {
            return await Task.Run(() =>
            {
                //To support spaces in path:
                DafnyFile = '\"' + DafnyFile + '\"';

                Process process = new Process();
                process.StartInfo.FileName = DafnyExe;
                process.StartInfo.Arguments = "/compile:1 /nologo " + DafnyFile;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.EnableRaisingEvents = true;

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
                    const string pattern = "Error:? .*\n";
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
