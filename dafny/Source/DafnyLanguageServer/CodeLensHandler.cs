using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public class CodeLensHandler : ICodeLensHandler
    {
        private CodeLensCapability _capability;
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.dfy"
            }
        );

        public CodeLensHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions()
        {
            return new CodeLensRegistrationOptions
            {
                DocumentSelector = _documentSelector,
                ResolveProvider = false
            };
        }

        public async Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                List<CodeLens> items = new List<CodeLens>();

                var fileSymboltable = _bufferManager.GetSymboltableForFile(request.TextDocument.Uri);
                foreach(var symbol in fileSymboltable.GetList())
                {
                    var symbolReferencecounter = 0; 
                    foreach(var fileBuffers in _bufferManager.GetAllFiles().Values)
                    {
                        foreach(var filesSymboltable in fileBuffers.Symboltable.GetList())
                        {
                            if (filesSymboltable.Name == symbol.Name)
                                symbolReferencecounter++; 
                        }
                    }

                    Range range = new Range {
                        Start = new Position((long)symbol.Line, (long)symbol.Position),
                        End = new Position((long)symbol.EndLine, (long)symbol.EndPosition)
                    };
                    Command command = new Command { Title = symbolReferencecounter+" reference", Name = "dafny.showReferences" };

                    items.Add(new CodeLens { Data = request.TextDocument.Uri, Range = range, Command = command });
                }

                // for each (field or method) symbol in current document
                // go throught each buffer (all files) and count used references





                // get symboltable for current document
                // filter "needsCodeLens"  .... fiield or method ... not constructor method
                // => btw: 
                //public static DefaultModuleName: string = "_default";
                //public static ConstructorMethod: string = "_ctor";


                Range range1 = new Range { Start = new Position(3, 5), End = new Position(3, 15) }; // position... 
                Command command1 = new Command { Title = "1 reference", Name = "acc3.m()"};

                Command command2 = new Command { Title = "1 reference", Name = "dafny.showReferences" };
                
                
                items.Add(new CodeLens { Data = request.TextDocument.Uri, Range = range1, Command = command1 /*Range, Comand, JToken Data*/ });

                return new CodeLensContainer(items); 
            });
        }

        public void SetCapability(CodeLensCapability capability)
        {
            _capability = capability;
        }
    }
}
