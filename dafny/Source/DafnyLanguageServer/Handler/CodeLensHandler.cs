using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
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
                // die laufzeit ist grottig husthust
                foreach (var symbol in fileSymboltable.GetFullList())
                {
                    var symbolReferencecounter = 0;
                    foreach (var fileBuffers in _bufferManager.GetAllFiles().Values)
                    {
                        foreach (var filesSymboltable in fileBuffers.Symboltable.GetFullList())
                        {
                            if (filesSymboltable.Name == symbol.Name)
                                symbolReferencecounter++;
                        }
                    }

                    Position position = new Position((long)symbol.Line - 1, 0);
                    Range range = new Range { Start = position,  End = position };
                    Command command = new Command {
                        Title = (symbolReferencecounter-1)+" reference(s) to "+symbol.Name,
                        Name = "dafny.showReferences"
                    };

                    items.Add(new CodeLens { Data = request.TextDocument.Uri, Range = range, Command = command });
                }
                return new CodeLensContainer(items); 
            });
        }

        public void SetCapability(CodeLensCapability capability)
        {
            _capability = capability;
        }
    }
}
