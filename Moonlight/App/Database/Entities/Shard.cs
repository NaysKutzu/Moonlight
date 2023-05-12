namespace Moonlight.App.Database.Entities;

public class Shard
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Fqdn { get; set; } = "";
    public string TokenId { get; set; } = "";
    public string Token { get; set; } = "";
    public int SftpPort { get; set; }
    public int HttpPort { get; set; }
    public int ShardPort { get; set; }
    public bool Ssl { get; set; } = false;

    public List<ShardAllocation> Allocations { get; set; } = new();
}