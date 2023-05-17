using Moonlight.App.ApiClients.ShardProxies;
using Moonlight.App.Database.Entities;

namespace Moonlight.App.Services.Shards;

public class ShardProxyService
{
    private readonly ShardProxyApiHelper ShardProxyApiHelper;

    public ShardProxyService(ShardProxyApiHelper shardProxyApiHelper)
    {
        ShardProxyApiHelper = shardProxyApiHelper;
    }

    public async Task AddNat(ShardProxy proxy, string target, int targetPort)
    {
        await ShardProxyApiHelper.Post(proxy, $"firewall/nat/{target}/{targetPort}");
    }
    
    public async Task RemoveNat(ShardProxy proxy, string target, int targetPort)
    {
        await ShardProxyApiHelper.Delete(proxy, $"firewall/nat/{target}/{targetPort}");
    }

    public async Task FlushNat(ShardProxy proxy)
    {
        await ShardProxyApiHelper.Delete(proxy, $"firewall/nat");
    }

    public async Task<bool> IsHostUp(ShardProxy proxy)
    {
        try
        {
            await ShardProxyApiHelper.Get(proxy, "");
        }
        catch (ShardProxyException e)
        {
            if (e.StatusCode == 404)
                return true;
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
}