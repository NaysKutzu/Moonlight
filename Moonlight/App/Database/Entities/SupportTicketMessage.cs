namespace Moonlight.App.Database.Entities;

public class SupportTicketMessage
{
    public int Id { get; set; }
    public string Message { get; set; } = "";
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}