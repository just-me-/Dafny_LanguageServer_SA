using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Dafny_Server_Redesign.Server
{
    public class ReciverHandler1 : ILspReciever
    {
        public ReciverHandler1()
        {
        }

        public (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            throw new NotImplementedException();
        }

        public void Initialized()
        {
            throw new NotImplementedException();
        }

        public bool IsValid(JToken container)
        {
            throw new NotImplementedException();
        }
    }

    public class ReciverHandler2 : IExecuteCommandHandler
    {
        public ExecuteCommandRegistrationOptions GetRegistrationOptions()
        {
            throw new NotImplementedException();
        }

        public Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SetCapability(ExecuteCommandCapability capability)
        {
            throw new NotImplementedException();
        }
    }
}
