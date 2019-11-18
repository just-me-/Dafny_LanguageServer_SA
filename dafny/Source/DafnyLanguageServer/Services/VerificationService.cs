using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DafnyLanguageServer.DafnyAdapter;
using Microsoft.Boogie;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using DafnyHelper = DafnyLanguageServer.DafnyAdapter.DafnyHelper;

namespace DafnyLanguageServer
{
    public class VerificationService
    {
        private readonly ILanguageServer _router;
        private IDafnyHelper _dafnyHelper;

        public void SetDafnyHelper(IDafnyHelper helper)
        {
            _dafnyHelper = helper;
        }

        private void SetDefaultDafnyHelper(DafnyFile file)
        {
            if (_dafnyHelper is null)
            {
                _dafnyHelper = new DafnyHelper(new string[]{}, file.Filepath, file.Sourcecode);
            }
           
        }

        public VerificationService(ILanguageServer router)
        {
            _router = router;
        }

        public void Verify(DafnyFile file)
        {
            SetDefaultDafnyHelper(file);

            // im Plugin das aktuelle Dokument setzen für die Statusbar
            _router.Window.SendNotification("activeVerifiyingDocument", file.Filepath);

            var helper = DafnyVerify(file);
            var diagnostics = CreateDafnyDiagnostics(helper.Errors, file.Filepath, file.Sourcecode);

            PublishDiagnosticsParams p = new PublishDiagnosticsParams
            {
                Uri = file.Uri,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            };
            _router.Document.PublishDiagnostics(p);
            SendErrornumberToClient(diagnostics.Count);
        }

        private void SendErrornumberToClient(int counted)
        {
            _router.Window.SendNotification("updateStatusbar", counted);
        }

        public IDafnyHelper DafnyVerify(DafnyFile file)
        {
            if (!_dafnyHelper.Verify())
            {
                throw new ArgumentException("Failed to verify document."); // 2do: Während des schreibens ist das doc immer wieder invalid... exception ist etwas zu krass imho
            }
            return _dafnyHelper;
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
