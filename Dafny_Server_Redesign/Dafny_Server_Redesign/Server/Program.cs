using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using System;
using System.Threading.Tasks;

namespace Dafny_Server_Redesign.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithMinimumLogLevel(LogLevel.Trace)
                    .WithServices(ConfigureServices)

                    //.WithHandler<TextDocumentHandler>() // 3 Klassen aus dem OmniSharp Tut
                    //.WithHandler<DidChangeWatchedFilesHandler>()
                    //.WithHandler<FoldingRangeHandler>()

                    

                    .WithHandler<TextDocumentSyncHandler>()
                    .WithHandler<CompletionHandler>()
            );

            await server.WaitForExit;
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BufferManager>();
            services.AddSingleton<NuGetAutoCompleteService>();

            services.AddSingleton<LspReciever>();
            //services.AddSingleton<ReciverHandler1>();

        }
    }
}