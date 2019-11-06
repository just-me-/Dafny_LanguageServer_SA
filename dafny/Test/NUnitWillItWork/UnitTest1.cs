using System;
using System.IO;
using DafnyLanguageServer;
using Microsoft.Boogie;
using Microsoft.Dafny;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            ExecutionEngine.printer = new DafnyConsolePrinter();

            //var fstream = new FileStream("D:\\a.txt", FileMode.OpenOrCreate, FileAccess.Write);

            //var instream = GenerateStreamFromString("T�del!!");





            //var server = LanguageServer.PreInit(options =>
            //    options
            //        .WithInput(Console.OpenStandardInput())
            //        .WithOutput(Console.OpenStandardOutput())
            //        //.WithLoggerFactory(new LoggerFactory())
            //        //.AddDefaultLoggingProvider()
            //        //.WithMinimumLogLevel(LogLevel.Trace)
            //        //.WithServices(ConfigureServices)

            //        //.WithHandler<TextDocumentSyncHandler>()
            //        //.WithHandler<CompletionHandler>()
            //        .WithHandler<CompileHandler>()
            //);
            //server.SendNotification("hallo???");
        }
    }
}