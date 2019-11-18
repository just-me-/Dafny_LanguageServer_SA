using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Boogie;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using DafnyHelper = DafnyLanguageServer.DafnyAdapter.DafnyHelper;

namespace DafnyLanguageServer
{
    public class VerificationService
    {
        private readonly ILanguageServer _router;

        public VerificationService(ILanguageServer router)
        {
            _router = router;
        }

        public void Verify(DafnyFile file)
        {
            // im Plugin das aktuelle Dokument setzen für die Statusbar
            _router.Window.SendNotification("activeVerifiyingDocument", file.Filepath);
            try
            {
                var helper = DafnyVerify(file);
                var diagnostics = CreateDafnyDiagnostics(helper.Errors, file.Filepath, file.Sourcecode);

                PublishDiagnosticsParams p = new PublishDiagnosticsParams
                {
                    Uri = file.Uri,
                    Diagnostics = new Container<Diagnostic>(diagnostics)
                };
                _router.Document.PublishDiagnostics(p);
                SendErrornumberToClient(diagnostics.Count);
            } catch (ArgumentException e)
            {
                // 2do
            }
            
        }

        private void SendErrornumberToClient(int counted)
        {
            _router.Window.SendNotification("updateStatusbar", counted);
        }

        public DafnyHelper DafnyVerify(DafnyFile file)
        {
            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, file.Filepath, file.Sourcecode);
            if (!helper.Verify())
            {
                throw new ArgumentException("Failed to verify document.");
            }
            return helper; 
        }

        public Collection<Diagnostic> CreateDafnyDiagnostics(IEnumerable<ErrorInformation> errors, string filepath, string sourcecode)
        {
            Collection<Diagnostic> diagnostics = new Collection<Diagnostic>();

            foreach (ErrorInformation e in errors)
            {
                int line = e.Tok.line - 1;
                int col = e.Tok.col - 1;
                int length = FileHelper.GetLineLength(sourcecode, line) - col;

                Diagnostic d = new Diagnostic
                {
                    Message = e.Msg + " - Hint: " + e.Tok.val,
                    Range = FileHelper.CreateRange(line, col, length),
                    Severity = DiagnosticSeverity.Error,
                    Source = filepath
                };

                for (int i = 0; i < e.Aux.Count - 1; i++) //ignore last element (trace)
                {
                    int auxline = e.Aux[i].Tok.line - 1;
                    int auxcol = e.Aux[i].Tok.col - 1;
                    int auxlength = FileHelper.GetLineLength(sourcecode, auxline) - auxcol;

                    Diagnostic relatedDiagnostic = new Diagnostic
                    {
                        Message = e.Aux[i].Msg,
                        Range = FileHelper.CreateRange(auxline, auxcol, auxlength),
                        Severity = DiagnosticSeverity.Warning,
                        Source = "The error: " + d.Message + " is the source of this warning!"
                    };

                    diagnostics.Add(relatedDiagnostic);
                }
                diagnostics.Add(d);
            }

            return diagnostics; 
        }
    }
}
