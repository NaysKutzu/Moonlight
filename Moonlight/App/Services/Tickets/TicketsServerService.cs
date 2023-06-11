using Microsoft.EntityFrameworkCore;
using Moonlight.App.Database.Entities;
using Moonlight.App.Events;
using Moonlight.App.Models.Misc;
using Moonlight.App.Repositories;

namespace Moonlight.App.Services.Tickets;

public class TicketServerService
{
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly DateTimeService DateTimeService;
    private readonly EventSystem Event;
    
    public TicketServerService(
        IServiceScopeFactory serviceScopeFactory,
        DateTimeService dateTimeService,
        EventSystem eventSystem)
    {
        ServiceScopeFactory = serviceScopeFactory;
        DateTimeService = dateTimeService;
        Event = eventSystem;
    }

    public SupportTicket CreateNewTicket(User user, String title)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var ticketRepo = scope.ServiceProvider.GetRequiredService<Repository<SupportTicket>>();

        var ticket = ticketRepo.Add(new SupportTicket()
        {
            Owner = user,
            Title = title,
            Priority = SupportTicketPriority.Normal,
            Status = SupportTicketStatus.Unclaimed
        });
        
        return ticket;
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

    public Task<Dictionary<SupportTicket, SupportTicketMessage?>> GetOpenTickets()
    {
        var result = new Dictionary<SupportTicket, SupportTicketMessage?>();

        using var scope = ServiceScopeFactory.CreateScope();
        var ticketRepo = scope.ServiceProvider.GetRequiredService<Repository<SupportTicket>>();

        foreach (var ticket in ticketRepo.Get().Include(x => x.Messages).ToArray())
        {
            var messages = ticket.Messages;
            
            result.Add(ticket, messages.Last());
        }

        return Task.FromResult(result);
    }
}