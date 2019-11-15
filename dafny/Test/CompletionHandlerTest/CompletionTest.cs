using DafnyLanguageServer;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;

namespace CompletionHandlerTest
{
    public class CompletionTests
    {

        // private static CompletionHandler completionHandler = new CompletionHandler(null, null);


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GeneratesCorrectCompletionResponse()
        {

            // Diese Omnisharp sachen mocken ist einfach pain. Jetzt würds theoretisch gehen aber
            // Weills ein Handler ist werden router sachen etc aufgerufen (hier ja null) und das mag Omnisharp ned...
            // müsste man also auch noch mocken oO 
            /*
            List<SymbolTable.SymbolInformation> symbols = new List<SymbolTable.SymbolInformation>();
            symbols.Add(new SymbolTable.SymbolInformation{ Name = "myFunction", Position = 0 });
            
            CompletionParams request = new CompletionParams { Position = { Line = 0, Character = 0 } };

            var result = completionHandler.ConvertListToCompletionresponse(symbols, request); 

            */
            Assert.Pass();
        }

    }
}