using System.Threading;
using System.Threading.Tasks;
using DafnyServer;
using Microsoft.Dafny;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace DafnyLanguageServerCore
{
    internal class CompletionHandler : ICompletionHandler
    {
        private const string PackageReferenceElement = "PackageReference";
        private const string IncludeAttribute = "Include";
        private const string VersionAttribute = "Version";
        private static readonly char[] EndElement = new[] { '>' };

        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;
        private readonly NuGetAutoCompleteService _nuGetService;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.dfy"
            }
        );

        private CompletionCapability _capability;

        public CompletionHandler(ILanguageServer router, BufferManager bufferManager, NuGetAutoCompleteService nuGetService)
        {
            _router = router;
            _bufferManager = bufferManager;
            _nuGetService = nuGetService;
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions
            {
                DocumentSelector = _documentSelector,
                ResolveProvider = false
            };
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToString();
            var buffer = _bufferManager.GetTextFromBuffer(documentPath);

            var demotext = "i'm the new text";
            var demotext2 = "You can do this !!!!  ;-) <3 <3 <3 :-) Keep trying!";

            return await Task.Run(() =>
            {

                if (buffer == null)
                {
                    return new CompletionList();
                }

                var citem1 = new CompletionItem
                {
                    Label = "Insert a new Text",
                    Kind = CompletionItemKind.Reference,
                    TextEdit = new TextEdit
                    {
                        NewText = demotext,
                        Range = new Range(
                            new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character
                            }, new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character + demotext.Length
                            })
                    }

                };

                var citem2 = new CompletionItem
                {
                    Label = "Let me cheer you up",
                    Kind = CompletionItemKind.Reference,
                    TextEdit = new TextEdit
                    {
                        NewText = demotext2,
                        Range = new Range(
                            new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character
                            }, new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character + demotext2.Length
                            })
                    }

                };

                string version = VersionCheck.CurrentVersion();

                var citem3 = new CompletionItem
                {
                    Label = "Insert Server Version here",
                    Kind = CompletionItemKind.Reference,
                    TextEdit = new TextEdit
                    {
                        NewText = version,
                        Range = new Range(
                            new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character
                            }, new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character + version.Length
                            })
                    }


                };

                string[] args = new string[]{};
                string filename = request.TextDocument.Uri.ToString();
                string sourcecode = _bufferManager.GetTextFromBuffer(filename);

                bool isValid = new DafnyHelper(args, filename, sourcecode, new ErrorReporterSink()).Verify();
                

                var citem4 = new CompletionItem
                {
                    Label = "is this document valid??",
                    Kind = CompletionItemKind.Reference,
                    TextEdit = new TextEdit
                    {
                        NewText = isValid.ToString(),
                        Range = new Range(
                            new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character
                            }, new Position
                            {
                                Line = request.Position.Line,
                                Character = request.Position.Character + isValid.ToString().Length
                            })
                    }


                };
                return new CompletionList(citem1, citem2, citem3, citem4);

            });



        }

        private static int GetPosition(string buffer, int line, int col)
        {
            var position = 0;
            for (var i = 0; i < line; i++)
            {
                position = buffer.IndexOf('\n', position) + 1;
            }
            return position + col;
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
        }
    }
}
