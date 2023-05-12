using Moonlight.App.Database.Entities;
using Newtonsoft.Json;
using RestSharp;

namespace Moonlight.App.ApiClients.Shards;

public class ShardApiHelper
{
    private readonly RestClient Client;

    public ShardApiHelper()
    {
        Client = new();
    }
    
    public async Task<T> Get<T>(Shard shard, string resource)
    {
        var request = await CreateRequest(shard, resource);

        var response = await Client.GetAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new ShardException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            else
            {
                throw new Exception($"An internal error occured: {response.ErrorMessage}");
            }
        }

        return JsonConvert.DeserializeObject<T>(response.Content!)!;
    }

    private Task<RestRequest> CreateRequest(Shard shard, string resource)
    {
        var url = $"http://{shard.Fqdn}:{shard.ShardPort}/";
        
        RestRequest request = new(url + resource);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", shard.Token);
        
        return Task.FromResult(request);
    }
}