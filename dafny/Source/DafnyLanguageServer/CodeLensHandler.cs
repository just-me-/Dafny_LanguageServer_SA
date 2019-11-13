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




                // get symboltable for current document
                // filter "needsCodeLens"  .... fiield or method ... not constructor method
                // => btw: 
                    //public static DefaultModuleName: string = "_default";
                    //public static ConstructorMethod: string = "_ctor";
                // map format to "ReferencesCodeLens"

                //var symbolTable = _bufferManager.GetSymboltableForFile(request.TextDocument.Uri).GetList()[0];
                //items.Add(new CodeLens { Data = request.TextDocument.Uri, symbolTable. /*Range, Comand, JToken Data*/ });

                // Range should only be a single line
                // command: anzeige
                // data irrelevant 

                Range range = new Range { Start = new Position(3, 5), End = new Position(3, 15) }; // position... 
                Command command = new Command { Title = "1 reference", Name = "acc3.m()"};

                Command command2 = new Command { Title = "1 reference", Name = "dafny.showReferences" };

                /*
                   codeLens.command = {
                    arguments: [Uri.parse(codeLens.symbol.document.uri).fsPath, codeLens.range.start, locations],
                    command: Commands.ShowReferences,
                    title: locations.length === 1
                        ? "1 reference"
                        : `${locations.length} references`,
                }; */

                items.Add(new CodeLens { Data = request.TextDocument.Uri, Range = range, Command = command /*Range, Comand, JToken Data*/
            });
                items.Add(new CodeLens { Data = request.TextDocument.Uri, Range = range, Command = command /*Range, Comand, JToken Data*/ });
                items.Add(new CodeLens { Data = request.TextDocument.Uri, Range = range, Command = command /*Range, Comand, JToken Data*/ });

                return new CodeLensContainer(items); 
            });
        }
        /*
        private static void ToCodeLens(TextDocumentIdentifier textDocument, Object node, List<CodeLens> codeLensContainer)
        {
            var codeLens = new CodeLens
            {
                Data = textDocument.Uri
                Range = node.Location.ToRange()
            };

            codeLensContainer.Add(codeLens);
        }
        */

        public void SetCapability(CodeLensCapability capability)
        {
            _capability = capability;
        }
    }
}
