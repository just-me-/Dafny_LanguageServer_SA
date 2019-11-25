using DafnyLanguageServer;
using DafnyLanguageServer.DafnyAdapter;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;

namespace CompletionHandlerTest
{
    public class CompletionTests
    {
        private static CompletionHandler completionHandler = new CompletionHandler(null, null);

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GeneratesCorrectCompletionResponse()
        {
            List<SymbolTable.SymbolInformation> symbols = new List<SymbolTable.SymbolInformation>();
            symbols.Add(new SymbolTable.SymbolInformation{ Name = "myFunction", Position = 0 });
            
            CompletionParams request = new CompletionParams { Position = { Line = 0, Character = 0 } };

            var result = completionHandler.ConvertListToCompletionresponse(symbols, request);

            Assert.AreEqual(result, null);
            
        }


        [Test]
        public void FileSymboltable()
        {
            // parentClass = symbols.GetParentForWord(word);
            //..
            Assert.Pass();
        }

    }
}