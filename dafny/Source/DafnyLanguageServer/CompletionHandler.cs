using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SymbolTable = DafnyLanguageServer.DafnyAdapter.SymbolTable;

namespace DafnyLanguageServer
{
    public class CompletionHandler : ICompletionHandler
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
                var word = FileHelper.GetCurrentWord(
                    _bufferManager.GetTextFromBuffer(request.TextDocument.Uri),
                    (int)request.Position.Line,
                    (int)request.Position.Character
                );
                var parentClass = symbols.GetParentForWord(word);
                return (symbols is null) ?
                    new CompletionList() :
                    ConvertListToCompletionresponse(symbols.GetList(parentClass), request);
            });
        }

        public CompletionList ConvertListToCompletionresponse(List<SymbolTable.SymbolInformation> symbols, CompletionParams request)
        {
            var complitionItems = new List<CompletionItem>();
            foreach(var symbol in symbols)
            {
                CompletionItemKind kind = CompletionItemKind.Reference;
                Enum.TryParse(symbol.SymbolType.ToString(), true, out kind);


                Range range = GetRange(request.Position.Line, request.Position.Character, symbol.Name.Length);
                TextEdit textEdit = new TextEdit
                {
                    NewText = symbol.Name,
                    Range = range
                };

                complitionItems.Add(
                    // jedes new ein dingens machen. zwischenobjekte als eigene lokale objekte reinmachen. 2do
                    new CompletionItem
                    {
                        Label = $"{symbol.Name} (Type: {symbol.SymbolType}) (Parent: {symbol.ParentClass})", // 2do: Klasse Label für prod und dev mode ODER überschreiben
                        Kind = kind,
                        TextEdit = textEdit
                    });
            }
            return new CompletionList(complitionItems);
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }


        //Todo: könnte man auch in nen utility klasse tun ;-)
        private Position GetPosition(long start, long end)
        {
            return new Position
            {
                Line = start,
                Character = end
            };
        }

        private Range GetRange(long line, long chr, long length)
        {

            Position start = GetPosition(line, chr);
            Position end = GetPosition(line, chr+length);
            return new Range(start, end);
        }
    }
}
