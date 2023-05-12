using Moonlight.App.Database.Entities;
using Moonlight.App.Repositories;

namespace Moonlight.App.Services;

public class SmartDeployService
{
    private readonly NodeRepository NodeRepository;
    private readonly Repository<CloudPanel> CloudPanelRepository;
    private readonly Repository<Shard> ShardRepository;
    private readonly WebSpaceService WebSpaceService;

    public SmartDeployService(
        NodeRepository nodeRepository,
        WebSpaceService webSpaceService,
        Repository<CloudPanel> cloudPanelRepository,
        Repository<Shard> shardRepository)
    {
        NodeRepository = nodeRepository;
        WebSpaceService = webSpaceService;
        CloudPanelRepository = cloudPanelRepository;
        ShardRepository = shardRepository;
    }

    public async Task<CloudPanel?> GetCloudPanel()
    {
        var result = new List<CloudPanel>();
        
        foreach (var cloudPanel in CloudPanelRepository.Get().ToArray())
        {
            if (await WebSpaceService.IsHostUp(cloudPanel))
                result.Add(cloudPanel);
        }

        return result.FirstOrDefault();
    }

    public async Task<Shard?> GetShard(int allocations = 1, ShardSpace? shardSpace = null)
    {
        //TODO Implement
        return ShardRepository.Get().First();
    }
}