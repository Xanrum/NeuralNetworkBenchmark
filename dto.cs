using Refit;

public class CalcRequest
{
    public int[] Model { get; set; }
    public string[] Input { get; set; }
    public double[] Synapses { get; set; }
}

public class CalcResponse
{
    public List<string> UnkownInputs { get; set; }
    public double[][] Outputs { get; set; }
}

public class LoadInputsRequest
{
    public List<LoadInputsRequestItem> Data { get; set; }
}

public class LoadInputsRequestItem {
    public string Key { get; set; }
    public double[] Inputs { get; set; }
}

public interface IServer
{
    [Post("/neural/calc")]
    Task<CalcResponse> Calc(CalcRequest request);
    
    [Post("/neural/calc")]
    Task CalcNoResponse(CalcRequest request);
    
    [Post("/neural/load")]
    Task Load(LoadInputsRequest request);
}