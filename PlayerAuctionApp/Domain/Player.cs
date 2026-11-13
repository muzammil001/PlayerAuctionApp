namespace PlayerAuctionApp.Domain;

public sealed class Player
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string City { get; set; } = string.Empty;
    public PlayerRole Role { get; set; }
    public BattingHand? BattingHand { get; set; }
    public string? BowlingStyle { get; set; }
    public string? PhotoDataUrl { get; set; }
    public int BasePricePoints { get; set; }
    public AuctionPurchase? Purchase { get; set; }
}

public enum PlayerRole
{
    Batter,
    Bowler,
    AllRounder,
    WicketKeeper,
}

public enum BattingHand
{
    Right,
    Left,
}
