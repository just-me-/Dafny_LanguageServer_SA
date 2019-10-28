﻿using System;
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
    static class VerificationService
    {
        private static readonly int MAGICLINEENDING = 100; // 2Do evt dynamisch anpassen an jeweilige Zeilenlänge 

        static private DafnyHelper DafnyVerify(DafnyFile file)
        {
            string[] args = new string[] { };
            DafnyHelper helper = new DafnyHelper(args, file.Filepath, file.Sourcecode);
            if (!helper.Verify())
            {
                throw new ArgumentException("Failed to verify document.");
            }
            return helper; 
        }

        static private Collection<Diagnostic> CreateDafnyDiagnostics(DafnyHelper helper, DafnyFile file)
        {
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
                d.Source = file.Filepath;

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

            return diagnostics; 
        }

        static public void Verify(ILanguageServer router, DafnyFile file)
        {
            // im Plugin das aktuelle Dokument setzen für die Statusbar
            router.Window.SendNotification("activeVerifiyingDocument", file.Filepath);

            var helper = DafnyVerify(file);
            var diagnostics = CreateDafnyDiagnostics(helper, file);
            
            PublishDiagnosticsParams p = new PublishDiagnosticsParams
            {
                Uri = file.Uri,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            };
            router.Document.PublishDiagnostics(p);
            router.Window.SendNotification("updateStatusbar", diagnostics.Count);
        }
    }
}
