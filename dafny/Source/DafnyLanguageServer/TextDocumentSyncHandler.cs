using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace DafnyLanguageServer
{
    internal class TextDocumentSyncHandler : ITextDocumentSyncHandler
    {
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.dfy"
            }
        );

        private SynchronizationCapability _capability;

        public TextDocumentSyncHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full; //TODO: Incremental damit nicht der ganze Karsumpel geschickt wird

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                SyncKind = Change
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "xml");
        }

        public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToString();
            var text = request.ContentChanges.FirstOrDefault()?.Text;

            _bufferManager.UpdateBuffer(documentPath, text);

            _router.Window.LogInfo($"Handled DidChangeDoc ---- Updated buffer for document: {documentPath}\n{text}");
            
            string filename = documentPath;
            string[] args2 = new string[] { };
            string source = text;

            DafnyHelper helper = new DafnyHelper(args2, filename, source);
            bool isValid = helper.Verify();

            Collection<Diagnostic> diagnostics = new Collection<Diagnostic>();

            foreach (ErrorInformation e in helper.Errors)
            {
                _router.Window.LogInfo($"Found Error '{e.Msg}' in Line {e.Tok.line} Col {e.Tok.col}. There is a problem at {e.Tok.val}.");
                Diagnostic d = new Diagnostic();
                d.Message = e.Msg;
                d.Range = new Range(
                    new Position
                    {
                        Line = e.Tok.line-1,
                        Character = e.Tok.col-1
                    }, new Position
                    {
                        Line = e.Tok.line-1,
                        Character = e.Tok.col + 1 -1
                    });
                d.Severity = DiagnosticSeverity.Error;
                d.Source = documentPath;
                diagnostics.Add(d);
            }

            PublishDiagnosticsParams p = new PublishDiagnosticsParams();
            p.Uri = request.TextDocument.Uri;
            p.Diagnostics = new Container<Diagnostic>(diagnostics);

            
            //_router.SendNotification("verificationResult", p);
            _router.Document.PublishDiagnostics(p);
            
            return Unit.Task;
        }

        public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var text = request.TextDocument.Text;
            var documentPath = request.TextDocument.Uri.ToString();
            _bufferManager.UpdateBuffer(documentPath, request.TextDocument.Text);
            _router.Window.LogInfo($"Server handled DidOpenDocument ---- Updated buffer for document: {documentPath}\n{text}");
            return Unit.Task;
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
            };
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }
    }
}