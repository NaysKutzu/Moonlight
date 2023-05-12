namespace Moonlight.App.ApiClients.Shards.Resources;

public class Container
{
    public string Name { get; set; } = "";
    public long Memory { get; set; }
    public double Cpu { get; set; }
    public long NetworkIn { get; set; }
    public long NetworkOut { get; set; }
}