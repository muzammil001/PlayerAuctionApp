namespace PlayerAuctionApp.Domain;

public sealed class AuctionBid
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; } = null!;
    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public int Amount { get; set; }
    public PurchasePaymentMethod PaymentMethod { get; set; }
    public DateTime PlacedAtUtc { get; set; }
}
