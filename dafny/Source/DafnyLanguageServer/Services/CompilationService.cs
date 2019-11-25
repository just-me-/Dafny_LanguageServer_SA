using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public class CompilationService
    {
        private string PathToDafnyDotExe { get; }
        private string _pathToDfy;
        private string PathToDfy
        {
            get => FileHelper.EscapeFilePath(_pathToDfy);
            set => _pathToDfy = value;
        }

        public CompilationService(string exe, string file)
        {
            PathToDafnyDotExe = exe;
            PathToDfy = file;
        }

        public async Task<CompilerResults> Compile()
        {
            return await Task.Run(() =>
            {
   
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = PathToDafnyDotExe,
                        Arguments = "/compile:1 /nologo " + PathToDfy,
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
