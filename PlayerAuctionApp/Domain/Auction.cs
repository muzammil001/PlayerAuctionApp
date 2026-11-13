namespace PlayerAuctionApp.Domain;

public sealed class Auction
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "Cricket Player Auction";
    public int MaximumPlayerCount { get; set; } = 96;
    public int MaximumSquadSize { get; set; } = 16;
    public int StartingBudgetPoints { get; set; } = 100_000;
    public int CashPurchaseLimit { get; set; } = 3;
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Player> Players { get; set; } = new List<Player>();
}
