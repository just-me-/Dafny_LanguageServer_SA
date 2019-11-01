using System.IO;
using DafnyLanguageServer;
using NUnit.Framework;

namespace CompileHandlerTest
{
    public class CompileIntegrationTests
    {

        static string assemblyPath = Path.GetDirectoryName(typeof(CompileIntegrationTests).Assembly.Location);
        static string testPath = Path.GetFullPath(Path.Combine(assemblyPath, "../../../../../Test/compileHandler"));
        static string dafnyExe = Path.GetFullPath(Path.Combine(assemblyPath, "../../../../../Binaries/Dafny.exe"));

        [SetUp]
        public void Setup()
        {
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
    }
}