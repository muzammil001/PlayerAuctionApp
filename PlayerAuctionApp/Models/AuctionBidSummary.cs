using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Models;

public sealed record AuctionBidSummary(
    Guid TeamId,
    string TeamName,
    string OwnerName,
    int Amount,
    PurchasePaymentMethod PaymentMethod,
    DateTime PlacedAtUtc);
