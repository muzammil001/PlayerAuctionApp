namespace PlayerAuctionApp.Domain;

public sealed class TeamOwner
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}
