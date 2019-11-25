using System;
using DafnyLanguageServer;
using NUnit.Framework;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace FileHelperTest
{
    internal class ParentChildTests
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

        [Test]
        public void GetWord()
        {
            var code = @"method Main() {
                        var a := 1 + 2;
                        var acc2 := new C();
                        var acc3.             
            }";
            var acc3 = FileHelper.GetCurrentWord(code, 4 - 1, 33);
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

    internal class PositionalTests
    {
        [Test]
        public void GetAValidPosition1()
        {
            const int line = 5;
            const int chr1 = 10;

            var pos = FileHelper.CreatePosition(line, chr1);
            Assert.AreEqual(new Position(line, chr1), pos);
        }

        [Test]
        public void GetInvalidPosition1()
        {
            const int line = -5;
            const int chr = 10;

            Assert.Throws<ArgumentException>(() => FileHelper.CreatePosition(line, chr));
        }

        [Test]
        public void GetInvalidPosition2()
        {
            const int line = 5;
            const int chr = -10;

            Assert.Throws<ArgumentException>(() => FileHelper.CreatePosition(line, chr));
        }

        [Test]
        public void GetAValidRange()
        {
            const int line = 5;
            const int chr1 = 10;
            const int chr2 = 15;
            const int len = chr2 - chr1;

            var range = FileHelper.CreateRange(line, chr1, len);
            Assert.AreEqual(new Position(line, chr1), range.Start);
            Assert.AreEqual(new Position(line, chr2), range.End);
        }

        [Test]
        public void LowerPositionIsFirstAkaNegativeRange()
        {
            const int line = 5;
            const int chr1 = 10;
            const int chr2 = 5;
            const int len = chr2 - chr1;

            var range = FileHelper.CreateRange(line, chr1, len);
            Assert.AreEqual(new Position(line, chr2), range.Start);
            Assert.AreEqual(new Position(line, chr1), range.End);
        }

        [Test]
        public void GetAnInvalidRange1()
        {
            const int line = -2;
            const int chr1 = 10;
            const int chr2 = 15;
            const int len = chr2 - chr1;

            Assert.Throws<ArgumentException>(() => FileHelper.CreateRange(line, chr1, len));
        }

        [Test]
        public void GetAnInvalidRange2()
        {
            const int line = 5;
            const int chr1 = -10;
            const int chr2 = 15;
            const int len = chr2 - chr1;

            Assert.Throws<ArgumentException>(() => FileHelper.CreateRange(line, chr1, len));
        }
    }

    internal class LineLengthTests
    {
        [Test]
        public void SimpleTest1()
        {
            const string s = "a\nabc\na";
            const int l = 1;

            int result = FileHelper.GetLineLength(s, l);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void SimpleTest2()
        {
            const string s = "a\nabc\na";
            const int l = 0;

            int result = FileHelper.GetLineLength(s, l);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void LineTooLarge()
        {
            const string s = "a\nabc\na";
            const int l = 3;

            Assert.Throws<ArgumentException>(() => FileHelper.GetLineLength(s, l));
        }

        [Test]
        public void LineNegativeThrows()
        {
            const string s = "a\nabc\na";
            const int l = -1;

            Assert.Throws<ArgumentException>(() => FileHelper.GetLineLength(s, l));
        }
    }


    internal class PathEscapeTests
    {
        [Test]
        public void SimplePath()
        {
            const string input = @"C:\a.dfy";
            const string expected = input;

            var output = FileHelper.EscapeFilePath(input);

            Assert.AreEqual(expected, output);
        }


        [Test]
        public void SpacedPath()
        {
            const string input = @"C:\some folder\a.dfy";
            const string expected = "\"" + input + "\"";

            var output = FileHelper.EscapeFilePath(input);

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void MultipleSpacedPath()
        {
            const string input = @"C:\some folder\here is my file.dfy";
            const string expected = "\"" + input + "\"";

            var output = FileHelper.EscapeFilePath(input);

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void PreEscapedPath()
        {
            const string input = @"""C:\some folder\a.dfy""";
            const string expected = input;

            var output = FileHelper.EscapeFilePath(input);

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void PathWithQuoteWithSpaceThrows()
        {
            const string input = @"C:\some fol""der\a.dfy";
            Assert.Throws<NotSupportedException>(() => FileHelper.EscapeFilePath(input));
        }

        [Test]
        public void PathWithQuoteWithoutSpaceThrows()
        {
            const string input = @"C:\ba""ba\a.dfy";
            Assert.Throws<NotSupportedException>(() => FileHelper.EscapeFilePath(input));
        }

    }

}