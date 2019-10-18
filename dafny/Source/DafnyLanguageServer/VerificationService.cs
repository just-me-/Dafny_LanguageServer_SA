using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace DafnyLanguageServer
{
    class VerificationService
    {

        private Uri FileUri { get; }
        private string Sourcecode { get; set; }
        private ILanguageServer Router { get; }
        private string Filename => FileUri.ToString();
        private readonly string[] args = new string[] { };

        public VerificationService(ILanguageServer router, Uri uri, string sourcecode)
        {
            Router = router;
            Sourcecode = sourcecode;
            FileUri = uri;
        }

        public void Verify()
        {

            DafnyHelper helper = new DafnyHelper(args, Filename, Sourcecode);

            if (!helper.Verify())
            {
                throw new ArgumentException("Während des Verifizierens ist ein Fehler aufgetreten, der nicht hätte passieren dürfen.");
            }

            Collection<Diagnostic> diagnostics = new Collection<Diagnostic>();

            foreach (ErrorInformation e in helper.Errors)
            {
                Diagnostic d = new Diagnostic();
                d.Message = e.Msg;
                d.Range = new Range(
                    new Position
                    {
                        Line = e.Tok.line - 1,
                        Character = e.Tok.col - 1
                    }, new Position
                    {
                        Line = e.Tok.line - 1,
                        Character = e.Tok.col + 1 - 1
                    });

                d.Severity = DiagnosticSeverity.Error;
                d.Source = Filename;
                diagnostics.Add(d);
            }

            PublishDiagnosticsParams p = new PublishDiagnosticsParams
            {
                Uri = FileUri,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            };

            Router.Document.PublishDiagnostics(p);
        }
    }
}
