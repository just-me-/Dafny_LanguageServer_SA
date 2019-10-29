using System;
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
             * Altes Plugin, Server Teil: 
             * 
             * class SymbolTable {
                    public symbols: DafnySymbol[];  => sehr intressante Klasse. jedes "Keyword" sozusagen
                                                       Unknown, Class, Method, Function, Field, Call, Definition, Predicate,
                                                       vgl file symbol.ts
                    public hash: number | undefined;
                    public fileName: string;
                    constructor(fileName: string) {
                        this.symbols = [];
                        this.fileName = fileName;
                    }
                }
             * 
             * symbolService: instanziiert diese SymbolTable
             * hat getSymbolsFromDafny, askDafnyForSymbols(gibt TextDocument mit), parseSymbols füllt Tabelle ab
             * 
             * private askDafnyForSymbols(document: TextDocument): Promise<any> {
                    return new Promise((resolve, reject) => {
                        this.server.addDocument(
                            document,
                            DafnyVerbs.Symbols,
                            (response: string) => this.handleProcessData(response, resolve),
                            () => reject(`Error while requesting symbols from dafny for document "${document.uri}"`),
                        );
                    });
                }
             * => löst verification process aus. ==> 2do evt erhalten wir via verification bereits die infos über das file? 
             * 
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

            

            // als DanfyFile Objekt ändern um dann den DanfyHelper elegant aufzurufen. 
            /*
            static public DafnyHelper DafnyGetSymbols(DafnyFile file, ILanguageServer routertmp)
                {
                    string[] args = new string[] { };
                    DafnyHelper helper = new DafnyHelper(args, file.Filepath, file.Sourcecode);


                    String s = helper.Symbols(); 

                    routertmp.Window.SendNotification("Symbols ============", s);
                }
            */

            return await Task.Run(() =>
            {
                var documentPath = request.TextDocument.Uri.ToString();
                var buffer = _bufferManager.GetTextFromBuffer(request.TextDocument.Uri);
                var version = VersionCheck.CurrentVersion();

                var demotext = "i'm the new text";
                var demotext2 = "You can do this !!!!  ;-) <3 <3 <3 :-) Keep trying!";

                if (buffer == null)
                {
                    return new CompletionList();
                }


                // ahmumumu :-)
                string[] args = new string[] { };
                DafnyHelper helper = new DafnyHelper(args, documentPath, buffer);
                String s = helper.Symbols();
                demotext2 = s; 

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
            });
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
