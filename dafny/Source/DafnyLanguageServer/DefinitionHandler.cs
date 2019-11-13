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
    public class DefinitionHandler : IDefinitionHandler
    {
        private DefinitionCapability _capability;
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.dfy"
            }
        );

        public DefinitionHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions
            {
                DocumentSelector = _documentSelector
            };
        }

        public async Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken token)
        {
            return await Task.Run(() =>
            {
            List<LocationOrLocationLink> links = new List<LocationOrLocationLink>();
                
                // function? definition... variable? declaration 

                // quiick n dirty annahme für v1: 
                // das erste vorkommen des symbols muss die definition sein 

                var symbols = _bufferManager.GetSymboltableForFile(request.TextDocument.Uri);

                var word = FileHelper.GetFollowingWord(
                    _bufferManager.GetTextFromBuffer(request.TextDocument.Uri),
                    (int)request.Position.Line,
                    (int)request.Position.Character
                );
                foreach (var symbol in symbols.GetFullList())
                {
                    if(word == symbol.Name)
                    {
                        Position position = new Position((long)symbol.Line-1, (long)symbol.Column);
                        Range range = new Range { Start = position, End = position };
                        var location = new Location { Uri = request.TextDocument.Uri, Range = range };

                        links.Add(new LocationOrLocationLink(location));
                        break; 
                    }
                }
                return new LocationOrLocationLinks(links);
            });
        }

        public void SetCapability(DefinitionCapability capability)
        {
            _capability = capability;
        }
    }
}
