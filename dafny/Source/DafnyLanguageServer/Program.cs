using System;
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
            //ExecutionEngine.printer = new DafnyConsolePrinter();


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

            await server.WaitForExit;
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BufferManager>();
            services.AddSingleton<NuGetAutoCompleteService>();

            

        }
    }
}