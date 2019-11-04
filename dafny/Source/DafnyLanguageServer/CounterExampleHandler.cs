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
        public string DafnyFile { get; set; }
    }

    public class CounterExampleResults
    {
        public int Line { get; set; }
        public int Col { get; set; }
        public string Variable { get; set; }
        public string Value { get; set; }
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
            var entry = models[models.Count - 1].States;
            var lastEntry = entry[entry.Count - 1];
            var lastEntryVariables = lastEntry.Variables;
            var lastEntryFirstVariable = lastEntryVariables[0];

            result.Col = lastEntry.Column;
            result.Line = lastEntry.Line;
            result.Variable = lastEntryFirstVariable.Name;
            result.Value = lastEntryFirstVariable.Value;

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


