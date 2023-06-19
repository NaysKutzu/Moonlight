 using Moonlight.App.Database;
using Moonlight.App.Events;
using Moonlight.App.Services.Files;

namespace Moonlight.App.Services.Tickets;

public class TicketClientService
{
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly EventSystem Event;
    private readonly BucketService BucketService;
    private readonly DataContext DataContext;

    public TicketClientService(
        IServiceScopeFactory serviceScopeFactory,
        EventSystem eventSystem,
        BucketService bucketService,
        DataContext dataContext)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Event = eventSystem;
        BucketService = bucketService;
        DataContext = dataContext;
    }
    
    
    
}