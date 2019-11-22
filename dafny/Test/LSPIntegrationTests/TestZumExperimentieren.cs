using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
//using DafnyLanguageServer;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Serilog;
using Serilog.Extensions.Logging;

namespace LSPIntegrationTests
{
    public class Tests
    {

        private static readonly string assemblyPath = Path.GetDirectoryName(typeof(Tests).Assembly.Location);
        internal static readonly string serverExe = Path.GetFullPath(Path.Combine(assemblyPath, "../Binaries/DafnyLanguageServer.exe"));
        internal static readonly string aDfyFile = Path.GetFullPath(Path.Combine(assemblyPath, "../Test/CounterExampleFiles/ok.dfy"));
        internal static readonly string workspaceDir = Path.GetFullPath(Path.Combine(assemblyPath, "../Test/CounterExampleFiles/"));




        [Test]
        public void DemoTest()
        {
            try
            {

                LoggerProviderCollection providers = new LoggerProviderCollection();

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.Providers(providers)
                    .CreateLogger();

                ILoggerFactory LoggerFactory = new SerilogLoggerFactory(Log.Logger);




                var cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(
                    TimeSpan.FromSeconds(30)
                );



                ServerProcess serverProcess = new StdioServerProcess(LoggerFactory, new ProcessStartInfo(serverExe)
                {
                    Arguments = "" // command-line arguments here
                });

                using (var client = new LanguageClient(LoggerFactory, serverProcess))
                {
                    Serilog.Log.Information("Initialising language server...");

                    // Tell the client to connect to the server and initialise it so it's ready to handle requests.
                    client.Initialize(
                        workspaceRoot: workspaceDir,
                        initializationOptions: new { }, // If the server requires initialisation options, you pass them here.
                        cancellationToken: cancellationSource.Token
                    ).Wait();

                    Log.Information("Language server has been successfully initialised.");

                    //var counterExampleParam = new CounterExampleParams
                    //{
                    //    DafnyFile = aDfyFile
                    //};  //can we do sth with this?

                    //client.TextDocument.DidOpen(aDfyFile, "dfy");

                    //CounterExampleResult res = client.SendRequest<CounterExampleResult>("counterExample", counterExampleParam, cancellationSource.Token).Result;

                    //Test result here for correctness

                    // Make a default LSP request to the client.
                    // Note that, in LSP, line and column are 0-based.
                    var completions = client.TextDocument.Completions(
                        filePath: aDfyFile,
                        line: 2,
                        column: 2,
                        cancellationToken: cancellationSource.Token
                    ).Result;

                    if (completions != null)
                    {
                        Log.Information("Got completion list" + completions);
                    }
                    else
                        Log.Warning("No hover info available at ({Line}, {Column}).", 7, 3);

                    Log.Information("Shutting down server...");
                    client.Shutdown().Wait();

                    Log.Information("Waiting for server shutdown to complete...");
                    client.HasShutdown.Wait();

                    Log.Information("Server shutdown is complete.");
                }
            }
            catch (Exception unexpectedError)
            {
                Log.Error(unexpectedError, "Unexpected error: {ErrorMessage}", unexpectedError.Message);
            }
        }
    }
}