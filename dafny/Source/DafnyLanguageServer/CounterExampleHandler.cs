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
using System.Collections.Generic;

namespace DafnyLanguageServer
{
    public class CounterExampleParams : IRequest<CounterExampleResults>
    {
        public string DafnyFile { get; set; }
    }

    public class CounterExampleResults
    {
        public int Line { get; set; }
        public int Col { get; set; }
        public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();

    }


    [Serial, Method("counterExample")]
    public interface ICounterExample : IJsonRpcRequestHandler<CounterExampleParams, CounterExampleResults>
    {
    }

    public class CounterExampleHandler : ICounterExample
    {

        private readonly BufferManager _bufferManager;

        public CounterExampleHandler(BufferManager b)
        {
            _bufferManager = b;
        }

        public async Task<CounterExampleResults> Handle(CounterExampleParams request, CancellationToken cancellationToken)
        {
            string[] args = new string[] { };
            string filename = request.DafnyFile;
            string programSource = _bufferManager.GetTextFromBuffer(new Uri(request.DafnyFile));

            var result = new CounterExampleResults();

            var helper = new DafnyHelper(args, filename, programSource);
            var models = helper.CounterExample();
            var states = models[0].States;
            var firstEntry = states[2];   //bisschen viele magic numbers aber ich glaub das kommt halt so retour von dafny. TODO: Im Dafny Helper anpassen und was gescheites returnen.

            var firstEntryVariables = firstEntry.Variables;

            result.Col = firstEntry.Column;
            result.Line = firstEntry.Line;

            foreach (var variable in firstEntryVariables)
            {
                result.Variables.Add(variable.Name, variable.Value);
            }
            

            return result;
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


