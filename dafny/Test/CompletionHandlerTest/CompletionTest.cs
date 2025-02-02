using DafnyLanguageServer.ContentManager;
using NUnit.Framework;

namespace CompletionHandlerTest
{
    public class CompletionTests
    {
        private FileSymboltable symbolTable = new FileSymboltable(new DafnyTranslationUnitFakeForCompletions());

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