namespace Moonlight.App.Database.Entities;

public class SupportTicketTag
{
    public int Id { get; set; }
    public string Tag { get; set; } = "";
    public string Description { get; set; } = "";
    public string ColorHex { get; set; } = "002e5d";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}