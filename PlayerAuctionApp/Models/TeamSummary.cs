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
    int SquadCount);
