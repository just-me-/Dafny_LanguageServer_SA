using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSPIntegrationTests
{
    public class Tests
    {

        private static readonly string assemblyPath = Path.GetDirectoryName(typeof(Tests).Assembly.Location);
        internal static readonly string serverExe = Path.GetFullPath(Path.Combine(assemblyPath, "../Binaries/DafnyLanguageServer.exe"));


        public static readonly ILoggerFactory LoggerFactory = new LoggerFactory();
            //.AddConsole()  //nur core? müsst eerilog blabla nutzen
            //.AddDeub();

        static readonly ILogger Log = LoggerFactory.CreateLogger(typeof(Tests));



        [Test]
        public void Ratata()
        {
            try
            {
                
                var cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(
                    TimeSpan.FromSeconds(30)
                );



                // Tell the client how to start and connect to the server. In this case, we're using STDIO (but you can also use named pipes or build your own transport).
                ServerProcess serverProcess = new StdioServerProcess(LoggerFactory, new ProcessStartInfo(serverExe)
                {
                    Arguments = "" // you may or may not need to pass any command-line arguments to the server
                });

                using (var client = new LanguageClient(LoggerFactory, serverProcess))
                {
                    Log.LogInformation("Initialising language server...");

                    // Tell the client to connect to the server and initialise it so it's ready to handle requests.
                    client.Initialize(
                        workspaceRoot: @"D:\jogl",
                        initializationOptions: new { }, // If the server requires initialisation options, you pass them here.
                        cancellationToken: cancellationSource.Token
                    ).Wait();

                    Log.LogInformation("Language server has been successfully initialised.");

                    // Make a request to the client.
                    // Note that, in LSP, line and column are 0-based.
                    Hover hover = client.TextDocument.Hover(
                        filePath: @"C:\\MyFile.xml",
                        line: 7,
                        column: 3,
                        cancellationToken: cancellationSource.Token
                    ).Result;

                    if (hover != null)
                    {
                        Log.LogInformation("Got hover info at ({StartPosition})-({EndPosition}): {HoverContent}",
                            $"{hover.Range.Start.Line},{hover.Range.Start.Character}",
                            $"{hover.Range.End.Line},{hover.Range.End.Character}",
                            hover.Contents
                        );
                    }
                    else
                        Log.LogWarning("No hover info available at ({Line}, {Column}).", 7, 3);

                    Log.LogInformation("Shutting down server...");
                    client.Shutdown().Wait();

                    Log.LogInformation("Waiting for server shutdown to complete...");
                    client.HasShutdown.Wait();

                    Log.LogInformation("Server shutdown is complete.");
                }
            }
            catch (Exception unexpectedError)
            {
                Log.LogError(unexpectedError, "Unexpected error: {ErrorMessage}", unexpectedError.Message);
            }
        }
    }
}