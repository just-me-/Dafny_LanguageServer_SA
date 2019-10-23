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
    class VerificationService //  müsste der Service designtechnisch nicht sogar static sein? 
    {
        private static readonly int MAGICLINEENDING = 100; // 2Do evt dynamisch anpassen an jeweilige Zeilenlänge 
        
        private DafnyFile File { get; set; }

        private ILanguageServer Router { get; }
        private readonly string[] args = new string[] { }; // hmm inlinen? 

        public VerificationService(ILanguageServer router, DafnyFile file)
        {
            Router = router;
            File = file;
        }

        public void Verify()
        {
            // im plugin das aktuelle Dokument setzen
            Router.Window.SendNotification("activeVerifiyingDocument", File.Filepath);

            DafnyHelper helper = new DafnyHelper(args, File.Filepath, File.Sourcecode);

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
                d.Source = File.Filepath;

                for (int i = 0; i < e.Aux.Count - 1; i++) //ignore last element (trace)
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
                Uri = File.Uri,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            };

            Router.Document.PublishDiagnostics(p);
            Router.Window.SendNotification("updateStatusbar", diagnostics.Count);
        }
    }
}
