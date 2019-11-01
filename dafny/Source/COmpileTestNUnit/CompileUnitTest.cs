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
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineDLL);
            CompileHandler.Compile(dafnyExe, dafnyFile);
            
            Thread.Sleep(500);

            DateTime exeLastAccess = File.GetLastAccessTime(dafnyExe);
            TimeSpan diff = DateTime.Now - exeLastAccess;

            Assert.LessOrEqual(diff, new TimeSpan(0,0,2), "dafnyExe was not Called within the last second");
        }

        [Test]
        public void DfyAccess()
        {
            string dafnyFile = Path.Combine(testPath, PathConstants.dfy_fineDLL);
            CompileHandler.Compile(dafnyExe, dafnyFile);

            Thread.Sleep(500);

            DateTime dfyLastAccess = File.GetLastAccessTime(dafnyFile);
            TimeSpan diff = DateTime.Now - dfyLastAccess;

            Assert.LessOrEqual(diff, new TimeSpan(0, 0, 2), ".dfy was not Called within the last second");
        }

    }
}
