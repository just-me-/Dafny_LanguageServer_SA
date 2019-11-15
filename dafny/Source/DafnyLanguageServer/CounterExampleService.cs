using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Microsoft.Dafny;
using DafnyHelper = DafnyLanguageServer.DafnyAdapter.DafnyHelper;

namespace DafnyLanguageServer
{
    public class CounterExampleService
    {
        public string Filename { get; }
        public string[] Args { get; } = { };

        public string ProgramSource { get; }
    



        public CounterExampleService(string filename, string programSource)

        {
            ProgramSource = programSource;
            Filename = filename;
        }


        public Task < CounterExampleResults> ProvideCounterExamples()
        {
            return Task.Run(() =>
            {

                var allCounterExamplesReturnContainer = new CounterExampleResults();


                var helper = new DafnyHelper(Args, Filename, ProgramSource);
                var models = helper.CounterExample();

                if (models.Count == 0)
                {
                    return allCounterExamplesReturnContainer;
                }

                var states = models[0].States;

                for (int i = 2; i < states.Count; i++)
                {
                    var entry = states[i];
                    var variables = entry.Variables;

                    CounterExampleResult currentCounterExample = new CounterExampleResult();

                    currentCounterExample.Col = entry.Column;
                    currentCounterExample.Line = entry.Line;

                    foreach (var variable in variables)
                    {
                        currentCounterExample.Variables.Add(variable.Name, variable.Value);
                    }

                    allCounterExamplesReturnContainer.CounterExamples.Add(currentCounterExample);

                }

                return allCounterExamplesReturnContainer;

            });


        }
    }
}
