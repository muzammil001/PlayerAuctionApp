using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Data;
using PlayerAuctionApp.Domain;
using PlayerAuctionApp.Models;

namespace PlayerAuctionApp.Services;

public sealed class AuctionPlayerService(IDbContextFactory<AuctionDbContext> contextFactory)
{
    public async Task<IReadOnlyList<PlayerSummary>> GetPlayersAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var players = await db.Players.AsNoTracking()
            .Where(player => player.AuctionId == AuctionDbContext.DefaultAuctionId)
            .Include(player => player.Purchase).ThenInclude(purchase => purchase!.Team)
            .OrderBy(player => player.FullName)
            .ToArrayAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return players.Select(player => new PlayerSummary(
            player.Id,
            player.FullName,
            player.DateOfBirth!.Value,
            CalculateAge(player.DateOfBirth.Value, today),
            player.City,
            player.Role,
            player.BattingHand!.Value,
            player.BowlingStyle,
            player.BasePricePoints,
            player.PhotoDataUrl,
            player.Purchase?.Team.Name)).ToArray();
    }

    public async Task<int> GetPlayerCountAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Players.CountAsync(
            player => player.AuctionId == AuctionDbContext.DefaultAuctionId, cancellationToken);
    }

    public async Task<string?> AddPlayerAsync(NewPlayerInput input, CancellationToken cancellationToken = default)
    {
        if (input.Role is null || input.BattingHand is null || input.DateOfBirth is null)
        {
            return "Enter the player's date of birth, role, and batting hand.";
        }

        var dateOfBirth = input.DateOfBirth.Value;
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return "Date of birth cannot be in the future.";
        }

        if (!string.IsNullOrEmpty(input.PhotoDataUrl) && input.PhotoDataUrl.Length > 2_800_000)
        {
            return "The player photo is too large. Choose an image up to 2 MB.";
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var auction = await db.Auctions.SingleAsync(
            item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);

        var playerCount = await db.Players.CountAsync(
            player => player.AuctionId == auction.Id, cancellationToken);
        if (playerCount >= auction.MaximumPlayerCount)
        {
            return $"The auction already has the maximum of {auction.MaximumPlayerCount} players.";
        }

        if (string.IsNullOrWhiteSpace(input.City))
        {
            return "Select the player's city.";
        }

        db.Players.Add(new Player
        {
            Id = Guid.NewGuid(),
            AuctionId = auction.Id,
            FullName = input.FullName.Trim(),
            DateOfBirth = dateOfBirth,
            City = input.City.Trim(),
            Role = input.Role.Value,
            BattingHand = input.BattingHand.Value,
            BowlingStyle = string.IsNullOrWhiteSpace(input.BowlingStyle) ? null : input.BowlingStyle.Trim(),
            BasePricePoints = input.BasePricePoints,
            PhotoDataUrl = input.PhotoDataUrl,
        });

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return null;
    }

    private static int CalculateAge(DateOnly dateOfBirth, DateOnly today)
    {
        var age = today.Year - dateOfBirth.Year;
        return dateOfBirth > today.AddYears(-age) ? age - 1 : age;
    }
}
