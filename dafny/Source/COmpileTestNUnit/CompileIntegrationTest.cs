using System.Collections.Generic;
using System.IO;
using DafnyLanguageServer;
using NUnit.Framework;

namespace CompileHandlerTest
{
    public class CompileIntegrationTests
    {

        private static readonly string testPath = PathConstants.testPath;
        private static readonly string dafnyExe = PathConstants.dafnyExe;

        [SetUp]
        public void DeleteFiles()
        {
            List<string> files = new List<string>
            {
                Path.Combine(testPath, "fineDLL.dll"),
                Path.Combine(testPath, "fineDLL.pdb"),
                Path.Combine(testPath, "fineEXE.exe"),
                Path.Combine(testPath, "fineEXE.pdb")
            };

            foreach (string path in files)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Test]
        public void IsFine()
        {
            string dafnyFile = Path.Combine(testPath, "fineDLL.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsFalse(r.Error);
            Assert.IsFalse(r.Executable ?? true);

        }

        [Test]
        public void IsFineExe()
        {
            string dafnyFile = Path.Combine(testPath, "fineEXE.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsFalse(r.Error);
            Assert.IsTrue(r.Executable ?? false);

        }

        [Test]
        public void Assertion()
        {
            string dafnyFile = Path.Combine(testPath, "assertion.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("assertion"));

        }

        [Test]
        public void Identifier()
        {
            string dafnyFile = Path.Combine(testPath, "identifier.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("unresolved identifier"));

        }

        [Test]
        public void Postcondition()
        {
            string dafnyFile = Path.Combine(testPath, "postcondition.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("postcondition might not hold"));

        }


        [Test]
        public void DllCreated()
        {
            string dafnyFile = Path.Combine(testPath, "fineDLL.dfy");
            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;
            Assert.IsTrue(File.Exists(Path.Combine(testPath, "fineDLL.dll")));
        }

        [Test]
        public void ExeCreated()
        {
            string dafnyFile = Path.Combine(testPath, "fineEXE.dfy");
            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;
            Assert.IsTrue(File.Exists(Path.Combine(testPath, "fineExe.exe")));
        }
    }
}