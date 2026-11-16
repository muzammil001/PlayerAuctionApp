namespace PlayerAuctionApp.Domain;

public sealed class Team
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? LogoImage { get; set; }
    public TeamOwner Owner { get; set; } = null!;
    public ICollection<AuctionPurchase> Purchases { get; set; } = new List<AuctionPurchase>();
    public ICollection<AuctionBid> Bids { get; set; } = new List<AuctionBid>();
}
