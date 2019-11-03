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
            /*
             * 
             * 2Do: von hier noch abgucken wie sie "die inteligente" vervollständigung gemacht haben
             * 
             * completionProvider.ts (auch server) 
             *   const word = this.parseWordForCompletion(document, handler.position);
                 const allSymbols = await this.server.symbolService.getAllSymbols(document);
                 const definition = allSymbols.find((e) => e.isDefinitionFor(word));
             * 
             *  falls "definition" ? liefereExakte Completion : liefere closest completion
             *  ... und baut dann die commands so zusammen wie wir das hier unten fakemässig gemacht haben. 
             *  
             *  
             *  private provideExactCompletions(symbols: DafnySymbol[], definition: DafnySymbol): CompletionItem[] {
                    const possibleSymbolForCompletion: DafnySymbol[] = symbols.filter(
                                (symbol: DafnySymbol) => symbol.canProvideCodeCompletionForDefinition(definition));
                    return possibleSymbolForCompletion.map((e: DafnySymbol) => this.buildCompletion(e));
                }

                private provideBestEffortCompletion(symbols: DafnySymbol[], word: string): CompletionItem[] {
                    const fields: DafnySymbol[] = symbols.filter((e: DafnySymbol) => e.isField(word));
                    const definingClass = this.findDefiningClassForField(symbols, fields);
                    if (definingClass) {
                        const possibleSymbolForCompletion: DafnySymbol[] = symbols.filter(
                            (symbol: DafnySymbol) => symbol.canProvideCodeCompletionForClass(definingClass));
                        return possibleSymbolForCompletion.map((e: DafnySymbol) => this.buildCompletion(e));
                    }
                    return [];
                }
                public canProvideCodeCompletionForClass(symbol: DafnySymbol) {
                    return this.hasParentClass(symbol.name) &&
                        this.hasModule(symbol.module) &&
                        this.isOfType([SymbolType.Field, SymbolType.Method]) &&
                        this.name !== DafnyKeyWords.ConstructorMethod;
                }
             */

            return await Task.Run(() =>
            {
                var documentPath = request.TextDocument.Uri.ToString();
                var symbols = _bufferManager.GetSymboltableFromBuffer(request.TextDocument.Uri); // 2do... noma mit dem alten server vergleichen
                // hmm eig will ich hier nicht die symbols von einem dokument sondern von allen. 
                // kann man sagen; nur die varis vom aktuellen dokument... methoden / klassen aber von allen files? 

                return convertListToCompletionresponse(symbols, request); 
            });
        }

        private CompletionList convertListToCompletionresponse(List<SymbolTable.SymbolInformation> symbols, CompletionParams request)
        {
            var complitionItems = new List<CompletionItem>(); 
            foreach(var symbol in symbols)
            {
                complitionItems.Add(
                    new CompletionItem
                    {
                        Label = symbol.Name+" --- Type: "+symbol.SymbolType,
                        Kind = CompletionItemKind.Reference,
                        TextEdit = new TextEdit
                        {
                            NewText = symbol.Name,
                            Range = new Range( // hmm ned das von der tabelle nehmen (weil das die vorkommen sind) sondern aktuelle position oder? 
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
            // 2do does empty case still work? 
        }

        /*
        private static int GetPosition(string buffer, int line, int col)
        {
            var position = 0;
            for (var i = 0; i < line; i++)
            {
                position = buffer.IndexOf('\n', position) + 1;
            }
            return position + col;
        }
        */

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }

    }
}
