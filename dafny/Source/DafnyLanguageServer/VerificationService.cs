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

        private static readonly int MAGICLINEENDING = 100;

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
                d.Message = e.Msg + " - Hint: " + e.Tok.val;
                d.Range = new Range(
                    new Position
                    {
                        Line = e.Tok.line - 1,
                        Character = e.Tok.col - 1
                    }, new Position
                    {
                        Line = e.Tok.line - 1,
                        Character = e.Tok.col + MAGICLINEENDING
                    });

                d.Severity = DiagnosticSeverity.Error;
                d.Source = Filename;

                /*
                Collection<DiagnosticRelatedInformation> auxWarnings = new Collection<DiagnosticRelatedInformation>();
                for (int i = 0; i < e.Aux.Count - 1; i++)  //ignore last element (trace iwas)
                {
                    DiagnosticRelatedInformation info = new DiagnosticRelatedInformation();
                    info.Message = e.Aux[i].Msg;
                    info.Location = "1";            //https://github.com/OmniSharp/csharp-language-server-protocol/issues/175
                    auxWarnings.Add(info);
                }

                d.RelatedInformation = auxWarnings;

              */

                for (int i = 0; i < e.Aux.Count - 1; i++) //ignore last element (trace iwas)
                {
                    Diagnostic relatedDiagnostic = new Diagnostic();
                    relatedDiagnostic.Message = e.Aux[i].Msg;
                    relatedDiagnostic.Range = new Range(
                        new Position
                        {
                            Line = e.Aux[i].Tok.line - 1,
                            Character = e.Aux[i].Tok.col - 1
                        }, new Position
                        {
                            Line = e.Aux[i].Tok.line - 1,
                            Character = e.Aux[i].Tok.col + MAGICLINEENDING
                        });

                    

                    relatedDiagnostic.Severity = DiagnosticSeverity.Warning;
                    relatedDiagnostic.Source = "The error: " + d.Message + " is the source of this warning!";
                    diagnostics.Add(relatedDiagnostic);
                }





                diagnostics.Add(d);
            }

            PublishDiagnosticsParams p = new PublishDiagnosticsParams
            {
                Uri = FileUri,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            };

            Router.Document.PublishDiagnostics(p);
            Router.SendNotification("updateStatusbar", diagnostics.Count);
        }
    }
}
