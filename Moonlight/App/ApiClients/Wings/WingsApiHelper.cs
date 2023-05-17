using Moonlight.App.ApiClients.Shards;
using Moonlight.App.Database.Entities;
using Moonlight.App.Exceptions;
using Moonlight.App.Services;
using Moonlight.App.Services.Shards;
using Newtonsoft.Json;
using RestSharp;

namespace Moonlight.App.ApiClients.Wings;

public class WingsApiHelper
{
    private readonly RestClient Client;
    private readonly ShardService ShardService;

    public WingsApiHelper(ShardService shardService)
    {
        ShardService = shardService;
        Client = new();
    }

    // GET
    
    public async Task<T> GetSharded<T>(Server server, string resource)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Get;

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            
            throw new Exception($"An internal error occured: {response.ErrorMessage}");
        }

        return JsonConvert.DeserializeObject<T>(response.Content!)!;
    }
    public async Task<T> Get<T>(Server server, string resource)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Get;

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            
            throw new Exception($"An internal error occured: {response.ErrorMessage}");
        }

        return JsonConvert.DeserializeObject<T>(response.Content!)!;
    }
    
    // GET RAW
    
    public async Task<string> GetRawSharded(Server server, string resource)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Get;

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            else
            {
                throw new Exception($"An internal error occured: {response.ErrorMessage}");
            }
        }

        return response.Content!;
    }
    public async Task<string> GetRaw(Server server, string resource)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Get;

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
                    $"An error occured: ({response.StatusCode}) {response.Content}",
                    (int)response.StatusCode
                );
            }
            else
            {
                throw new Exception($"An internal error occured: {response.ErrorMessage}");
            }
        }

        return response.Content!;
    }
    
    // POST
    
    public async Task<T> PostSharded<T>(Server server, string resource, object? body)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Post;

        request.AddParameter("text/plain",
            JsonConvert.SerializeObject(body),
            ParameterType.RequestBody
        );

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    public async Task<T> Post<T>(Server server, string resource, object? body)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Post;

        request.AddParameter("text/plain",
            JsonConvert.SerializeObject(body),
            ParameterType.RequestBody
        );

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    
    // POST (No response)
    
    public async Task PostSharded(Server server, string resource, object? body)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Post;

        if(body != null)
            request.AddParameter("text/plain", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    public async Task Post(Server server, string resource, object? body)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Post;

        if(body != null)
            request.AddParameter("text/plain", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    
    // POST RAW
    
    public async Task PostRawSharded(Server server, string resource, object body)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Post;

        request.AddParameter("text/plain", body, ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    public async Task PostRaw(Server server, string resource, object body)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Post;

        request.AddParameter("text/plain", body, ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    
    // DELETE
    
    public async Task DeleteSharded(Server server, string resource, object? body)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Delete;

        if(body != null)
            request.AddParameter("text/plain", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    public async Task Delete(Server server, string resource, object? body)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Delete;

        if(body != null)
            request.AddParameter("text/plain", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    
    // PUT
    
    public async Task PutSharded(Server server, string resource, object? body)
    {
        var request = await CreateShardedRequest(server, resource);

        request.Method = Method.Put;

        request.AddParameter("text/plain", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    public async Task Put(Server server, string resource, object? body)
    {
        var request = await CreateRequest(server, resource);

        request.Method = Method.Put;

        request.AddParameter("text/plain", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

        var response = await Client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            if (response.StatusCode != 0)
            {
                throw new WingsException(
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
    
    private async Task<RestRequest> CreateShardedRequest(Server server, string resource)
    {
        var shard = await ShardService.GetCurrentShard(server);
        
        var url = (shard.Ssl ? "https" : "http") + $"://{shard.Fqdn}:{shard.HttpPort}/" + resource;

        var request = new RestRequest(url)
        {
            Timeout = 60 * 15
        };

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", "Bearer " + shard.Token);

        return request;
    }
    private async Task<RestRequest> CreateRequest(Server server, string resource)
    {
        var shard = await ShardService.GetHomeShard(server);
        
        var url = (shard.Ssl ? "https" : "http") + $"://{shard.Fqdn}:{shard.HttpPort}/" + resource;

        var request = new RestRequest(url)
        {
            Timeout = 60 * 15
        };

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", "Bearer " + shard.Token);

        return request;
    }
}