using Moonlight.App.Models.Misc;

namespace Moonlight.App.Database.Entities;

public class SupportTicket
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public User Owner { get; set; } = null!;
    public SupportTicketType Type { get; set; }
    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Unclaimed;
    public SupportTicketPriority Priority { get; set; } = SupportTicketPriority.Normal;
    public List<SupportTicketTag> Tags { get; set; } = new();
    public List<SupportTicketMessage> Messages { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}