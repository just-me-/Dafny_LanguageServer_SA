﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;

namespace DafnyLanguageServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExecutionEngine.printer = new DafnyConsolePrinter();

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithMinimumLogLevel(LogLevel.Trace)
                    .WithServices(ConfigureServices)

                    .WithHandler<TestHandler>()
                    .WithHandler<TextDocumentSyncHandler>()
                    .WithHandler<CompletionHandler>()
            );

            try
            {
                string toms_ego_pfad = @"D:\Eigene Dokumente\Desktop\MsgLogger.txt";
                string normaler_pfad = "./MsgLogger.txt";
                string path = normaler_pfad;
                using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    Console.SetOut(writer);
                    await server.WaitForExit;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open MsgLogger.txt for writing");
                Console.WriteLine(e.Message);
            }
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BufferManager>();
            services.AddSingleton<NuGetAutoCompleteService>();

            

        }
    }
}