using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moonlight.App.Database.Entities;
using Moonlight.App.Events;
using Moonlight.App.Helpers;
using Moonlight.App.Http.Resources.Wings;
using Moonlight.App.Repositories;
using Moonlight.App.Repositories.Servers;
using Logging.Net;
using Moonlight.App.Services.Background;

namespace Moonlight.App.Http.Controllers.Api.Remote;

[Route("api/remote/servers")]
[ApiController]
public class ServersController : Controller
{
    private readonly WingsServerConverter Converter;
    private readonly ShardServerService ShardServerService;
    private readonly ServerRepository ServerRepository;
    private readonly Repository<Shard> ShardRepository;
    private readonly EventSystem Event;

    public ServersController(
        WingsServerConverter converter,
        ServerRepository serverRepository,
        EventSystem eventSystem,
        Repository<Shard> shardRepository,
        ShardServerService shardServerService)
    {
        Converter = converter;
        ServerRepository = serverRepository;
        Event = eventSystem;
        ShardRepository = shardRepository;
        ShardServerService = shardServerService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<WingsServer>>> GetServers(
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "per_page")] int perPage)
    {
        var tokenData = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var id = tokenData.Split(".")[0];
        var token = tokenData.Split(".")[1];

        var shard = ShardRepository.Get().FirstOrDefault(x => x.TokenId == id);

        if (shard == null)
            return NotFound();

        if (token != shard.Token)
            return Unauthorized();

        var servers = ServerRepository
            .Get()
            .Include(x => x.Shard)
            .Where(x => x.Shard.Id == shard.Id)
            .ToArray();

        List<WingsServer> wingsServers = new();
        int totalPages = 1;

        if (servers.Length > 0)
        {
            var slice = servers.Chunk(perPage).ToArray();
            var part = slice[page];

            foreach (var server in part)
            {
                wingsServers.Add(Converter.FromServer(server));
            }

            totalPages = slice.Length - 1;
        }

        await Event.Emit($"wings.{shard.Id}.serverList", shard);

        //Logger.Debug($"[BRIDGE] Shard '{shard.Name}' is requesting server list page {page} with {perPage} items per page");

        return PaginationResult<WingsServer>.CreatePagination(
            wingsServers.ToArray(),
            page,
            perPage,
            totalPages,
            servers.Length
        );
    }


    [HttpPost("reset")]
    public async Task<ActionResult> Reset() //TODO: Emulate fake creates with the servers running on this shard and are not located here, which can be found using the docker list thing
    {
        var tokenData = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var id = tokenData.Split(".")[0];
        var token = tokenData.Split(".")[1];

        var shard = ShardRepository.Get().FirstOrDefault(x => x.TokenId == id);

        if (shard == null)
            return NotFound();

        if (token != shard.Token)
            return Unauthorized();

        await Event.Emit($"wings.{shard.Id}.stateReset", shard);

        foreach (var server in ServerRepository
                     .Get()
                     .Include(x => x.Shard)
                     .Where(x => x.Shard.Id == shard.Id)
                     .ToArray()
                )
        {
            if (server.Installing)
            {
                server.Installing = false;
                ServerRepository.Update(server);
            }
        }

        return Ok();
    }

    [HttpGet("{uuid}")]
    public async Task<ActionResult<WingsServer>> GetServer(Guid uuid)
    {
        var tokenData = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var id = tokenData.Split(".")[0];
        var token = tokenData.Split(".")[1];

        var shard = ShardRepository.Get().FirstOrDefault(x => x.TokenId == id);

        if (shard == null)
            return NotFound();

        if (token != shard.Token)
            return Unauthorized();

        var server = ServerRepository.Get().FirstOrDefault(x => x.Uuid == uuid);

        if (server == null)
            return NotFound();

        await Event.Emit($"wings.{shard.Id}.serverFetch", server);

        try //TODO: Remove
        {
            return Converter.FromServer(server);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("{uuid}/install")]
    public async Task<ActionResult<WingsServerInstall>> GetServerInstall(Guid uuid)
    {
        var tokenData = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var id = tokenData.Split(".")[0];
        var token = tokenData.Split(".")[1];

        var shard = ShardRepository.Get().FirstOrDefault(x => x.TokenId == id);

        if (shard == null)
            return NotFound();

        if (token != shard.Token)
            return Unauthorized();

        var server = ServerRepository.Get().Include(x => x.Image).FirstOrDefault(x => x.Uuid == uuid);

        if (server == null)
            return NotFound();

        await Event.Emit($"wings.{shard.Id}.serverInstallFetch", server);

        if (server.Installing)
        {
            Logger.Debug("Real install detected");
            
            return new WingsServerInstall()
            {
                Entrypoint = server.Image.InstallEntrypoint,
                Script = server.Image.InstallScript!,
                Container_Image = server.Image.InstallDockerImage
            };   
        }
        else
        {
            return new WingsServerInstall()
            {
                Entrypoint = server.Image.InstallEntrypoint,
                Script = "exit 0",
                Container_Image = server.Image.InstallDockerImage
            };
        }
    }

    [HttpPost("{uuid}/install")]
    public async Task<ActionResult> SetInstallState(Guid uuid)
    {
        var tokenData = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var id = tokenData.Split(".")[0];
        var token = tokenData.Split(".")[1];

        var shard = ShardRepository.Get().FirstOrDefault(x => x.TokenId == id);

        if (shard == null)
            return NotFound();

        if (token != shard.Token)
            return Unauthorized();

        var server = ServerRepository.Get().Include(x => x.Image).FirstOrDefault(x => x.Uuid == uuid);

        if (server == null)
            return NotFound();

        await ShardServerService.UnlockServer(server);
        
        if (server.Installing)
        {
            server.Installing = false;
            ServerRepository.Update(server);

            await Event.Emit($"wings.{shard.Id}.serverInstallComplete", server);
            await Event.Emit($"server.{server.Uuid}.installComplete", server);
        }
        else // If the server was not installing but this endpoint was called,
             // we know the shard server transfer was successful
        {
            await Event.Emit($"server.{server.Uuid}.reconnect");
        }

        return Ok();
    }
}