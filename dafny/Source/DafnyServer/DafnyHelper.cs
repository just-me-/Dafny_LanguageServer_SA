using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

using Microsoft.Boogie;
using DafnyServer;
using Bpl = Microsoft.Boogie;

namespace Microsoft.Dafny
{
    // FIXME: This should not be duplicated here
    public class DafnyConsolePrinter : ConsolePrinter
    {
        public override void ReportBplError(IToken tok, string message, bool error, TextWriter tw, string category = null)
        {
            // Dafny has 0-indexed columns, but Boogie counts from 1
            var realigned_tok = new Token(tok.line, tok.col - 1);
            realigned_tok.kind = tok.kind;
            realigned_tok.pos = tok.pos;
            realigned_tok.val = tok.val;
            realigned_tok.filename = tok.filename;
            base.ReportBplError(realigned_tok, message, error, tw, category);

            if (tok is Dafny.NestedToken)
            {
                var nt = (Dafny.NestedToken)tok;
                ReportBplError(nt.Inner, "Related location", false, tw);
            }
        }
    }

    public class DafnyHelper
    {
        private string fname;
        private string source;
        private string[] args;

        private readonly Dafny.ErrorReporter reporter;
        private Dafny.Program dafnyProgram;
        private IEnumerable<Tuple<string, Bpl.Program>> boogiePrograms;

        public List<ErrorInformation> Errors { get; } = new List<ErrorInformation>();

        private void addErrorToList(ErrorInformation e)
        {
            Errors.Add(e);
        }
        
        public DafnyHelper(string[] args, string fname, string source)
        {
            this.args = args;
            this.fname = fname;
            this.source = source;
            this.reporter = new Dafny.ConsoleErrorReporter();
        }

        //Note: Diesen CTOR hat Tom hinzugefügt, brauchen wir wahrscheinlich gar net mehr.
        public DafnyHelper(string[] args, string fname, string source, ErrorReporter reporter)
        {
            this.args = args;
            this.fname = fname;
            this.source = source;
            this.reporter = reporter;
        }

        public bool Verify()
        {
            ServerUtils.ApplyArgs(args, reporter);
            return Parse() && Resolve() && Translate() && Boogie();
        }

        private bool Parse()
        {
            Dafny.ModuleDecl module = new Dafny.LiteralModuleDecl(new Dafny.DefaultModuleDecl(), null);
            Dafny.BuiltIns builtIns = new Dafny.BuiltIns();
            var success = (Dafny.Parser.Parse(source, fname, fname, null, module, builtIns, new Dafny.Errors(reporter)) == 0 &&
                           Dafny.Main.ParseIncludes(module, builtIns, new List<string>(), new Dafny.Errors(reporter)) == null);
            if (success)
            {
                dafnyProgram = new Dafny.Program(fname, module, builtIns, reporter);
            }
            //Console.WriteLine("Parsed");
            return success;
        }

        private bool Resolve()
        {
            var resolver = new Dafny.Resolver(dafnyProgram);
            resolver.ResolveProgram(dafnyProgram);
            //Console.WriteLine("Resolved");
            return reporter.Count(ErrorLevel.Error) == 0;
        }

        private bool Translate()
        {
            boogiePrograms = Translator.Translate(dafnyProgram, reporter,
                new Translator.TranslatorFlags() { InsertChecksums = true, UniqueIdPrefix = fname }); // FIXME how are translation errors reported?
            //Console.WriteLine("Translated");
            return true;
        }

        private bool BoogieOnce(string moduleName, Bpl.Program boogieProgram)
        {
            if (boogieProgram.Resolve() == 0 && boogieProgram.Typecheck() == 0)
            { //FIXME ResolveAndTypecheck?
                ExecutionEngine.EliminateDeadVariables(boogieProgram);
                ExecutionEngine.CollectModSets(boogieProgram);
                ExecutionEngine.CoalesceBlocks(boogieProgram);
                ExecutionEngine.Inline(boogieProgram);

                //NOTE: We could capture errors instead of printing them(pass a delegate instead of null)

                var ps = new PipelineStatistics();
                var stringteil = "ServerProgram_" + moduleName;
                var time = DateTime.UtcNow.Ticks.ToString();
                var a = ExecutionEngine.InferAndVerify(boogieProgram, ps, stringteil, addErrorToList, time);
                switch (a)
                {
                    case PipelineOutcome.Done:
                    case PipelineOutcome.VerificationCompleted:
                        //Console.WriteLine("BoogieOnced");
                        return true;
                }
            }

            return false;
        }

        private bool Boogie()
        {
            var isVerified = true;
            foreach (var boogieProgram in boogiePrograms)
            {
                isVerified = isVerified && BoogieOnce(boogieProgram.Item1, boogieProgram.Item2);  //TODO Can be made schöner.
            }
            return isVerified;
        }

        public List<SymbolTable.SymbolInformation> Symbols()
        { 
            ServerUtils.ApplyArgs(args, reporter);
            if (Parse() && Resolve())
            {
                var symbolTable = new SymbolTable(dafnyProgram);
                return symbolTable.CalculateSymbols(); 
            }
            else
            {
                return new List<SymbolTable.SymbolInformation>(); 
            }
        }

        public void CounterExample()
        {
            var listArgs = args.ToList();
            listArgs.Add("/mv:" + CounterExampleProvider.ModelBvd);
            ServerUtils.ApplyArgs(listArgs.ToArray(), reporter);
            try
            {
                if (Parse() && Resolve() && Translate())
                {
                    var counterExampleProvider = new CounterExampleProvider();
                    foreach (var boogieProgram in boogiePrograms)
                    {
                        RemoveExistingModel();
                        BoogieOnce(boogieProgram.Item1, boogieProgram.Item2);
                        var model = counterExampleProvider.LoadCounterModel();
                        Console.WriteLine("COUNTEREXAMPLE_START " + ConvertToJson(model) + " COUNTEREXAMPLE_END");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error collection models: " + e.Message);
            }
        }

        private void RemoveExistingModel()
        {
            if (File.Exists(CounterExampleProvider.ModelBvd))
            {
                File.Delete(CounterExampleProvider.ModelBvd);
            }
        }

        public void DotGraph()
        {
            ServerUtils.ApplyArgs(args, reporter);

            if (Parse() && Resolve() && Translate())
            {
                foreach (var boogieProgram in boogiePrograms)
                {
                    BoogieOnce(boogieProgram.Item1, boogieProgram.Item2);

                    foreach (var impl in boogieProgram.Item2.Implementations)
                    {
                        using (StreamWriter sw = new StreamWriter(fname + impl.Name + ".dot"))
                        {
                            sw.Write(boogieProgram.Item2.ProcessLoops(impl).ToDot());
                        }
                    }
                }
            }
        }

        private static string ConvertToJson<T>(T data)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, data);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }
    }
}
