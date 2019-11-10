using DafnyLanguageServer;
using NUnit.Framework;

namespace CompletionHandlerTest
{
    public class FileHelperTests
    {
        [Test]
        public void IsInRange()
        {
            Assert.IsTrue(FileHelper.ChildIsContainedByParent(1, 1, 0, 0, 0, 2, 0, 0));
        }

        [Test]
        public void IsNotInRange()
        {
            Assert.IsFalse(FileHelper.ChildIsContainedByParent(0, 0, 0, 0, 1, 1, 0, 0));
        }

        [Test]
        public void IsInRange_SameLine()
        {
            Assert.IsTrue(FileHelper.ChildIsContainedByParent(0, 0, 2, 3, 0, 0, 1, 4));
        }

        [Test]
        public void IsNotInRange_SameLine()
        {
            Assert.IsFalse(FileHelper.ChildIsContainedByParent(0, 0, 1, 2, 0, 0, 3, 4));
        }

        //FileHandler.GetCurrentWord(); 

        [Test]
        public void getWord()
        {
            var code = @"method Main() {
                        var a := 1 + 2;
                        var acc2 := new C();
                        var acc3.             
            }";
            var acc3 = FileHelper.GetCurrentWord(code, 4-1, 33);
            Assert.AreEqual("acc3", acc3);
        }

        [Test]
        public void getWordOfSimpleLine()
        {
            var code = "myWord.";
            var myWord = FileHelper.GetCurrentWord(code, 0, 7);
            Assert.AreEqual("myWord", myWord);
        }

    }
}