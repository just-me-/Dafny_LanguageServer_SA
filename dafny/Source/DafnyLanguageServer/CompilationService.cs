using System;
using System.Diagnostics;
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
                DafnyFile = '\"' + DafnyFile + '\"'; //Todo systemmethode

                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = DafnyExe,
                        Arguments = "/compile:1 /nologo " + DafnyFile,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    },
                    EnableRaisingEvents = true
                };

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
