using DafnyLanguageServer;
using DafnyLanguageServer.DafnyAdapter;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;

namespace CompletionHandlerTest
{
    public class CompletionTests
    {
        private FileSymboltable symbolTable = new FileSymboltable(new DafnyTranslationUnitMock());

        [Test]
        public void FullList()
        {
            var list = symbolTable.GetFullList();
            Assert.AreEqual(7, list.Count);
        }

        [Test]
        public void ListWithoutDuplicates()
        {
            var list = symbolTable.GetList();
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void ListForIdentifier()
        {
            var list = symbolTable.GetList("ClassA");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("aFunctionInClassA", list[0].Name);
        }

        [Test]
        public void GetParentForWord()
        {
            var parent = symbolTable.GetParentForWord("aFunctionInClassA");
            Assert.AreEqual("ClassA", parent);
        }

    }
}