using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{


    /* 2do TODO: Brauchen wir das????*/
    //nein

    class BufferManagerMock : IBufferManager
    {
        private ConcurrentDictionary<Uri, DafnyFile> _buffers = new ConcurrentDictionary<Uri, DafnyFile>();

        public BufferManagerMock()
        {

            Uri uri1 = new Uri("uri1");

            string source1 = @"method MultipleReturns(x: int, y: int) returns (more: int, less: int)
                requires 0 < y
                ensures less < x < more
                {
                more := x + y;
                less:= x - y;
                // assert x == 1;              //auskommentieren -> assertion violation geht weg
                var a := 1;
                var aa := 1;
                assert x == 1;

            }";

            FileSymboltable symboltable = new FileSymboltable(uri1.ToString(), source1);

            DafnyFile file1 = new DafnyFile
            {
                Sourcecode = source1,
                Symboltable = symboltable,
                Uri = uri1
            };

            if (symboltable.HasEntries)
            {
                file1.Symboltable = symboltable;
            }

            _buffers.AddOrUpdate(uri1, file1, (k,v) => file1);
        }

        public void UpdateBuffer(Uri documentPath, string content)
        {
            throw new NotImplementedException();
        }

        public void UpdateBuffer(DafnyFile file)
        {
            throw new NotImplementedException();
        }

        public string GetTextFromBuffer(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer.Sourcecode : null;
        }

        public FileSymboltable GetSymboltableForFile(Uri documentPath)
        {
            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer.Symboltable : null;
        }

        public ConcurrentDictionary<Uri, DafnyFile> GetAllFiles()
        {
            return _buffers;
        }
    }
}
