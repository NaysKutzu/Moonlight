using Microsoft.EntityFrameworkCore;
using Moonlight.App.ApiClients.Shards;
using Moonlight.App.ApiClients.Shards.Resources;
using Moonlight.App.Database.Entities;
using Moonlight.App.Repositories;
using Moonlight.App.Services.Background;

namespace Moonlight.App.Services;

public class ShardService
{
    private readonly Repository<Server> ServerRepository;
    private readonly ShardServerService ShardServerService;
    private readonly ShardApiHelper ShardApiHelper;

    public ShardService(
        Repository<Server> serverRepository,
        ShardApiHelper shardApiHelper,
        ShardServerService shardServerService)
    {
        ServerRepository = serverRepository;
        ShardApiHelper = shardApiHelper;
        ShardServerService = shardServerService;
    }

    // Shard finding
    public async Task<Shard> GetCurrentShard(Server server)
    {
        var shard = await ShardServerService.GetCurrentShard(server);

        if (shard == null)
        {
            shard = ServerRepository
                .Get()
                .Include(x => x.Shard)
                .First(x => x.Id == server.Id).Shard;
        }

        return shard;
    }
    public Task<Shard> GetHomeShard(Server server)
    {
        if (server.Shard == null)
        {
            var shard = ServerRepository
                .Get()
                .Include(x => x.Shard)
                .First(x => x.Id == server.Id).Shard;

            return Task.FromResult(shard);
        }

        return Task.FromResult(server.Shard);
    }
    
    // Reading shard (daemon) data

    public async Task<CpuMetrics> GetCpuMetrics(Shard shard)
    {
        return await ShardApiHelper.Get<CpuMetrics>(shard, "metrics/cpu");
    }
    
    public async Task<MemoryMetrics> GetMemoryMetrics(Shard shard)
    {
        return await ShardApiHelper.Get<MemoryMetrics>(shard, "metrics/memory");
    }
    
    public async Task<DiskMetrics> GetDiskMetrics(Shard shard)
    {
        return await ShardApiHelper.Get<DiskMetrics>(shard, "metrics/disk");
    }
    
    public async Task<SystemMetrics> GetSystemMetrics(Shard shard)
    {
        return await ShardApiHelper.Get<SystemMetrics>(shard, "metrics/system");
    }
    
    public async Task<DockerMetrics> GetDockerMetrics(Shard shard)
    {
        return await ShardApiHelper.Get<DockerMetrics>(shard, "metrics/docker");
    }

    public async Task<bool> IsHostUp(Shard shard)
    {
        try
        {
            await GetSystemMetrics(shard);
        }
        catch (Exception)
        {
            // ignored
        }

        return false;
    }
}