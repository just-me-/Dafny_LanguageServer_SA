using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DafnyServer;
using Microsoft.Dafny;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace DafnyLanguageServer
{
    internal class CompletionHandler : ICompletionHandler
    {
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;
        private CompletionCapability _capability;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.dfy"
            }
        );

        public CompletionHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions
            {
                DocumentSelector = _documentSelector,
                ResolveProvider = false
            };
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var symbols = _bufferManager.GetSymboltableForFile(request.TextDocument.Uri);
                var word = "C"; // 2do..
                return (symbols is null) ?
                    new CompletionList() :
                    convertListToCompletionresponse(symbols.getList(word), request);
            });
        }

        private CompletionList convertListToCompletionresponse(List<SymbolTable.SymbolInformation> symbols, CompletionParams request)
        {
            var complitionItems = new List<CompletionItem>(); 
            foreach(var symbol in symbols)
            {
                CompletionItemKind kind = CompletionItemKind.Reference;
                Enum.TryParse(symbol.SymbolType.ToString(), true, out kind);

                complitionItems.Add(
                    new CompletionItem
                    {
                        Label = symbol.Name + " (Type: " + symbol.SymbolType + ")" + symbol.ParentClass,
                        Kind = kind, 
                        TextEdit = new TextEdit
                        {
                            NewText = symbol.Name,
                            Range = new Range( 
                            new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character
                            }, new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character + symbol.Name.Length
                            })
                        }
                    }); 
            }
            return new CompletionList(complitionItems); 
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }

    }
}
