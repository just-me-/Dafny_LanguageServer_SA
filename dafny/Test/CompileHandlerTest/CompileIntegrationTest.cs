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
                Path.Combine(testPath, PathConstants.fineDLLOutput),
                Path.Combine(testPath, PathConstants.fineEXEOutput)
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
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineDLL);

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsFalse(r.Error);
            Assert.IsFalse(r.Executable ?? true);

        }

        [Test]
        public void IsFineExe()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineEXE);

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsFalse(r.Error);
            Assert.IsTrue(r.Executable ?? false);

        }

        [Test]
        public void Assertion()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_assertion);

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("assertion"));

        }

        [Test]
        public void Identifier()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_identifier);

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("unresolved identifier"));

        }

        [Test]
        public void Postcondition()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_postcondition);

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("postcondition might not hold"));

        }


        [Test]
        public void DllCreated()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineDLL);
            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;
            Assert.IsTrue(File.Exists(Path.Combine(testPath, PathConstants.fineDLLOutput)));
        }

        [Test]
        public void ExeCreated()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineEXE);
            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;
            Assert.IsTrue(File.Exists(Path.Combine(testPath, PathConstants.fineEXEOutput)));
        }
    }
}