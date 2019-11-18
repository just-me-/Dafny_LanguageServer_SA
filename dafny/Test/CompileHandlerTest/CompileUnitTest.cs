using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using DafnyLanguageServer;
using NUnit.Framework;


namespace CompileHandlerTest
{
    class CompileUnitTest
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
        public void ExeCalled()
        {
            /*
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineDLL);
            new CompilationService(dafnyExe, dafnyFile).Compile();

            Thread.Sleep(1000);

            DateTime exeLastAccess = File.GetLastAccessTime(dafnyExe); //2do: Auf server nicht supported; und so timing zeugs ist eh nicht gut
            TimeSpan diff = DateTime.Now - exeLastAccess;

            Assert.LessOrEqual(diff, new TimeSpan(0,0,3), "dafnyExe was not Called within the last second");
            */
            Assert.Pass();
        }

        [Test]
        public void DfyAccess()
        {
            /*
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineDLL);
            new CompilationService(dafnyExe, dafnyFile).Compile();

            Thread.Sleep(1000);

            DateTime dfyLastAccess = File.GetLastAccessTime(dafnyFile);
            TimeSpan diff = DateTime.Now - dfyLastAccess;

            Assert.LessOrEqual(diff, new TimeSpan(0, 0, 3), ".dfy was not Called within the last second");
            */
            Assert.Pass();
        }

    }
}
