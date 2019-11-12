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

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions();
        }

        public async Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                List<LocationOrLocationLink> links = new List<LocationOrLocationLink>();

                Range range = new Range { Start = new Position(1, 5), End = new Position(10, 15) }; // position... 
                var location = new Location { Uri = request.TextDocument.Uri, Range = range};
                links.Add(new LocationOrLocationLink(location));

                return new LocationOrLocationLinks(links);
            });
        }

        public void SetCapability(DefinitionCapability capability)
        {
            _capability = capability;
        }
    }
}
