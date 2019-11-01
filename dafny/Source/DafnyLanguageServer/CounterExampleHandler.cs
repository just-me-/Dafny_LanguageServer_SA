using System;
using System.Diagnostics;
using System.IO.Packaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace DafnyLanguageServer
{
    public class CounterExampleParams : IRequest<CounterExampleResults>
    {
        public string DafnyFileUri { get; set; }
    }

    public class CounterExampleResults
    {
        public string counterExample { get; set; }

        [Serial, Method("counterExample")]
        public interface ICounterExample : IJsonRpcRequestHandler<CounterExampleParams, CounterExampleResults>
        {
        }

        public class CounterExampleHandler : ICounterExample
        {
            public async Task<CounterExampleResults> Handle(CounterExampleParams request,
                CancellationToken cancellationToken)
            {


                return null;
            }





        }
    }
}



/*    type ist da counterExample, und arg ist das TextDocument als ganzes
     recht generisch, im tditem sogar die languageid drin, wtf.
     
 *     private sendDocument(textDocument: vscode.TextDocument, type: string): void {
        if (textDocument !== null && textDocument.languageId === EnvironmentConfig.Dafny) {
            this.context.localQueue.add(textDocument.uri.toString());
            const tditem = JSON.stringify(TextDocumentItem.create(textDocument.uri.toString(),
                textDocument.languageId, textDocument.version, textDocument.getText()));
            this.languageServer.sendNotification(type, tditem);
        }
    }

    */


