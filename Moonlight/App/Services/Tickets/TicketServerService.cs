using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Moonlight.App.Database.Entities;
using Moonlight.App.Events;
using Moonlight.App.Models.Misc;
using Moonlight.App.Repositories;
using Moonlight.App.Services.Files;
using Moonlight.Shared.Components.Partials;

namespace Moonlight.App.Services.Tickets;

public class TicketServerService
{
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly EventSystem Event;
    private readonly BucketService BucketService;

    public TicketServerService(
        IServiceScopeFactory serviceScopeFactory,
        EventSystem eventSystem,
        BucketService bucketService)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Event = eventSystem;
        BucketService = bucketService;
    }

    public bool CanOpenTicket(User user)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var ticketRepo = scope.ServiceProvider.GetRequiredService<Repository<SupportTicket>>();
        var configService = scope.ServiceProvider.GetRequiredService<ConfigService>();
        
        var maxTickets = configService.GetSection("Moonlight").GetSection("TicketSystem").GetValue<int>("MaxTicketCount");
        var tickets = ticketRepo.Get().Where(x => x.Owner == user);

        if (tickets.Count() <= maxTickets)
        {
            return true;
        }
        
        return false;
    }
    
    public Task<List<SupportTicket>> GetOpenTickets(User? user = null)
    {
        var result = new List<SupportTicket>();

        using var scope = ServiceScopeFactory.CreateScope();
        var ticketRepo = scope.ServiceProvider.GetRequiredService<Repository<SupportTicket>>();
        var userRepo = scope.ServiceProvider.GetRequiredService<Repository<User>>();
        var u = user == null ? null : userRepo.Get().First(x => x.Id == user.Id);

        result = user == null ?
            ticketRepo.Get()
                .Include(x => x.Messages)
                .Include(x => x.Owner).ToList()
            :
            ticketRepo.Get()
                .Where(x => x.Owner.Id == u.Id)
                .Include(x => x.Messages)
                .Include(x => x.Owner).ToList();

        return Task.FromResult(result);
    }
    
    public Task<SupportTicket?> CreateNewTicket(User user, String title, int serviceId, SupportTicketType type, SupportTicketPriority priority)
    {
        if (string.IsNullOrEmpty(title))
            return Task.FromResult<SupportTicket?>(null);
        
        using var scope = ServiceScopeFactory.CreateScope();
        var ticketRepo = scope.ServiceProvider.GetRequiredService<Repository<SupportTicket>>();
        var userRepo = scope.ServiceProvider.GetRequiredService<Repository<User>>();
        var u = userRepo.Get().First(x => x.Id == user.Id);

        var ticket = ticketRepo.Add(new SupportTicket
        {
            Owner = u,
            Title = title,
            Type = type,
            Priority = priority,
            ServiceId = serviceId,
        });
        
        return Task.FromResult<SupportTicket?>(ticket);
    }
    
    public async Task<SupportTicket> CreateTicketMessage(User user, string content, SupportTicket ticket, IBrowserFile? browserFile = null)
    {
        if (string.IsNullOrEmpty(content))
            return ticket;
        
        string? attachment = null;
        using var scope = ServiceScopeFactory.CreateScope();
        var ticketRepo = scope.ServiceProvider.GetRequiredService<Repository<SupportTicket>>();
        var userRepo = scope.ServiceProvider.GetRequiredService<Repository<User>>();
        var u = userRepo.Get().First(x => x.Id == user.Id);
        var supportTicket = ticketRepo.Get()
            .Include(x => x.Owner)
            .First(x => x.Id == ticket.Id);

        if (browserFile != null)
        {
            attachment = await BucketService.StoreFile(
                "supportTicket",
                browserFile.OpenReadStream(1024 * 1024 * 5),
                browserFile.Name);
        }
        
        supportTicket.Messages.Add(new SupportTicketMessage
        {
            User = u,
            Message = content,
            Attachment = attachment ?? ""
        });
        
        ticketRepo.Update(supportTicket);

        return supportTicket;
    }
    
}