using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CompileHandlerTest
{
    static class PathConstants
    {

        private static readonly string assemblyPath = Path.GetDirectoryName(typeof(PathConstants).Assembly.Location);
        internal static readonly string testPath = Path.GetFullPath(Path.Combine(assemblyPath, "../../../../../Test/compileHandler"));
        internal static readonly string dafnyExe = Path.GetFullPath(Path.Combine(assemblyPath, "../../../../../Binaries/Dafny.exe"));

    }
}
