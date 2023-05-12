using Moonlight.App.Database.Entities;

namespace Moonlight.App.Services.Background;

public class ShardServerService
{
    private readonly Dictionary<Server, Shard> CurrentServerShards = new();

    public Task<Shard?> GetCurrentShard(Server server)
    {
        lock (CurrentServerShards)
        {
            if (CurrentServerShards.TryGetValue(server, out var shard))
            {
                return Task.FromResult(shard!)!;
            }
            else
            {
                return Task.FromResult<Shard?>(null);
            }
        }
    }
    
    public async Task RescanShards()
    {
        //TODO: This function will scan every shard for docker containers and recreates the CurrentServerShards with the data
    }
}