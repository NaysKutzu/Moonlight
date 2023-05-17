using Moonlight.App.Database.Entities;
using RestSharp;

namespace Moonlight.App.ApiClients.ShardProxies;

public class ShardProxyApiHelper
{
    private readonly RestClient Client;

    public ShardProxyApiHelper()
    {
        Client = new();
    }
    
    public async Task Get(ShardProxy proxy, string resource)
    {
        var request = await CreateRequest(proxy, resource);

        request.Method = Method.Get;
        
        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new ShardProxyException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            else
            {
                throw new Exception($"An internal error occured: {response.ErrorMessage}");
            }
        }
    }
    
    public async Task Post(ShardProxy proxy, string resource)
    {
        var request = await CreateRequest(proxy, resource);

        request.Method = Method.Post;
        
        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new ShardProxyException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            else
            {
                throw new Exception($"An internal error occured: {response.ErrorMessage}");
            }
        }
    }
    
    public async Task Delete(ShardProxy shard, string resource)
    {
        var request = await CreateRequest(shard, resource);

        request.Method = Method.Delete;
        
        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new ShardProxyException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            else
            {
                throw new Exception($"An internal error occured: {response.ErrorMessage}");
            }
        }
    }

    private Task<RestRequest> CreateRequest(ShardProxy proxy, string resource)
    {
        var url = $"http://{proxy.Fqdn}:9999/"; //TODO: Make port a variable
        
        RestRequest request = new(url + resource);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", proxy.Key);
        
        return Task.FromResult(request);
    }
}