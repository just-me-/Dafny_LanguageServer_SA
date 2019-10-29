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
             * symbolService: instanziiert diese SymbolTable
             * hat getSymbolsFromDafny, askDafnyForSymbols(gibt TextDocument mit), parseSymbols füllt Tabelle ab
             * 
             * ---
             * 
             * completionProvider.ts (auch server) 
             *   const word = this.parseWordForCompletion(document, handler.position);
                 const allSymbols = await this.server.symbolService.getAllSymbols(document);
                 const definition = allSymbols.find((e) => e.isDefinitionFor(word));
             * 
             *  falls "definition" ? liefereExakte Completion : liefere closest completion
             *  ... und baut dann die commands so zusammen wie wir das hier unten fakemässig gemacht haben. 
             * 
             */

            return await Task.Run(() =>
            {
                var documentPath = request.TextDocument.Uri.ToString();
                var buffer = _bufferManager.GetTextFromBuffer(request.TextDocument.Uri);
                var version = VersionCheck.CurrentVersion(); // wozu? löschbar? ins startup verschieben unnd info an plugin senden? 

                if (buffer == null)
                {
                    return new CompletionList();
                }

                var symbols = getSymbolList(documentPath, buffer);
                return convertListToCompletionresponse(symbols, request); 
                // return tmpGetFakeCompletion(request); 
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
                        Label = symbol.Name+" Type: "+symbol.SymbolType,
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
        }

        private CompletionList tmpGetFakeCompletion(CompletionParams request)
        {
            var demotext = "i'm the new text";
            var demotext2 = "You can do this !!!!  ;-) <3 <3 <3 :-) Keep trying!";
            var citem1 = new CompletionItem
            {
                Label = "Insert a new Text",
                Kind = CompletionItemKind.Reference,
                TextEdit = new TextEdit
                {
                    NewText = demotext,
                    Range = new Range(
                        new Position
                        {
                            Line = request.Position.Line,
                            Character = request.Position.Character
                        }, new Position
                        {
                            Line = request.Position.Line,
                            Character = request.Position.Character + demotext.Length
                        })
                }
            };
            var citem2 = new CompletionItem
            {
                Label = "Let me cheer you up",
                Kind = CompletionItemKind.Reference,
                TextEdit = new TextEdit
                {
                    NewText = demotext2,
                    Range = new Range(
                        new Position
                        {
                            Line = request.Position.Line,
                            Character = request.Position.Character
                        }, new Position
                        {
                            Line = request.Position.Line,
                            Character = request.Position.Character + demotext2.Length
                        })
                }
            };
            return new CompletionList(citem1, citem2);
        }

        private List<SymbolTable.SymbolInformation> getSymbolList(String documentPath, String code)
        {
            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, documentPath, code);
            return helper.Symbols();
        }

        private static int GetPosition(string buffer, int line, int col)
        {
            var position = 0;
            for (var i = 0; i < line; i++)
            {
                position = buffer.IndexOf('\n', position) + 1;
            }
            return position + col;
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }
    }
}
