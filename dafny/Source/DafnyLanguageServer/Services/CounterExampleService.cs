using DafnyLanguageServer.DafnyAdapter;
using System.Threading.Tasks;
using DafnyHelper = DafnyLanguageServer.DafnyAdapter.DafnyHelper;

namespace DafnyLanguageServer
{
    public class CounterExampleService
    {
        private IDafnyHelper _helper; 

        public CounterExampleService(IDafnyHelper helper)
        {
            _helper = helper;
        }

        public Task <CounterExampleResults> ProvideCounterExamples()
        {
            return Task.Run(() =>
            {
                var allCounterExamplesReturnContainer = new CounterExampleResults();
                var models = _helper.CounterExample();

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
