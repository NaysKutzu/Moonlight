using System.Net;
using Logging.Net;
using Microsoft.EntityFrameworkCore;
using Moonlight.App.ApiClients.Wings;
using Moonlight.App.Database.Entities;
using Moonlight.App.Events;
using Moonlight.App.Exceptions;
using Moonlight.App.Repositories;
using Moonlight.App.Services.Shards;

namespace Moonlight.App.Services.Background;

public class ShardServerService
{
    private readonly Dictionary<Server, Shard> CurrentServerShards = new();
    private readonly List<Server> CurrentLockedServers = new();
    private readonly EventSystem Event;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public ShardServerService(IServiceScopeFactory serviceScopeFactory, EventSystem eventSystem)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Event = eventSystem;
    }

    public Task<Shard?> GetCurrentShard(Server server)
    {
        lock (CurrentServerShards)
        {
            if (CurrentServerShards.Any(x => x.Key.Id == server.Id))
                return Task.FromResult<Shard?>(
                    CurrentServerShards.First(x => x.Key.Id == server.Id).Value);
            else
                return Task.FromResult<Shard?>(null!);
        }
    }

    public async Task StartServer(Server s, Shard? useShard = null)
    {
        if (await IsLocked(s))
            throw new DisplayException("Server is currently locked during shard transfer");
        
        await LockServer(s);

        try
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var serverRepo = scope.ServiceProvider.GetRequiredService<Repository<Server>>();
            var shardSpaceRepo = scope.ServiceProvider.GetRequiredService<Repository<ShardSpace>>();
            var shardRepo = scope.ServiceProvider.GetRequiredService<Repository<Shard>>();

            var serverService = scope.ServiceProvider.GetRequiredService<ServerService>();
            var shardService = scope.ServiceProvider.GetRequiredService<ShardService>();
            var shardProxyService = scope.ServiceProvider.GetRequiredService<ShardProxyService>();

            var server = serverRepo
                .Get()
                .Include(x => x.Shard)
                .Include(x => x.Allocations)
                .First(x => x.Id == s.Id);

            if (!await serverService.IsHostUp(server))
                throw new DisplayException("The servers home shard is offline");

            var details = await serverService.GetDetails(server);

            if (details.State != "offline")
                throw new DisplayException("Unable to start a server which is not offline");

            // Remove old shard state if needed
            lock (CurrentServerShards)
            {
                var possibleData = CurrentServerShards.Keys.FirstOrDefault(x => x.Id == server.Id);

                if (possibleData != null)
                    CurrentServerShards.Remove(possibleData);
            }

            var shardSpace = shardSpaceRepo
                .Get()
                .Include(x => x.Shards)
                .Include(x => x.Proxy)
                .First(x => x.Shards.Any(y => y.Id == server.Shard.Id));

            if (shardSpace.Proxy == null)
                throw new DisplayException("No proxy configured for this shard space");

            if (!await shardProxyService.IsHostUp(shardSpace.Proxy))
                throw new DisplayException("The shard proxy of the current shard space is offline");

            Shard shard;

            if (useShard == null)
            {
                // TODO: Add smart deploy like shard detection
                shard = shardRepo.Get().First();
            }
            else
            {
                shard = useShard;
            }

            // We remove every nat rule which could possibly exist
            // TODO: Make this section more efficient, like calling the api async or only removing rules that exist
            foreach (var allocation in server.Allocations)
            {
                foreach (var aShard in shardRepo.Get().ToArray())
                {
                    try
                    {
                        var shardIp = (await Dns.GetHostAddressesAsync(aShard.Fqdn)).First().ToString();

                        await shardProxyService.RemoveNat(shardSpace.Proxy, shardIp, allocation.Port);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn($"Error removing nat rule for {aShard.Name}");
                        Logger.Warn(e);
                    }
                }
            }

            // Now we can add the correct nat rules
            foreach (var allocation in server.Allocations)
            {
                try
                {
                    var shardIp = (await Dns.GetHostAddressesAsync(shard.Fqdn)).First().ToString();

                    await shardProxyService.AddNat(shardSpace.Proxy, shardIp, allocation.Port);
                }
                catch (Exception e)
                {
                    Logger.Warn($"Error while creating nat rule for server {server.Uuid} on shard {shard.Name}");
                    Logger.Warn(e);

                    throw new DisplayException("An unknown error occured while creating server network environment");
                }
            }

            // MOUNTING

            var volumePath = $"/var/lib/pterodactyl/volumes/{server.Uuid.ToString().ToLower()}";
        
            // Unmount all possible mounts
            foreach (var aShard in shardRepo.Get().ToArray())
            {
                try
                {
                    await shardService.Unmount(aShard, volumePath);
                }
                catch (Exception e)
                {
                    // ignored
                
                    //Logger.Warn("Error unmounting server");
                    //Logger.Warn(e);
                }
            }

            // Only if it is not the home shard, we need to mount
            if (shard.Id != server.Shard.Id)
            {
                try
                {
                    await shardService.Mount(shard, server.Shard.Fqdn, volumePath, volumePath);
                }
                catch (Exception e)
                {
                    Logger.Warn("Error mounting server directory");
                    Logger.Warn(e);

                    throw new DisplayException("An unknown error occurred while mounting server");
                }
            }

            lock (CurrentServerShards)
            {
                CurrentServerShards.Add(server, shard);
            }

            try
            {
                // Because of wings not able to start a server which is not in its local cache,
                // we call the create endpoint here. Because of the check for a installing status
                // when providing wings with the server install script, we can cancel any unwanted
                // reinstall.
                await serverService.SyncWings(server);
                //TODO: Check if we can use the start after install flag instead of the start power states
            }
            catch (Exception)
            {
                // Remove shard state if needed, because we encountered an error
                lock (CurrentServerShards)
                {
                    var possibleData = CurrentServerShards.Keys.FirstOrDefault(x => x.Id == server.Id);

                    if (possibleData != null)
                        CurrentServerShards.Remove(possibleData);
                }
            
                throw;
            }
        
            // At this point, we can be sure that everything is setup for the current/new shard
            // to start the server so we send a sharded start signal to the shard
            await serverService.SetPowerState(server, PowerSignal.Start);
        }
        catch (Exception)
        {
            await UnlockServer(s);
            throw;
        }
    }

    public async Task UnlockServer(Server s)
    {
        lock (CurrentLockedServers)
        {
            if (CurrentLockedServers.Any(x => x.Id == s.Id))
            {
                CurrentLockedServers.RemoveAll(x => x.Id == s.Id);
            }
        }
        
        await Event.Emit($"server.{s.Uuid}.updateShardStatus", s);
    }
    
    public async Task LockServer(Server s)
    {
        lock (CurrentLockedServers)
        {
            if(CurrentLockedServers.All(x => x.Id != s.Id))
                CurrentLockedServers.Add(s);
        }
        
        await Event.Emit($"server.{s.Uuid}.updateShardStatus", s);
    }

    public Task<bool> IsLocked(Server s)
    {
        lock (CurrentLockedServers)
        {
            return Task.FromResult(CurrentLockedServers.Any(x => x.Id == s.Id));
        }
    }

    public async Task RescanShards()
    {
        //TODO: This function will scan every shard for docker containers and recreates the CurrentServerShards with the data
    }
}