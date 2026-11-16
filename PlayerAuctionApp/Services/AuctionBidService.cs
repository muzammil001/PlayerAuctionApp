using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Data;
using PlayerAuctionApp.Domain;
using PlayerAuctionApp.Models;

namespace PlayerAuctionApp.Services;

public sealed class AuctionBidService(IDbContextFactory<AuctionDbContext> contextFactory)
{
    public async Task<IReadOnlyList<AuctionBidSummary>> GetBidsAsync(Guid playerId, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var bids = await db.AuctionBids.AsNoTracking()
            .Where(bid => bid.AuctionId == AuctionDbContext.DefaultAuctionId && bid.PlayerId == playerId)
            .Include(bid => bid.Team).ThenInclude(team => team.Owner)
            .OrderByDescending(bid => bid.Amount).ThenByDescending(bid => bid.PlacedAtUtc)
            .ToArrayAsync(cancellationToken);
        return bids.Select(bid => new AuctionBidSummary(
            bid.TeamId, bid.Team.Name, bid.Team.Owner.FullName, bid.Amount, bid.PaymentMethod, bid.PlacedAtUtc)).ToArray();
    }

    public async Task<string?> PlaceBidAsync(Guid playerId, Guid teamId, int amount, PurchasePaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        if (amount <= 0) return "Enter a valid bid amount.";

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var auction = await db.Auctions.SingleAsync(item => item.Id == AuctionDbContext.DefaultAuctionId, cancellationToken);
        if (auction.StartedAtUtc is null) return "Start the auction before accepting bids.";

        var player = await db.Players.SingleOrDefaultAsync(item => item.Id == playerId && item.AuctionId == auction.Id, cancellationToken);
        if (player is null) return "This player is not in the auction pool.";
        if (await db.AuctionPurchases.AnyAsync(purchase => purchase.PlayerId == playerId, cancellationToken)) return "This player has already been sold.";

        var highestBid = await db.AuctionBids.Where(bid => bid.PlayerId == playerId)
            .OrderByDescending(bid => bid.Amount).Select(bid => (int?)bid.Amount).FirstOrDefaultAsync(cancellationToken);
        var minimumBid = highestBid is null ? player.BasePricePoints : highestBid.Value + 500;
        if (amount < minimumBid) return $"The next bid must be at least {minimumBid:N0} points.";

        var team = await db.Teams.SingleOrDefaultAsync(item => item.Id == teamId && item.AuctionId == auction.Id, cancellationToken);
        if (team is null) return "Choose a team for this bid.";
        var purchases = await db.AuctionPurchases.Where(purchase => purchase.TeamId == teamId).ToArrayAsync(cancellationToken);
        if (purchases.Length >= auction.MaximumSquadSize) return $"{team.Name} already has a full squad.";
        if (paymentMethod == PurchasePaymentMethod.Cash)
        {
            var cashCount = purchases.Count(purchase => purchase.PaymentMethod == PurchasePaymentMethod.Cash);
            if (cashCount >= auction.CashPurchaseLimit) return $"{team.Name} has used all cash purchases.";
        }
        else
        {
            var spent = purchases.Where(purchase => purchase.PaymentMethod == PurchasePaymentMethod.Points).Sum(purchase => purchase.PricePoints);
            var remaining = auction.StartingBudgetPoints - spent;
            if (amount > remaining) return $"{team.Name} only has {remaining:N0} points remaining.";
        }

        db.AuctionBids.Add(new AuctionBid
        {
            Id = Guid.NewGuid(), AuctionId = auction.Id, PlayerId = playerId, TeamId = teamId,
            Amount = amount, PaymentMethod = paymentMethod, PlacedAtUtc = DateTime.UtcNow,
        });
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return null;
    }
}
