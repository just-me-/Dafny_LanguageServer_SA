using System;
using System.IO;
using Microsoft.Boogie;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DafnyLanguageServer;

namespace CompileHandlerTest
{
    [TestClass]
    public class CompileHandlerTests
    {

        static string assemblyPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        static string testPath = Path.GetFullPath(Path.Combine(assemblyPath, "../../../../Test/compileHandler"));
        static string dafnyExe = Path.GetFullPath(Path.Combine(assemblyPath, "../../../../Binaries/Dafny.exe"));

        [TestMethod]
        public void IsFine()
        {
            string dafnyFile = Path.Combine(testPath, "fineDLL.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsFalse(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            
            
        }

        [TestMethod]
        public void IsFineExe()
        {
            string dafnyFile = Path.Combine(testPath, "fineEXE.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsFalse(r.Error);
            Assert.IsTrue(r.Executable ?? false);

        }

        [TestMethod]
        public void Assertion()
        {
            string dafnyFile = Path.Combine(testPath, "assertion.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("assertion"));

        }

        [TestMethod]
        public void Identifier()
        {
            string dafnyFile = Path.Combine(testPath, "identifier.dfy");

            CompilerResults r = CompileHandler.Compile(dafnyExe, dafnyFile).Result;

            Assert.IsTrue(r.Error);
            Assert.IsFalse(r.Executable ?? true);
            Assert.IsTrue(r.Message.Contains("unresolved identifier"));

        }

        [TestMethod]
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
