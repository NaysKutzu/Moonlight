namespace Moonlight.App.ApiClients.Shards.Resources;

public class DockerMetrics
{
    public Container[] Containers { get; set; } = Array.Empty<Container>();
}