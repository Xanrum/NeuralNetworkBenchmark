using Refit;
using System.Text.Json.Serialization;

public class LoadInputsRequest
{
    public List<LoadInputsRequestItem> Data { get; set; }
}

public class LoadInputsRequestItem {
    public long Key { get; set; }
    public double[] Inputs { get; set; }
}

public class CalcResponse
{
    [JsonPropertyName("unkownInputs")]
    public List<long> UnkownInputs { get; set; }
    [JsonPropertyName("outputs")]
    public double[][] Outputs { get; set; }
}

public interface IServer
{
    [Post("/neural/load")]
    Task Load(LoadInputsRequest request);
}