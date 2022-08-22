using Refit;


public static class Tester
{
    public static async Task Test()
    {


        Random rand = new Random(0);
        var inputsCount = 10;
        var inputCount = 3;
        double[][] inputs = new double[inputsCount][];
        for (var i = 0; i < inputsCount; i++)
        {
            inputs[i] = new double[inputCount];
            for (var j = 0; j < inputCount; j++)
            {
                inputs[i][j] = rand.NextDouble() * 2 - 1;
            }
        }

        var model = new[]
        {
            inputCount,
            3,
            4,
            2
        };
        var synapsesCount = 10;
        var synapsCount = model[0] * model[1] + model[1] * model[2] + model[2] * model[3];
        double[][] synapses = new double[synapsesCount][];
        for (var i = 0; i < synapsesCount; i++)
        {
            synapses[i] = new double[synapsCount];
            for (var j = 0; j < synapsCount; j++)
            {
                synapses[i][j] = rand.NextDouble() * 2 - 1;
            }
        }

        var api = RestService.For<IServer>("http://192.168.1.10:5018");
        await api.Load(new()
        {
            Data = inputs.Select((p, i) => new LoadInputsRequestItem()
            {
                Key = "sample" + i,
                Inputs = p
            }).ToList()
        });

        var inputNames = Enumerable.Range(0, inputsCount).Select(p => "sample" + p).ToArray();


        double[] Calc(double[] input, double[] synapse)
        {
            var prev = input;
            var synapseIndex = 0;
            for (var i = 1; i < model.Length; i++)
            {
                var next = new double[model[i]];
                for (int j = 0; j < model[i]; j++)
                {
                    var sum = 0d;
                    foreach (var d in prev)
                    {
                        sum += d * synapse[synapseIndex];
                        synapseIndex++;
                    }
                    next[j] = Math.Tanh(sum);
                }
                prev = next;
            }
            return prev;
        }

        foreach (var synapse in synapses)
        {
            var result = await api.Calc(new()
            {
                Model = model,
                Input = inputNames,
                Synapses = synapse
            });

            for (var i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                var apiCalc = result.Outputs[i];
                var calcResult = Calc(input, synapse);
                if (apiCalc.Zip(calcResult).Any(p => Math.Abs(p.First - p.Second) > 0.00001))
                    throw new("wrong");
            }
        }
    }
}