using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Models;

public sealed record PlayerSummary(
    Guid Id,
    string FullName,
    DateOnly DateOfBirth,
    int Age,
    string City,
    PlayerRole Role,
    BattingHand BattingHand,
    string? BowlingStyle,
    int BasePricePoints,
    string? Photo,
    string? TeamName);
