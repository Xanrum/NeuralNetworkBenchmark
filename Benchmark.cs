using Refit;
using System.Diagnostics;
using System.Text.Json;


public static class Benchmark
{
    public static async Task Bench()
    {
        Random rand = new Random(0);
        var inputsCount = 1000;
        var inputCount = 60;
        double[] inputs = new double[inputsCount*inputCount];
        for (var i = 0; i < inputsCount; i++)
        {
            for (var j = 0; j < inputCount; j++)
            {
                inputs[i*inputCount+j] = rand.NextDouble() * 2 - 1;
            }
        }

        var model = new[]
        {
            inputCount,
            300,
            300,
            300,
            1
        };
        var synapsesCount = 200;
        var synapsCount = model[0] * model[1] + model[1] * model[2] + model[2] * model[3]+ model[3] * model[4];
        double[][] synapses = new double[synapsesCount][];
        for (var i = 0; i < synapsesCount; i++)
        {
            synapses[i] = new double[synapsCount];
            for (var j = 0; j < synapsCount; j++)
            {
                synapses[i][j] = rand.NextDouble() * 2 - 1;
            }
        }
        var httpClient = new HttpClient(new HttpClientHandler()
        {
            MaxConnectionsPerServer = 32
        })
        {
            BaseAddress = new ("http://192.168.1.10:5018")
        };
        var api = RestService.For<IServer>(httpClient);
        await api.Load(new()
        {
            Key = 1,
            Data = inputs,
            Indexes = Enumerable.Range(0, inputsCount).Select(p => p * inputCount).ToArray()
        });

        var inputNames = Enumerable.Range(0, inputsCount).Select(p => (long)p).ToArray();


        Console.WriteLine("start");
        var sw = Stopwatch.StartNew();
        var tasks = synapses.Select(p => Task.Run(async () => {
            var ms = new MemoryStream();
            PayloadFormatter.Format(ms, 1, p, model);
            ms.Position = 0;
            var res = await httpClient.PostAsync("/neural/calc", new StreamContent(ms));
            return await JsonSerializer.DeserializeAsync<CalcResponse>(await res.Content.ReadAsStreamAsync());
        })).ToList();
        await Task.WhenAll(tasks);
        sw.Stop();
        Console.WriteLine(sw.Elapsed);
    }
}