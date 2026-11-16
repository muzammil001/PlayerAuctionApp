namespace PlayerAuctionApp.Models;

public sealed record TeamSummary(
    Guid Id,
    string Name,
    string City,
    string? LogoImage,
    string OwnerName,
    string OwnerPhone,
    string? OwnerEmail,
    int RemainingPoints,
    int CashPurchasesRemaining,
    int SquadCount,
    IReadOnlyList<TeamPlayerSummary> Squad);

public sealed record TeamPlayerSummary(
    Guid Id,
    string FullName,
    string City,
    string Role,
    string? Photo,
    int PurchasePrice,
    string PaymentMethod,
    DateTime PurchasedAtUtc);
