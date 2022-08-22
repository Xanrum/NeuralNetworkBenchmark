using System.Text;

public static class PayloadFormatter
{
    public static void Format(Stream stream, long[] inputs, double[] synapses, int[] model)
    {
        using var writer = new BinaryWriter(stream, Encoding.Default, true);
        writer.Write(model.Length);
        foreach (var t in model)
        {
            writer.Write(t);
        }
        writer.Write(inputs.Length);
        foreach (var t in inputs)
        {
            writer.Write(t);
        }

        writer.Write(synapses.Length);
        foreach (var t in synapses)
        {
            writer.Write(t);
        }
    }
}