namespace PlayerAuctionApp.Domain;

public sealed class AuctionPurchase
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public int PricePoints { get; set; }
    public PurchasePaymentMethod PaymentMethod { get; set; }
    public DateTime PurchasedAtUtc { get; set; }
}

public enum PurchasePaymentMethod
{
    Points,
    Cash,
}
