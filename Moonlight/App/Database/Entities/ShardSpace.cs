namespace Moonlight.App.Database.Entities;

public class ShardSpace
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ShardProxy? Proxy { get; set; }
    public List<Shard> Shards { get; set; } = new();
}